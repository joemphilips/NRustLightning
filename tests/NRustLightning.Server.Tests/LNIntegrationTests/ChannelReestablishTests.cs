using System;
using System.Net.Http;
using System.Threading.Tasks;
using DockerComposeFixture;
using NRustLightning.Infrastructure.Models.Response;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests.LNIntegrationTests
{
    public class ChannelReestablishTests : IClassFixture<LNIntegrationTestsBase>
    {
        private readonly LNIntegrationTestsBase _dockerFixture;
        private readonly ITestOutputHelper _output;

        public ChannelReestablishTests(LNIntegrationTestsBase dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;
            _output = output;
        }
        
        [Fact]
        [Trait("IntegrationTest", "LNFixture")]
        public async Task CanResumeOutboundChannelsBy_channel_reestablish()
        {
            var clients = _dockerFixture.Clients;
            await clients.OutBoundConnectAll();
            await clients.PrepareFunds();
            await clients.CreateEnoughTxToEstimateFee();
            await _dockerFixture.OutBoundChannelOpenRoundtrip(clients, clients.CLightningClient);
            await _dockerFixture.OutBoundChannelOpenRoundtrip(clients, clients.LndClient);
            await _dockerFixture.Restart("nrustlightning");
            await Support.Utils.Retry(15, TimeSpan.FromSeconds(2.5), async () =>
            {
                ChannelInfoResponse resp;
                try
                {
                    resp = await clients.NRustLightningHttpClient.GetChannelDetailsAsync();
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