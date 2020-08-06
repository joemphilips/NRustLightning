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
using OpenChannelRequest = BTCPayServer.Lightning.OpenChannelRequest;

namespace NRustLightning.Server.Tests.LNIntegrationTests
{
    public abstract class LNIntegrationTestsBase : IClassFixture<DockerFixture>
    {
        private readonly DockerFixture _dockerFixture;
        private readonly ITestOutputHelper _output;
        protected Clients _clients;
        
        protected LNIntegrationTestsBase (DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;
            _output = output;
            _clients = _dockerFixture.StartLNTestFixtureAsync(_output, this.GetType().Name).GetAwaiter().GetResult();
        }

        protected async Task OutBoundChannelOpenRoundtrip(Clients clients, ILightningClient remoteClient)
        {
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            var info = await remoteClient.GetInfo();
            Assert.Single(info.NodeInfoList);
            var theirNodeKey = info.NodeInfoList.FirstOrDefault()?.NodeId;
            Assert.NotNull(theirNodeKey);
            await clients.NRustLightningHttpClient.OpenChannelAsync(new Infrastructure.Models.Request.OpenChannelRequest
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
        internal async Task OutboundChannelCloseRoundtrip(Clients clients, ILightningClient remoteClient)
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
        internal async Task InboundChannelOpenRoundtrip(Clients clients, ILightningClient remoteClient)
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

        protected async Task InboundPaymentRoundTrip(Clients clients, ILightningClient lnClient)
        {
            
            var resp = await clients.NRustLightningHttpClient.GetInvoiceAsync(new InvoiceCreationOption { Amount = LNMoney.MilliSatoshis(100L), Description = "foo bar" });
            var ts = 10;
            await Support.Utils.Retry(3, TimeSpan.FromSeconds(ts), async () =>
            {
                var payResp = await lnClient.Pay(resp.Invoice.ToString());
                return payResp.Result == PayResult.Ok;
            });
        }

        protected async Task OutboundPaymentRoundTrip(Clients clients, ILightningClient lnClient)
        {
            await Task.Delay(10000); // not sure why we need this.
            await Support.Utils.Retry(3, TimeSpan.FromSeconds(12), async () =>
            {
                var invoice = await lnClient.CreateInvoice(10000, "CanCreateInvoice", TimeSpan.FromMinutes(5));
                try
                {
                    await clients.NRustLightningHttpClient.PayToInvoiceAsync(invoice.BOLT11);
                }
                catch (HttpRequestException)
                {
                    return false;
                }

                return true;
            });
        }

    }
}