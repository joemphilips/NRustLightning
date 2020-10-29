namespace NRustLightning.GUI.Server.Services

open System
open System.IO
open System.Text.Json
open System.Threading
open Bolero.Remoting.Server
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open NBitcoin.RPC
open NRustLightning.GUI.Client.Configuration


type ConfigurationService(ctx: IRemoteContext, env: IWebHostEnvironment, conf: IConfiguration, opts: IOptions<WalletBiwaConfiguration>) as this =
    inherit RemoteHandler<ConfigurationModule.ConfigurationService>()
#if DEBUG
    let path = "appsettings.Development.json"
#else
    let path = "appsettings.json"
#endif
    let jsonPath = Path.Combine(env.ContentRootPath, path)
    let mutable configs = opts.Value
    let lockObj = new SemaphoreSlim(1, 1)
    
    member internal this.CommitConfig(newConf) = async {
        try
            do! lockObj.WaitAsync() |> Async.AwaitTask
            use fs = File.OpenWrite(jsonPath)
            configs <- newConf
            do! JsonSerializer.SerializeAsync(fs, configs) |> Async.AwaitTask
        finally
            lockObj.Release() |> ignore
    }
    member private this.ValidateConfigAsync(conf: WalletBiwaConfiguration): Async<Result<_,string>> = async {
        return Ok ()
    }
        
    override this.Handler = {
        Update = fun newConf -> async {
            let! r = this.ValidateConfigAsync(newConf)
            match r with
            | Error e -> return Error e
            | Ok _ ->
                do! this.CommitConfig(newConf)
                return Ok()
        }
        
        LoadConfig = fun () -> async {
            return configs
        }
    }
