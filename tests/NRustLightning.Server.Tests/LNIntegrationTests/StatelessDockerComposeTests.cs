using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using DockerComposeFixture;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;
using OpenChannelRequest = NRustLightning.Infrastructure.Models.Request.OpenChannelRequest;

namespace NRustLightning.Server.Tests.LNIntegrationTests
{
    public class StatelessDockerComposeTests : LNIntegrationTestsBase
    {
        public StatelessDockerComposeTests(DockerFixture dockerFixture, ITestOutputHelper output) : base(dockerFixture, output)
        {
        }

        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanConnectNodes()
        {
            var clients = _clients;
            var blockchainInfo = await clients.BitcoinRPCClient.GetBlockchainInfoAsync();
            Assert.NotNull(blockchainInfo);
            var lndInfo = await clients.LndLNClient.GetInfo();
            Assert.NotEmpty(lndInfo.NodeInfoList);
            var clightningInfo = await clients.ClightningLNClient.GetInfo();
            Assert.NotNull(clightningInfo);
            var info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.NotNull(info.ConnectionString);
            
            // inbound connection (lnd)
            var ourInfo = await clients.NRustLightningHttpClient.GetInfoAsync();
            NodeInfo.TryParse(ourInfo.ConnectionString.ToString(), out var nodeInfo);
            await clients.LndLNClient.ConnectTo(nodeInfo);
            await Task.Delay(200);
            var lndPeerInfo = await clients.LndClient.SwaggerClient.ListPeersAsync();
            Assert.Single(lndPeerInfo.Peers);
            
            // Outbound connection (clightning).
            await clients.NRustLightningHttpClient.ConnectAsync(clightningInfo.NodeInfoList.FirstOrDefault().ToConnectionString());
            await Task.Delay(200);
            var clightningPeerInfo = await clients.CLightningClient.ListPeersAsync();
            Assert.Single(clightningPeerInfo);
            Assert.Equal(clightningPeerInfo.First().Id, ourInfo.ConnectionString.NodeId.ToHex());
            info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.Equal(2,info.NumConnected);

            // Can disconnect from inside (clightning)
            await clients.NRustLightningHttpClient.DisconnectAsync(clightningInfo.NodeInfoList.FirstOrDefault().ToConnectionString());
            info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.Equal(1, info.NumConnected);
            clightningPeerInfo = await clients.CLightningClient.ListPeersAsync();
            Assert.Empty(clightningPeerInfo);

            // Can disconnect from outside (lnd)
            await clients.LndClient.SwaggerClient.DisconnectPeerAsync(info.ConnectionString.NodeId.ToHex());
            lndPeerInfo = await clients.LndClient.SwaggerClient.ListPeersAsync();
            Assert.Empty(lndPeerInfo.Peers);
            info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.Equal(0, info.NumConnected);
            
            // Can disconnect from inside for inbound connection.
            // TODO: after handling events from rl, we must hold PubKey => connection in ConnectionHandler.
            // And then we can disconnect from inside.
        }

        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanOpenCloseChannelsWithLND()
        {
            var clients = _clients;
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotNull(walletInfo.DerivationStrategy);
            Assert.Equal(0, walletInfo.OnChainBalanceSatoshis);
            Assert.DoesNotContain("legacy", walletInfo.DerivationStrategy.ToString());
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await OutBoundChannelOpenRoundtrip(clients, clients.LndLNClient);
            await OutboundChannelCloseRoundtrip(clients, clients.LndLNClient);
            await InboundChannelOpenRoundtrip(clients, clients.LndClient);
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanSendInboundPaymentFromLnd()
        {
            var clients = _clients;
            
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await InboundChannelOpenRoundtrip(clients, clients.LndClient);
            // ---- payment tests ----
            await InboundPaymentRoundTrip(clients, clients.LndClient);
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanSendOutboundPaymentToLnd()
        {
            var clients = _clients;
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await OutBoundChannelOpenRoundtrip(clients, clients.LndClient);
            await OutboundPaymentRoundTrip(clients, clients.LndClient);
        }
        
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanOpenCloseChannelsWithLightningD()
        {
            var clients = _clients;
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await OutBoundChannelOpenRoundtrip(clients, clients.CLightningClient);
            await OutboundChannelCloseRoundtrip(clients, clients.ClightningLNClient);
            await InboundChannelOpenRoundtrip(clients, clients.CLightningClient);

            // ---- payment tests ----
            var resp = await clients.NRustLightningHttpClient.GetInvoiceAsync(new InvoiceCreationOption { Amount = LNMoney.MilliSatoshis(100L), Description = "foo bar" });
            var payResp = await clients.ClightningLNClient.Pay(resp.Invoice.ToString());
            Assert.Equal(PayResult.Ok,payResp.Result);
        }
    }
}