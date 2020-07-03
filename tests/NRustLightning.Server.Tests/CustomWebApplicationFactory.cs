using System.IO;
using LSATAuthenticationHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Services;
using NRustLightning.Server.Tests.Stubs;

namespace NRustLightning.Server.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<NRustLightning.Server.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder webHost)
        {
            webHost.UseEnvironment("Development");
            var curr = Directory.GetCurrentDirectory();
            webHost.UseContentRoot(curr);
            webHost.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.AddJsonFile("appsettings.Development.json");
            });
            webHost.UseStartup<Startup>();
            webHost.ConfigureTestServices(services =>
            {
                services.AddSingleton<Network>(Network.RegTest);
                services.AddSingleton<IKeysRepository, InMemoryKeysRepository>();
                services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
                services.AddSingleton<IMacaroonSecretRepository, InMemoryMacaroonSecretRepository>();
                services.AddSingleton<ILSATInvoiceProvider, InMemoryInvoiceRepository>();
                services.AddSingleton<IFeeEstimator, TestFeeEstimator>();
                services.AddSingleton<IBroadcaster, TestBroadcaster>();
                services.AddSingleton<IChainWatchInterface, TestChainWatchInterface>();
                services.AddSingleton<IPeerManagerProvider, TestPeerManagerProvider>();
                services.AddSingleton<IWalletService, StubWalletService>();
                services.AddSingleton<INBXplorerClientProvider, StubNBXplorerClientProvider>();
            });
            webHost.UseTestServer();
        }
    }
}