using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;
using OpenChannelRequest = NRustLightning.Infrastructure.Models.Request.OpenChannelRequest;

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
            Assert.Empty(lndPeerInfo.Peers);
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
            var localId = (await clients.NRustLightningHttpClient.GetInfoAsync()).ConnectionString.NodeId;
            await Support.Utils.Retry(20, TimeSpan.FromSeconds(1.2), async () =>
            {
                var maybeLocalChannel = (await clients.NRustLightningHttpClient.GetChannelDetailsAsync()).Details.FirstOrDefault(cd => cd.RemoteNetworkId.Equals(theirNodeKey) && cd.IsLive);
                if (maybeLocalChannel is null)
                    return false;
                // CLightning client throws null reference exception when it is used as ILightningClient.
                // So we do dirty fallback here.
                if (remoteClient is CLightningClient cli)
                {
                    var maybeRemoteChannel = (await cli.ListChannelsAsync()).FirstOrDefault(c => new PubKey(c.Destination).Equals(localId));
                    return maybeRemoteChannel != null;
                }
                else
                {
                    var maybeRemoteChannel = (await remoteClient.ListChannels()).FirstOrDefault(c => c.RemoteNode.Equals(localId));
                    return maybeRemoteChannel != null;
                }
            });
        }
        async Task OutboundChannelCloseRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var info = await remoteClient.GetInfo();
            var localInfo = (await clients.NRustLightningHttpClient.GetInfoAsync());
            var theirNodeKey = info.NodeInfoList.FirstOrDefault(x => !x.NodeId.Equals(localInfo.ConnectionString.NodeId))?.NodeId ?? Infrastructure.Utils.Utils.Fail<PubKey>("Channel Seems already closed");
            await clients.NRustLightningHttpClient.CloseChannelAsync(theirNodeKey);
            await Support.Utils.Retry(20, TimeSpan.FromSeconds(2), async () =>
            {
                var add = await clients.BitcoinRPCClient.GetNewAddressAsync();
                await clients.BitcoinRPCClient.GenerateToAddressAsync(2, add);
                
                var maybeLocalChannel = (await clients.NRustLightningHttpClient.GetChannelDetailsAsync()).Details.FirstOrDefault(cd => cd.RemoteNetworkId.Equals(theirNodeKey));
                if (maybeLocalChannel != null && maybeLocalChannel.IsLive)
                    return false;
                var maybeRemoteChannel = (await remoteClient.ListChannels()).FirstOrDefault(c => c.RemoteNode.Equals(localInfo.ConnectionString.NodeId));
                return (maybeRemoteChannel is null || !maybeRemoteChannel.IsActive);
            });
        }
        async Task InboundChannelOpenRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var localInfo = await clients.NRustLightningHttpClient.GetInfoAsync();
            var feeRate = (await clients.NBXClient.GetFeeRateAsync(3)).FeeRate;
            await remoteClient.OpenChannel(new BTCPayServer.Lightning.OpenChannelRequest()
                {NodeInfo = localInfo.ConnectionString.ToNodeInfo(), ChannelAmount = 100000, FeeRate = feeRate});
            // wait until bitcoind detects unconfirmed funding tx on mempool.
            await Support.Utils.Retry(12, TimeSpan.FromSeconds(1.5), async () =>
            {
                var m = await clients.BitcoinRPCClient.GetRawMempoolAsync();
                return m.Length > 0;
            });


            var remoteInfo = await remoteClient.GetInfo();
            var theirNodeKey = remoteInfo.NodeInfoList.FirstOrDefault(x => !x.NodeId.Equals(localInfo.ConnectionString.NodeId))?.NodeId ?? Infrastructure.Utils.Utils.Fail<PubKey>("Channel Seems already closed");
            var addr = await clients.NRustLightningHttpClient.GetNewDepositAddressAsync();
            await clients.BitcoinRPCClient.GenerateToAddressAsync(10, addr.Address);
            await Support.Utils.Retry(20, TimeSpan.FromSeconds(2.0), async () =>
            {
                await clients.BitcoinRPCClient.GenerateToAddressAsync(2, addr.Address);
                var maybeLocalChannel = (await clients.NRustLightningHttpClient.GetChannelDetailsAsync()).Details.FirstOrDefault(c => c.RemoteNetworkId.Equals(theirNodeKey));
                if (maybeLocalChannel is null || !maybeLocalChannel.IsLive)
                    return false;
                // CLightning client throws null reference exception when it is used as ILightningClient.
                // So we do dirty fallback here.
                if (remoteClient is CLightningClient cli)
                {
                    var channelList = await cli.ListChannelsAsync();
                    Assert.NotNull(channelList);
                    return channelList.Length > 0 && channelList.First().Active && new PubKey(channelList.First().Destination).Equals(localInfo.ConnectionString.NodeId);
                }
                else
                {
                    var channelList = await remoteClient.ListChannels(CancellationToken.None);
                    Assert.NotNull(channelList);
                    return (channelList.Length > 0 && channelList.First().IsActive && channelList.First().RemoteNode.Equals(localInfo.ConnectionString.NodeId));
                }
            });
        }

        private async Task InboundPaymentRoundTrip(Clients clients, ILightningClient lnClient)
        {
            
            var resp = await clients.NRustLightningHttpClient.GetInvoiceAsync(new InvoiceCreationOption { Amount = LNMoney.MilliSatoshis(100L), Description = "foo bar" });
            var ts = 10;
            await Support.Utils.Retry(3, TimeSpan.FromSeconds(ts), async () =>
            {
                var payResp = await lnClient.Pay(resp.Invoice.ToString());
                if (payResp.Result != PayResult.Ok)
                {
                    output.WriteLine($"Failed inbound payment {payResp.Result}... retrying in {ts} seconds.");
                }
                return payResp.Result == PayResult.Ok;
            });
        }

        private async Task OutboundPaymentRoundTrip(Clients clients, ILightningClient lnClient)
        {
            await Task.Delay(10000); // not sure why we need this.
            await Support.Utils.Retry(3, TimeSpan.FromSeconds(12), async () =>
            {
                var invoice = await lnClient.CreateInvoice(10000, "CanCreateInvoice", TimeSpan.FromMinutes(5));
                try
                {
                    await clients.NRustLightningHttpClient.PayToInvoiceAsync(invoice.BOLT11);
                }
                catch (HttpRequestException ex)
                {
                    output.WriteLine($"Failed outbound payment {ex.Message}. retrying....");
                    return false;
                }

                return true;
            });
        }
        
        [Fact]
        public async Task CanOpenCloseChannelsWithLND()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanOpenCloseChannelsWithLND));
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotNull(walletInfo.DerivationStrategy);
            Assert.Equal(0, walletInfo.OnChainBalanceSatoshis);
            Assert.DoesNotContain("legacy", walletInfo.DerivationStrategy.ToString());
            await clients.ConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await OutBoundChannelOpenRoundtrip(clients, clients.LndLNClient);
            await OutboundChannelCloseRoundtrip(clients, clients.LndLNClient);
            await InboundChannelOpenRoundtrip(clients, clients.LndClient);
        }
        
        [Fact]
        public async Task CanSendInboundPaymentFromLnd()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanSendInboundPaymentFromLnd));
            
            await clients.ConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await InboundChannelOpenRoundtrip(clients, clients.LndClient);
            // ---- payment tests ----
            await InboundPaymentRoundTrip(clients, clients.LndClient);
        }
        
        [Fact]
        public async Task CanSendOutboundPaymentToLnd()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanSendOutboundPaymentToLnd));
            await clients.ConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            await OutBoundChannelOpenRoundtrip(clients, clients.LndClient);
            await OutboundPaymentRoundTrip(clients, clients.LndClient);
        }
        
        
        [Fact]
        public async Task CanOpenCloseChannelsWithLightningD()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanOpenCloseChannelsWithLightningD));
            await clients.ConnectAll();
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