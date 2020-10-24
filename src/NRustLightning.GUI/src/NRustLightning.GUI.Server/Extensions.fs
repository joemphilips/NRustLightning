namespace NRustLightning.GUI.Server

open Microsoft.AspNetCore.Connections
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices
open NBitcoin
open NRustLightning.GUI.Client
open NRustLightning.GUI.Client.AppState
open NRustLightning.GUI.Client.Configuration
open NRustLightning.Infrastructure.Interfaces
open NRustLightning.Infrastructure.Networks
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Configuration
open NRustLightning.Net
open NRustLightning.GUI.Server.InternalServices

[<Extension;AbstractClass;Sealed>]
type Extensions() =
    [<Extension>]
    static member GetNetworkType(conf: IConfiguration): NetworkType =
        let network = conf.GetOrDefault<string>("network", "regtest")
        let n = Network.GetNetwork(network)
        n.NetworkType
            
    [<Extension>]
    static member ConfigureNRustLightning(services: IServiceCollection, config: IConfiguration) =
        let networkType = config.GetNetworkType()
        services
            .AddSingleton(NRustLightningNetworkProvider(networkType))
            .AddSingleton(AppState())
            .Configure<WalletBiwaConfiguration>(config)
        
    [<Extension>]
    static member AddNRustLightning(services: IServiceCollection) =
        services
            .AddSingleton<ISocketDescriptorFactory, SocketDescriptorFactory>()
            .AddSingleton<IKeysRepository, FlatFileKeyRepository>()
            .AddSingleton<IRPCClientProvider, RPCClientProvider>()
            .AddSingleton<IRepository, DbTrieRepository>()
