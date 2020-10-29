namespace NRustLightning.GUI.Server

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices
open Microsoft.Extensions.Logging
open NBitcoin
open NRustLightning.GUI.Client.AppState
open NRustLightning.GUI.Client.Configuration
open NRustLightning.Infrastructure.Interfaces
open NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Configuration
open NRustLightning.Net
open NRustLightning.GUI.Server.Services
open Bolero.Remoting.Server
open Microsoft.Extensions.DependencyInjection;
open MatBlazor

[<Extension;AbstractClass;Sealed>]
type ConfigurationExtension =
    [<Extension>]
    static member LoadArgs(this: WalletBiwaConfiguration, config: IConfiguration, logger: ILogger): unit =
        logger.LogDebug(sprintf "configuration read from file is %A" config)
        let d = WalletBiwaConfiguration.Default
        if (this.DBPath |> String.IsNullOrWhiteSpace) then
           this.DBPath <- config.GetOrDefault<string>("DBPath", d.DBPath)
        this.RPCHost <- config.GetOrDefault("RPCHost", d.RPCHost)
        this.RPCPort <- config.GetOrDefault("RPCPort", d.RPCPort)
        this.RPCPassword <- config.GetOrDefault("RPCPassword", d.RPCPassword)
        this.RPCUser <- config.GetOrDefault("RPCUser", d.RPCUser)
        this.RPCCookieFile <- config.GetOrDefault("RPCCookieFile", d.RPCCookieFile)
        this._Network <- config.GetOrDefault("Network", d._Network)
        ()

[<Extension;AbstractClass;Sealed>]
type Extensions =
    [<Extension>]
    static member GetNetworkType(conf: IConfiguration): NetworkType =
        let networkStr = conf.GetOrDefault<string>("network", "regtest")
        Network.GetNetwork(networkStr).NetworkType
            
    [<Extension>]
    static member ConfigureNRustLightning(services: IServiceCollection, config: IConfiguration, logger: ILogger) =
        services
            .AddRemoting<ConfigurationService>()
            .AddRemoting<WalletService>()
            .AddRemoting<MainService>()
            .AddMatToaster(fun config ->
                config.Position <- MatToastPosition.TopRight
                config.PreventDuplicates <- true
                config.NewestOnTop <- true
                config.ShowCloseButton <- true
                config.VisibleStateDuration <- 30000
                ())
            .Configure<WalletBiwaConfiguration>(fun (w: WalletBiwaConfiguration) -> w.LoadArgs(config, logger)) |> ignore
        services
        
    [<Extension>]
    static member AddNRustLightning(services: IServiceCollection) =
        services
            .AddSingleton<ISocketDescriptorFactory, SocketDescriptorFactory>()
            .AddSingleton<IKeysRepository, FlatFileKeyRepository>()
            .AddSingleton(AppState())
            .AddSingleton<Repository>()
            .AddSingleton<IRepository, DbTrieRepository>()
