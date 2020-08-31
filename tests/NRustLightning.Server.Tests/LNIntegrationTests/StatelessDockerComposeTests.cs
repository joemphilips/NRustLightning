using System;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using NBitcoin;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests.LNIntegrationTests
{
    // [CollectionDefinition("Non-Parallel", DisableParallelization = true)]
    // class NonParallelCollectionDefinition {}
    // [Collection("Non-Parallel")]
    public class StatelessDockerComposeTests : IClassFixture<LNIntegrationTestsBase>
    {
        private LNIntegrationTestsBase _fixture;
        private Clients _clients;
        private ITestOutputHelper _output;

        public StatelessDockerComposeTests(LNIntegrationTestsBase dockerFixture, ITestOutputHelper output)
        {
            _output = output;
            _fixture = dockerFixture;
            _clients = dockerFixture.Clients;
        }

        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanOpenCloseChannelsWithLND()
        {
            var clients = _clients;
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotNull(walletInfo.DerivationStrategy);
            Assert.DoesNotContain("legacy", walletInfo.DerivationStrategy.ToString());

            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await _fixture.OutBoundChannelOpenRoundtrip(clients, clients.LndLNClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, clients.LndLNClient);
            await  _fixture.InboundChannelOpenRoundtrip(clients, clients.LndClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, clients.LndClient);
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanSendInboundPaymentFromLnd()
        {
            var clients = _clients;
            
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await  _fixture.InboundChannelOpenRoundtrip(clients, clients.LndClient);
            // ---- payment tests ----
            // await  _fixture.InboundPaymentRoundTrip(clients, clients.LndClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, clients.LndClient);
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanSendOutboundPaymentToLnd()
        {
            var clients = _clients;
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await  _fixture.OutBoundChannelOpenRoundtrip(clients, clients.LndClient);
            await  _fixture.OutboundPaymentRoundTrip(clients, clients.LndClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, clients.LndClient);
        }
        
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanOpenCloseChannelsWithLightningD()
        {
            var clients = _clients;
            
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await  _fixture.OutBoundChannelOpenRoundtrip(clients, clients.CLightningClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, clients.ClightningLNClient);
            await  _fixture.InboundChannelOpenRoundtrip(clients, clients.CLightningClient);
            await  _fixture.OutboundChannelCloseRoundtrip(clients, _clients.CLightningClient);
        }

        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanWithdrawFunds()
        {
            await _clients.OutBoundConnectAll();
            await _clients.PrepareFunds();
            await _clients.CreateEnoughTxToEstimateFee();

            await _fixture.OutBoundChannelOpenRoundtrip(_clients, _clients.LndLNClient);
            await  _fixture.OutboundChannelCloseRoundtrip(_clients, _clients.LndLNClient);
            await  _fixture.InboundChannelOpenRoundtrip(_clients, _clients.LndClient);
            await  _fixture.OutboundChannelCloseRoundtrip(_clients, _clients.LndClient);

            var addr = new Key().PubKey.WitHash.GetAddress(_clients.NBXClient.Network.NBitcoinNetwork);
            var txHash = await  _clients.NRustLightningHttpClient.WithdrawFundsAsync(Money.Coins(3m), addr);
            
            var info = await _clients.BitcoinRPCClient.GetRawTransactionAsync(txHash);
            Assert.NotNull(info);
        }
    }
}