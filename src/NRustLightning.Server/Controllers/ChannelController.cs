using System.Buffers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "LSAT")]
    public class ChannelController : ControllerBase
    {
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly ILogger<ChannelController> _logger;
        private readonly MemoryPool<byte> _pool;

        public ChannelController(PeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider, ILogger<ChannelController> logger)
        {
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
            _logger = logger;
            _pool = MemoryPool<byte>.Shared;
        }
        
        [HttpGet]
        [Route("{cryptoCode}")]
        public ChannelInfoResponse Get(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var peer = _peerManagerProvider.GetPeerManager(n);
            var details =  peer.ChannelManager.ListChannels(_pool);
            return new ChannelInfoResponse {Details = details};
        }

        [HttpPost]
        [Route("{cryptoCode}")]
        public ulong OpenChannel(string cryptoCode, [FromBody] OpenChannelRequest o)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var chanMan = _peerManagerProvider.GetPeerManager(n).ChannelManager;
            var maybeConfig = o.OverrideConfig;
            var userId = RandomUtils.GetUInt64();
            if (maybeConfig is null)
                chanMan.CreateChannel(o.TheirNetworkKey, o.ChannelValueSatoshis, o.PushMSat,
                    userId);
            else
            {
                var v = maybeConfig.Value;
                chanMan.CreateChannel(o.TheirNetworkKey, o.ChannelValueSatoshis, o.PushMSat,
                    userId, in v);
            }

            return userId;
        }
    }
}