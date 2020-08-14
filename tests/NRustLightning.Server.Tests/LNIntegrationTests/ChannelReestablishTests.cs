using System;
using System.Net.Http;
using System.Threading.Tasks;
using DockerComposeFixture;
using NRustLightning.Infrastructure.Models.Response;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests.LNIntegrationTests
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
            await _clients.OutBoundConnectAll();
            await _clients.PrepareFunds();
            await _clients.CreateEnoughTxToEstimateFee();
            await OutBoundChannelOpenRoundtrip(_clients, _clients.CLightningClient);
            await OutBoundChannelOpenRoundtrip(_clients, _clients.LndClient);
            await _dockerFixture.Restart("nrustlightning");
            await Support.Utils.Retry(15, TimeSpan.FromSeconds(2.5), async () =>
            {
                ChannelInfoResponse resp;
                try
                {
                    resp = await _clients.NRustLightningHttpClient.GetChannelDetailsAsync();
                }
                catch(HttpRequestException)
                {
                    return false;
                }

                return resp.Details.Length == 2;
            });
        }
    }
}