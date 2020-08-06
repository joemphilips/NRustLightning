using System.Threading.Tasks;
using DockerComposeFixture;
using NRustLightning.Server.Tests.LNIntegrationTests;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public class ChannelReestablishTests : LNIntegrationTestsBase
    {
        public ChannelReestablishTests(DockerFixture dockerFixture, ITestOutputHelper output) : base (dockerFixture, output)
        {
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanResumeOutboundChannelsBy_channel_reestablish()
        {
            await _clients.ConnectAll();
            await _clients.PrepareFunds();
            await _clients.CreateEnoughTxToEstimateFee();
            await OutBoundChannelOpenRoundtrip(_clients, _clients.CLightningClient);
            await OutBoundChannelOpenRoundtrip(_clients, _clients.LndClient);
        }
    }
}