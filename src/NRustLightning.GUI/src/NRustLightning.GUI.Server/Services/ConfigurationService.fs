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
    let lockObj = new SemaphoreSlim(1, 1)
    
    member internal this.CommitConfig(newConf) = async {
        try
            do! lockObj.WaitAsync() |> Async.AwaitTask
            do File.Delete(jsonPath)
            use fs = File.OpenWrite(jsonPath)
            do! JsonSerializer.SerializeAsync(fs, newConf) |> Async.AwaitTask
        finally
            lockObj.Release() |> ignore
    }
    member private this.ValidateConfigAsync(conf: WalletBiwaConfiguration): Async<string option> = async {
        let cli = conf.GetRPCClient(httpClientFactory.CreateClient("ConfigurationService"))
        try
            let! _info = cli.GetBlockchainInfoAsync() |> Async.AwaitTask
            try
                let! _ = cli.GetBlockHeaderAsync(conf.GetNetwork().GenesisHash) |> Async.AwaitTask
                return None
            with
            | ex ->
                return Some(sprintf "Failed to get genesis block! Are you sure this node is running in expected network?: %s" ex.Message)
        with
        | :? AggregateException as aex when aex.InnerExceptions.Any(fun e -> e :? HttpRequestException) ->
            return Some(sprintf "Failed to connect to rpc server: %s" aex.Message)
        | :? RPCException as ex ->
            return (Some(sprintf "Failed to connect to rpc server: %s" ex.Message))
        | ex -> return Some (sprintf "Unexpected Error: %s" (ex.ToString()))
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
            return opts.CurrentValue
        }
    }
