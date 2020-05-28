using System.Buffers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class ChannelController : ControllerBase
    {
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly MemoryPool<byte> _pool;

        public ChannelController(PeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider)
        {
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
            _pool = MemoryPool<byte>.Shared;
        }
        
        [HttpGet]
        [Route("{cryptoCode}")]
        public ChannelInfoResponse Get(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var peer = _peerManagerProvider.GetPeerManager(n);
            var details =  peer.ChannelManager.ListChannels(_pool);
            return new ChannelInfoResponse {Details = details};
        }
    }
}