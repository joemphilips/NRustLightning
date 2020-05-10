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
        private readonly ITestOutputHelper output;
        private Clients Clients { get; }

        public StatelessDockerComposeTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            this.output = output;
            Clients = dockerFixture.StartLNTestFixture(output, nameof(StatelessDockerComposeTests));
        }

        [Fact]
        public async Task CanConnectNodes()
        {
            var blockchainInfo = await Clients.BitcoinRPCClient.GetBlockchainInfoAsync();
            Assert.NotNull(blockchainInfo);
            var lndInfo = await Clients.LndClient.SwaggerClient.GetInfoAsync();
            Assert.NotNull(lndInfo.Version);
            var clightningInfo = await Clients.CLightningClient.GetInfoAsync();
            Assert.NotEmpty(clightningInfo.Address);
            Assert.NotNull(clightningInfo.Network);
            Assert.NotNull(clightningInfo.Id);
            Assert.NotNull(clightningInfo.Version);
            var info = await Clients.HttpClient.GetInfoAsync();
            Assert.NotNull(info.ConnectionString);
            
            var ourInfo = await Clients.HttpClient.GetInfoAsync();
            NodeInfo.TryParse(ourInfo.ConnectionString.ToString(), out var nodeInfo);
            await ((ILightningClient)Clients.LndClient).ConnectTo(nodeInfo);
            var lndPeerInfo = await Clients.LndClient.SwaggerClient.ListPeersAsync();
            Assert.Single(lndPeerInfo.Peers);
        }
    }
}