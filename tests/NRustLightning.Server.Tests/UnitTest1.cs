using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.FSharp.Core;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Tests.Support;
using Xunit;

namespace NRustLightning.Server.Tests
{
    public class UnitTest1 : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _fixture;
        public IServiceProvider sp;
        
        public UnitTest1(CustomWebApplicationFactory fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CanConvertJsonTypes()
        {
            var invoice =
                "lnbc1p0vhtzvpp5akajlfqdj6ek7eeh4kae6gc05fz9j99n8jadatqt4fmlwwxwx4zsnp4q2uqg2j52gxtxg5d0v928h5pll95ynsaek2csgfg26tvuzydgjrwgdqhdehjqer9wd3hy6tsw35k7msna3vtx";
            var paymentRequest = PaymentRequest.Parse(invoice);
            var resp = new InvoiceResponse() { Invoice = paymentRequest.ResultValue };
            var j = JsonSerializer.Serialize(resp);
            JsonSerializer.Deserialize<InvoiceResponse>(j);
            var invoiceResponseRaw = "{\"invoice\":\"lnbc1p0vma42pp5t2v5ehyay3x9g8769gqkrhmdlqjq0kc8ksqfxu3xjw7s2y96jegqnp4q2uqg2j52gxtxg5d0v928h5pll95ynsaek2csgfg26tvuzydgjrwgdqhdehjqer9wd3hy6tsw35k7ms3xhenl\"}";
            JsonSerializer.Deserialize<InvoiceResponse>(invoiceResponseRaw);
        }

        [Fact]
        public async Task CanGetNewInvoice()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var resp = await c.GetInvoiceAsync(new InvoiceCreationOption());
            Assert.Equal(resp.Invoice.AmountValue, FSharpOption<LNMoney>.None);
            Assert.True(resp.Invoice.Expiry > DateTimeOffset.UnixEpoch);
            Assert.False(resp.Invoice.IsExpired);
            Assert.Null(resp.Invoice.AmountValue.ToNullable());
        }

        [Fact]
        public async Task CanGetChannelList()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var resp = await c.GetChannelDetails();
            Assert.NotNull(resp);
            Assert.Empty(resp.Details);
        }

        [Fact]
        public async Task CanGetOnChainAddress()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var resp = await c.GetNewDepositAddressAsync();
            Assert.NotNull(resp.Address);
        }

        [Fact]
        public async Task CanGetWalletInfo()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var w = await c.GetWalletInfoAsync();
            Assert.NotNull(w);
            Assert.NotNull(w.DerivationStrategy);
        }

        [Fact]
        public async Task CanGetInfo()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var c = host.GetTestNRustLightningClient();
            var info = await c.GetInfoAsync();
            Assert.NotNull(info);
            Assert.Equal(0, info.NumConnected);
            Assert.NotNull(info.ConnectionString);
            Assert.Empty(info.NodeIds);
        }

        [Fact]
        public async Task LSATAuthenticationTest()
        {
            var hostBuilder = new HostBuilder().ConfigureTestHost();
            using var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            return;
        }
    }
}
