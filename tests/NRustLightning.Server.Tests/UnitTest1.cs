using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Tests.Support;
using Xunit;

namespace NRustLightning.Server.Tests
{
    public class UnitTest1
    {
        public IServiceProvider sp;

        [Fact]
        public async Task CanConvertJsonTypes()
        {
            var invoice =
                "lnbc1p0vhtzvpp5akajlfqdj6ek7eeh4kae6gc05fz9j99n8jadatqt4fmlwwxwx4zsnp4q2uqg2j52gxtxg5d0v928h5pll95ynsaek2csgfg26tvuzydgjrwgdqhdehjqer9wd3hy6tsw35k7msna3vtx";
            var paymentRequest = PaymentRequest.Parse(invoice);
            var resp = new InvoiceResponse() { Invoice = paymentRequest.ResultValue };
            JsonSerializer.Serialize(resp);
        }

        [Fact]
        public async Task CanGetNewInvoice()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost(webHost =>
            {
                webHost.UseEnvironment("Development");
                var curr = Directory.GetCurrentDirectory();
                webHost.UseContentRoot(curr);
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
                webHost.UseTestServer();
            });

            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var resp = await c.GetInvoiceAsync(new InvoiceCreationOption());
            Assert.Null(resp.Invoice.AmountValue.ToNullable());
        }
    }
}
