using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
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

        [Fact]
        public async Task CanOpenCloseChannels()
        {
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(CanOpenCloseChannels));
            var walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotNull(walletInfo.DerivationStrategy);
            Assert.Equal(0, walletInfo.BalanceSatoshis);
            Assert.DoesNotContain("legacy", walletInfo.DerivationStrategy.ToString());
            await clients.ConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            
            // check wallet info and nbxplorer info is synchronized.
            walletInfo = await clients.NRustLightningHttpClient.GetWalletInfoAsync();
            Assert.NotEqual(0, walletInfo.BalanceSatoshis);
            var explorerInfo = await clients.NBXClient.GetBalanceAsync(walletInfo.DerivationStrategy);
            Assert.Equal(NBitcoin.Money.Satoshis(walletInfo.BalanceSatoshis), explorerInfo.Total);

            var lnd = await clients.LndLNClient.GetInfo();
            Assert.Single(lnd.NodeInfoList);
            var i = lnd.NodeInfoList.FirstOrDefault()?.NodeId;
            Assert.NotNull(i);
            await clients.NRustLightningHttpClient.OpenChannel(new OpenChannelRequest { TheirNetworkKey = i, ChannelValueSatoshis = 1000000, PushMSat = 100000});
            var addr = await clients.NRustLightningHttpClient.GetNewDepositAddressAsync();
            await clients.BitcoinRPCClient.GenerateToAddressAsync(10, addr.Address);
            await Task.Delay(5000);

            var lndChannelInfo = await clients.LndLNClient.ListChannels();
            Assert.Single(lndChannelInfo);
        }
        
        [Fact]
        public async Task CanPayToOtherNodes() {}
    }
}