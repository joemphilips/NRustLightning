using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Tests.Support;
using Xunit;

namespace NRustLightning.Server.Tests
{
    public class UnitTest1
    {
        public IServiceProvider sp;

        [Fact]
        public async Task CanGetNewInvoice()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseEnvironment("Development");
                webHost.UseStartup<Startup>();
            });

            var host = await hostBuilder.StartAsync();
            var c = host.GetNRustLightningClient();
            var invoice = await c.GetInvoiceAsync();
            Assert.Null(invoice.AmountValue.ToNullable());
        }
    }
}
