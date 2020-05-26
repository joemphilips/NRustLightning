using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Repository;

namespace NRustLightning.Server.Tests.Support
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder webHost)
        {
            webHost.UseTestServer();
            webHost.UseEnvironment("Development");
            webHost.ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.Development.json");
            });
            webHost.UseStartup<Startup>();
            webHost.ConfigureTestServices(services =>
            {
                services.TryAddSingleton<IKeysRepository, InMemoryKeysRepository>();
                services.TryAddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
            });
        }
    }
}