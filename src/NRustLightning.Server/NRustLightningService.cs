using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Services;

namespace NRustLightning.Server
{
    public static class ServiceCollectionExtension
    {
        public static void AddNRustLightning(this IServiceCollection services)
        {
            services.AddSingleton<ISocketDescriptorFactory, SocketDescriptorFactory>();
            services.AddSingleton<IKeysRepository, InMemoryKeysRepository>();
            services.AddSingleton<IRPCClientProvider, RPCClientProvider>();
            services.AddSingleton<NBXplorerClientProvider>();
            services.AddSingleton<PeerManagerProvider>();
        }

        public static void ConfigureNRustLightning(this IServiceCollection services, IConfiguration configuration)
        {
            var network = configuration.GetValue<NetworkType>("network");
            var networkProvider = new NRustLightningNetworkProvider(network);
            services.AddSingleton(networkProvider);
            foreach (var n in networkProvider.GetAll())
            {
                var chainSettings = configuration.GetSection(n.CryptoCode);
                if (!(chainSettings is null))
                {
                    services.Configure<ChainConfiguration>(n.CryptoCode, chainSettings);
                }
            }
        }
    }
}