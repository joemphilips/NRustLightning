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
            await Clients.BitcoinRPCClient.GetBlockchainInfoAsync();
            await Clients.LndClient.SwaggerClient.GetInfoAsync();
            await Clients.CLightningClient.GetInfoAsync();
            await Clients.Client.GetInfoAsync();
        }
    }
}