namespace NRustLightning.GUI.Server

open System.IO
open System.Text.Json
open System.Threading
open Bolero.Remoting.Server
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open NBitcoin.RPC
open NRustLightning.GUI.Client.Configuration


type ConfigurationService(ctx: IRemoteContext, env: IWebHostEnvironment, conf: IConfiguration, opts: IOptions<WalletBiwaConfiguration>) =
    inherit RemoteHandler<ConfigurationModule.ConfigurationService>()
#if DEBUG
    let path = "appsettings.Development.json"
#else
    let path = "appsettings.json"
#endif
    let jsonPath = Path.Combine(env.ContentRootPath, path)
    let json = jsonPath |> File.ReadAllText
    let mutable configs = JsonSerializer.Deserialize<WalletBiwaConfiguration>(json)
    let lockObj = new SemaphoreSlim(1, 1)
    
    member private this.CommitConfig(newConf) = async {
        try
            do! lockObj.WaitAsync() |> Async.AwaitTask
            use fs = File.OpenWrite(jsonPath)
            configs <- newConf
            do! JsonSerializer.SerializeAsync(fs, configs) |> Async.AwaitTask
        finally
            lockObj.Release() |> ignore
    }
        
    override this.Handler = {
        Update = fun newConf -> async {
            do! this.CommitConfig(newConf)
        }
        
        LoadConfig = fun () -> async {
            return configs
        }
    }
