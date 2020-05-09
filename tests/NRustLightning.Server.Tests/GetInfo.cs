using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public class GetInfo : IClassFixture<DockerFixture>
    {
        private readonly ITestOutputHelper output;
        private Clients Clients { get; }

        public GetInfo(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            this.output = output;
            Clients = dockerFixture.StartLNTestFixture(output, nameof(GetInfo));
        }

        [Fact]
        public async Task CanPingClients()
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
            await Clients.Client.GetInfoAsync();
        }
    }
}