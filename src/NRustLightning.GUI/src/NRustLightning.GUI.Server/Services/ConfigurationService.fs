namespace NRustLightning.GUI.Server.Services

open System
open System.Linq
open System.IO
open System.Net.Http
open System.Net.Sockets
open System.Text.Json
open System.Threading
open Bolero.Remoting.Server
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open NBitcoin.RPC
open NRustLightning.GUI.Client.Configuration


type ConfigurationService(ctx: IRemoteContext, env: IWebHostEnvironment, conf: IConfiguration, opts: IOptionsMonitor<WalletBiwaConfiguration>, httpClientFactory: IHttpClientFactory) as this =
    inherit RemoteHandler<ConfigurationModule.ConfigurationService>()
#if DEBUG
    let path = "appsettings.Development.json"
#else
    let path = "appsettings.json"
#endif
    let jsonPath = Path.Combine(env.ContentRootPath, path)
    let mutable configs = opts.CurrentValue
    let lockObj = new SemaphoreSlim(1, 1)
    
    do
        printfn "configs url: %A" configs.Url
        printfn "And Network: %A" configs.Network
    
    member internal this.CommitConfig(newConf) = async {
        try
            do! lockObj.WaitAsync() |> Async.AwaitTask
            use fs = File.OpenWrite(jsonPath)
            configs <- newConf
            do! JsonSerializer.SerializeAsync(fs, configs) |> Async.AwaitTask
        finally
            lockObj.Release() |> ignore
    }
    member private this.ValidateConfigAsync(conf: WalletBiwaConfiguration): Async<string option> = async {
        let cli = conf.GetRPCClient(httpClientFactory.CreateClient("ConfigurationService"))
        try
            let! _info = cli.GetBlockchainInfoAsync() |> Async.AwaitTask
            return None
        with
        | :? AggregateException as aex when aex.InnerExceptions.Any(fun e -> e :? HttpRequestException) ->
            return Some(sprintf "Failed to connect to rpc server: %s" aex.Message)
        | :? RPCException as ex ->
            return (Some(sprintf "Failed to connect to rpc server: %s" ex.Message))
        | ex -> return Some (sprintf "Unexpected Error: %s" ex.Message)
    }
        
    override this.Handler = {
        Update = fun newConf -> async {
            match! this.ValidateConfigAsync(newConf) with
            | Some x -> return Error x
            | None ->
            do! this.CommitConfig(newConf)
            return Ok()
        }
        
        LoadConfig = fun () -> async {
            return configs
        }
    }
