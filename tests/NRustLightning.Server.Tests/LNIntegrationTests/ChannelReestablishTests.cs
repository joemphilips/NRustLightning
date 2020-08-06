using System.Threading.Tasks;
using DockerComposeFixture;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public class ChannelReestablishTests : IClassFixture<DockerFixture>
    {
        private readonly DockerFixture _dockerFixture;
        private readonly ITestOutputHelper _output;
        private Clients _clients;

        public ChannelReestablishTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;
            _output = output;
            _clients = _dockerFixture.StartLNTestFixtureAsync(_output, nameof(CanResumeChannelsBy_channel_reestablish)).GetAwaiter().GetResult();
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanResumeChannelsBy_channel_reestablish()
        {
            await _clients.ConnectAll();
            await _clients.PrepareFunds();
            await _clients.CreateEnoughTxToEstimateFee();
            await StatelessDockerComposeTests.OutBoundChannelOpenRoundtrip(_clients, _clients.CLightningClient);
            await StatelessDockerComposeTests.OutBoundChannelOpenRoundtrip(_clients, _clients.LndClient);
        }
    }
}