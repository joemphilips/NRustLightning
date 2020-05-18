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
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;

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
            var clients = await dockerFixture.StartLNTestFixtureAsync(output, nameof(StatelessDockerComposeTests));
            var blockchainInfo = await clients.BitcoinRPCClient.GetBlockchainInfoAsync();
            Assert.NotNull(blockchainInfo);
            var lndInfo = await clients.LndLNClient.GetInfo();
            Assert.NotEmpty(lndInfo.NodeInfoList);
            var clightningInfo = await clients.ClightningLNClient.GetInfo();
            Assert.NotNull(clightningInfo);
            var info = await clients.NRustLightningHttpClient.GetInfoAsync();
            Assert.NotNull(info.ConnectionString);
            
            var ourInfo = await clients.NRustLightningHttpClient.GetInfoAsync();
            NodeInfo.TryParse(ourInfo.ConnectionString.ToString(), out var nodeInfo);
            await clients.LndLNClient.ConnectTo(nodeInfo);

            await Task.Delay(200);
            
            var lndPeerInfo = await clients.LndClient.SwaggerClient.ListPeersAsync();
            Assert.Single(lndPeerInfo.Peers);
            
            await clients.NRustLightningHttpClient.ConnectAsync(clightningInfo.NodeInfoList.FirstOrDefault().ToConnectionString());

            await Task.Delay(200);
            var clightningPeerInfo = await clients.CLightningClient.ListPeersAsync();
            Assert.Single(clightningPeerInfo);
            Assert.Equal(clightningPeerInfo.First().Id, ourInfo.ConnectionString.NodeId.ToHex());
        }
    }
}