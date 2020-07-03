using System.Buffers;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    // [Authorize(AuthenticationSchemes = "LSAT", Policy = "Readonly")]
    public class ChannelController : ControllerBase
    {
        private readonly IPeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly ILogger<ChannelController> _logger;
        private readonly MemoryPool<byte> _pool;

        public ChannelController(IPeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider, ILogger<ChannelController> logger)
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
            var peer = _peerManagerProvider.TryGetPeerManager(n);
            var details =  peer.ChannelManager.ListChannels(_pool);
            return new ChannelInfoResponse {Details = details};
        }

        [HttpPost]
        [Route("{cryptoCode}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ulong> OpenChannel(string cryptoCode, [FromBody] OpenChannelRequest o)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var peerMan = _peerManagerProvider.TryGetPeerManager(n);
            var ps = peerMan.GetPeerNodeIds(_pool);
            if (ps.All(p => p != o.TheirNetworkKey))
            {
                return BadRequest($"Unknown peer {o.TheirNetworkKey}. Make sure you are already connected to the peer.");
            }
            var chanMan = peerMan.ChannelManager;
            var maybeConfig = o.OverrideConfig;
            var userId = RandomUtils.GetUInt64();
            try
            {
                if (maybeConfig is null)
                    chanMan.CreateChannel(o.TheirNetworkKey, o.ChannelValueSatoshis, o.PushMSat,
                        userId);
                else
                {
                    var v = maybeConfig.Value;
                    chanMan.CreateChannel(o.TheirNetworkKey, o.ChannelValueSatoshis, o.PushMSat,
                        userId, in v);
                }
                peerMan.ProcessEvents();
            }
            catch (FFIException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(userId);
        }

        [HttpDelete]
        [Route("{cryptoCode}")]
        public ActionResult CloseChannel(string cryptoCode, [FromBody] CloseChannelRequest req)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var peerMan = _peerManagerProvider.GetPeerManager(n);
            var channels = peerMan.ChannelManager.ListChannels(_pool);
            var s = channels.FirstOrDefault(x => x.RemoteNetworkId == req.TheirNetworkKey);
            if (s is null)
            {
                return NotFound($"There is no opened channel against {req.TheirNetworkKey}");
            }
            peerMan.ChannelManager.CloseChannel(s.ChannelId);
            return Ok();
        }
    }
}