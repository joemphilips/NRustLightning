using System.IO;
using System.Threading.Channels;
using DotNetLightning.Utils;
using LSATAuthenticationHandler;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Middlewares;
using NRustLightning.Server.P2P;
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
            services.AddSingleton<IRepository, DbTrieRepository>();
            services.AddSingleton<InvoiceService>();
            services.AddSingleton<RepositoryProvider>();
            services.AddSingleton<IWalletService, WalletService>();
            services.AddSingleton<IConnectionFactory, P2PConnectionFactory>();
            services.AddSingleton<P2PConnectionHandler>();
            services.AddSingleton<INBXplorerClientProvider, NBXplorerClientProvider>();
            services.AddSingleton<PeerManagerProvider>();
            services.AddTransient<RequestResponseLoggingMiddleware>();
            services.AddSingleton<ChannelProvider>();
            services.AddSingleton<EventAggregator>();
            
            services.AddHostedService(sp => sp.GetRequiredService<PeerManagerProvider>());
            services.AddHostedService<NBXplorerListeners>();
            services.AddHostedService<RustLightningEventReactors>();
        }

        public static void ConfigureNRustLightning(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var network = configuration.GetNetworkType();
            var networkProvider = new NRustLightningNetworkProvider(network);
            services.AddSingleton(networkProvider);
            services.AddLogging();
            services.Configure<Config>(o => { o.LoadArgs(configuration, logger); });
        }

        public static void ConfigureNRustLightningAuth(this IServiceCollection services, IConfiguration Configuration)
        {
            // configure lsat/macaroon authentication
            services.AddSingleton<IMacaroonSecretRepository, InMemoryMacaroonSecretRepository>();
            // services.AddSingleton<ILSATInvoiceProvider, InMemoryRepository>();
            
            string? ourServiceName = null;
            int ourServiceTier = 0;
            var lsatConfig = Configuration.GetSection("LSAT");
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = LSATDefaults.Scheme;
                options.DefaultChallengeScheme = LSATDefaults.Scheme;
            }).AddLSATAuthentication(options =>
            {
                options.ServiceName = "nrustlightning";
                options.ServiceTier = ourServiceTier;
                lsatConfig.Bind(options);
                ourServiceName = options.ServiceName;
                ourServiceTier = options.ServiceTier;
                // we want to give users only read capability when they have payed for it. not write.
                options.MacaroonCaveats.Add($"{ourServiceName}{DotNetLightning.Payment.LSAT.Constants.CAPABILITIES_CONDITION_PREFIX}=read");
                int amount = lsatConfig.GetOrDefault("amount", 1);
                options.InvoiceAmount = LNMoney.MilliSatoshis(amount);
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Readonly", policy =>
                {
                    policy.RequireClaim("service", $"{ourServiceName}:{ourServiceTier}");
                    policy.RequireClaim($"{ourServiceName}{DotNetLightning.Payment.LSAT.Constants.CAPABILITIES_CONDITION_PREFIX}", "read", "admin");
                });
                
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim("service", $"{ourServiceName}:{ourServiceTier}");
                    policy.RequireClaim($"{ourServiceName}{DotNetLightning.Payment.LSAT.Constants.CAPABILITIES_CONDITION_PREFIX}", "admin");
                });
            });
        }
    }
}