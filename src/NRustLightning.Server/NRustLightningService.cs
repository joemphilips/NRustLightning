using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Configuration.SubConfiguration;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Middlewares;
using NRustLightning.Server.Networks;
using NRustLightning.Server.P2P;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NRustLightning.Server
{
    public static class ServiceCollectionExtension
    {
        public static void AddNRustLightning(this IServiceCollection services)
        {
            services.AddSingleton<ISocketDescriptorFactory, SocketDescriptorFactory>();
            services.AddSingleton<IKeysRepository, FlatFileKeyRepository>();
            services.AddSingleton<IRPCClientProvider, RPCClientProvider>();
            services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
            services.AddSingleton<P2PConnectionFactory>();
            services.AddSingleton<P2PConnectionHandler>();
            services.AddSingleton<NBXplorerClientProvider>();
            services.AddSingleton<PeerManagerProvider>();
            services.AddTransient<RequestResponseLoggingMiddleware>();
        }

        public static void ConfigureNRustLightning(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var network = configuration.GetValue<NetworkType>("network");
            var networkProvider = new NRustLightningNetworkProvider(network);
            services.AddSingleton(networkProvider);
            services.AddLogging();
            services.Configure<Config>(o => { o.LoadArgs(configuration, logger); });
        }
    }
}