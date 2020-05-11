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
            var lndInfo = await clients.LndClient.SwaggerClient.GetInfoAsync();
            Assert.NotNull(lndInfo.Version);
            var clightningInfo = await clients.CLightningClient.GetInfoAsync();
            Assert.NotEmpty(clightningInfo.Address);
            Assert.NotNull(clightningInfo.Network);
            Assert.NotNull(clightningInfo.Id);
            Assert.NotNull(clightningInfo.Version);
            var info = await clients.HttpClient.GetInfoAsync();
            Assert.NotNull(info.ConnectionString);
            
            var ourInfo = await clients.HttpClient.GetInfoAsync();
            NodeInfo.TryParse(ourInfo.ConnectionString.ToString(), out var nodeInfo);
            await ((ILightningClient)clients.LndClient).ConnectTo(nodeInfo);
            var lndPeerInfo = await clients.LndClient.SwaggerClient.ListPeersAsync();
            Assert.Single(lndPeerInfo.Peers);
        }
    }
}