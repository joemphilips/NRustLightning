using System.IO;
using LSATAuthenticationHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Repository;

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
                services.AddSingleton<IKeysRepository, InMemoryKeysRepository>();
                services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
                services.AddSingleton<IMacaroonSecretRepository, InMemoryMacaroonSecretRepository>();
                services.AddSingleton<ILSATInvoiceProvider, InMemoryInvoiceRepository>();
            });
            webHost.UseTestServer();
        }
    }
}