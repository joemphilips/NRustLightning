using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;
using OpenChannelRequest = NRustLightning.Server.Models.Request.OpenChannelRequest;

namespace NRustLightning.Server.Tests
{
    public class StatelessDockerComposeTests : IClassFixture<DockerFixture>
    {
        private readonly DockerFixture dockerFixture;
        private readonly ITestOutputHelper output;
        public StatelessDockerComposeTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            this.dockerFixture = dockerFixture;
            this.output = output;
        }

        [Fact]
        public async Task CanConnectNodes()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanConnectNodes));
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
            Assert.Null(lndPeerInfo.Peers);
            info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.Equal(0, info.NumConnected);
            
            // Can disconnect from inside for inbound connection.
            // TODO: after handling events from rl, we must hold PubKey => connection in ConnectionHandler.
            // And then we can disconnect from inside.
        }

        async Task OutBoundChannelOpenRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            var info = await remoteClient.GetInfo();
            Assert.Single(info.NodeInfoList);
            var theirNodeKey = info.NodeInfoList.FirstOrDefault()?.NodeId;
            Assert.NotNull(theirNodeKey);
            await clients.NRustLightningHttpClient.OpenChannelAsync(new OpenChannelRequest
                {TheirNetworkKey = theirNodeKey, ChannelValueSatoshis = 1000000, PushMSat = 100000});

            // wait until nbx detects unconfirmed funding tx.
            await Support.Utils.Retry(30, TimeSpan.FromSeconds(1.5), async () =>
            {
                var e = (await clients.NBXClient.GetTransactionsAsync(walletInfo.DerivationStrategy));
                return e.UnconfirmedTransactions.Transactions.Count > 0;
            });

            var addr = await clients.NRustLightningHttpClient.GetNewDepositAddressAsync();
            await clients.BitcoinRPCClient.GenerateToAddressAsync(10, addr.Address);
            await Support.Utils.Retry(20, TimeSpan.FromSeconds(1.2), async () =>
            {
                // CLightning client throws null reference exception when it is used as ILightningClient.
                // So we do dirty fallback here.
                if (remoteClient is CLightningClient cli)
                {
                    var channelList = await cli.ListChannelsAsync();
                    Assert.NotNull(channelList);
                    return channelList.Length > 0;
                }
                else
                {
                    var channelList = await remoteClient.ListChannels(CancellationToken.None);
                    Assert.NotNull(channelList);
                    return channelList.Length > 0;
                }
            });
        }
        async Task OutboundChannelCloseRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var info = await remoteClient.GetInfo();
            var id = info.NodeInfoList.FirstOrDefault()?.NodeId;
            await clients.NRustLightningHttpClient.CloseChannelAsync(id);
            await Support.Utils.Retry(9, TimeSpan.FromSeconds(2), async () =>
            {
                var add = await clients.BitcoinRPCClient.GetNewAddressAsync();
                await clients.BitcoinRPCClient.GenerateToAddressAsync(1, add);
                var aliveChannels = (await clients.NRustLightningHttpClient.GetChannelDetailsAsync()).Details.Where(x => x.RemoteNetworkId == id && x.IsLive);
                return !(await remoteClient.ListChannels()).Any(x => x.IsActive)
                       && !aliveChannels.Any();
            });
        }
        async Task InboundChannelOpenRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var nrlInfo = await clients.NRustLightningHttpClient.GetInfoAsync();
            var feeRate = (await clients.NBXClient.GetFeeRateAsync(3)).FeeRate;
            await remoteClient.OpenChannel(new BTCPayServer.Lightning.OpenChannelRequest()
                {NodeInfo = nrlInfo.ConnectionString.ToNodeInfo(), ChannelAmount = 100000, FeeRate = feeRate});
            // wait until bitcoind detects unconfirmed funding tx on mempool.
            await Support.Utils.Retry(12, TimeSpan.FromSeconds(1.5), async () =>
            {
                var m = await clients.BitcoinRPCClient.GetRawMempoolAsync();
                return m.Length > 0;
            });

            var addr = await clients.NRustLightningHttpClient.GetNewDepositAddressAsync();
            await clients.BitcoinRPCClient.GenerateToAddressAsync(10, addr.Address);
            await Support.Utils.Retry(20, TimeSpan.FromSeconds(1.2), async () =>
            {
                // CLightning client throws null reference exception when it is used as ILightningClient.
                // So we do dirty fallback here.
                if (remoteClient is CLightningClient cli)
                {
                    var channelList = await cli.ListChannelsAsync();
                    Assert.NotNull(channelList);
                    return channelList.Length > 0;
                }
                else
                {
                    var channelList = await remoteClient.ListChannels(CancellationToken.None);
                    Assert.NotNull(channelList);
                    return channelList.Length > 0;
                }
            });
        }
        [Fact]
        public async Task CanOpenCloseChannels()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanOpenCloseChannels));
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotNull(walletInfo.DerivationStrategy);
            Assert.Equal(0, walletInfo.OnChainBalanceSatoshis);
            Assert.DoesNotContain("legacy", walletInfo.DerivationStrategy.ToString());
            await clients.ConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            // check wallet info and nbxplorer info is synchronized.
            walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotEqual(0, walletInfo.OnChainBalanceSatoshis);
            var explorerInfo = await clients.NBXClient.GetBalanceAsync(walletInfo.DerivationStrategy);
            Assert.Equal(NBitcoin.Money.Satoshis(walletInfo.OnChainBalanceSatoshis), explorerInfo.Total);

            await OutBoundChannelOpenRoundtrip(clients, clients.LndLNClient);
            await OutBoundChannelOpenRoundtrip(clients, clients.CLightningClient);
            await OutboundChannelCloseRoundtrip(clients, clients.LndLNClient);
            await OutboundChannelCloseRoundtrip(clients, clients.ClightningLNClient);
            await InboundChannelOpenRoundtrip(clients, clients.LndClient);
            
            
            // Inbound chanel opening is a bit flaky for c-lightning. ignoring for now.
            // await InboundChannelOpenRoundtrip(clients, clients.CLightningClient);

            // ---- payment tests ----
            var resp = await clients.NRustLightningHttpClient.GetInvoiceAsync(new InvoiceCreationOption() { Amount = LNMoney.MilliSatoshis(100L), Description = "foo bar" });
            var payResp = await clients.LndLNClient.Pay(resp.Invoice.ToString());
            Assert.Equal(PayResult.Ok,payResp.Result);
        }
        
        [Fact]
        public async Task CanPayToOtherNodes() {}
    }
}