using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.P2P;
using NRustLightning.Server.Services;
using NodeInfo = NRustLightning.Infrastructure.Models.Response.NodeInfo;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    // [Authorize(AuthenticationSchemes = "LSAT", Policy = "Readonly")]
    public class InfoController : ControllerBase
    {
        private readonly IKeysRepository keysRepository;
        private readonly P2PConnectionHandler _connectionHandler;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly RepositoryProvider _repositoryProvider;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly Config config;
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        public InfoController(IKeysRepository keysRepository, IOptions<Config> config,
            P2PConnectionHandler connectionHandler, NRustLightningNetworkProvider networkProvider,
            RepositoryProvider repositoryProvider, PeerManagerProvider peerManagerProvider)
        {
            this.keysRepository = keysRepository;
            _connectionHandler = connectionHandler;
            _networkProvider = networkProvider;
            _repositoryProvider = repositoryProvider;
            _peerManagerProvider = peerManagerProvider;
            this.config = config.Value;
        }
        
        [HttpGet]
        public async Task<NodeInfo> Get()
        {
            var nodeIds = _connectionHandler.GetPeerNodeIds();
            return new NodeInfo
            {
                NumConnected = nodeIds.Length,
                NodeIds = nodeIds.Select(x => x.ToHex()).ToList(),
                ConnectionString = new PeerConnectionString(keysRepository.GetNodeId(), config.P2PExternalIp)
            };
        }

        [HttpGet("/graph")]
        public NetworkGraph DescribeGraph()
        {
            var n = _networkProvider.GetByCryptoCode(config.ChainConfiguration[0].CryptoCode);
            var peerMan = _peerManagerProvider.GetPeerManager(n);
            return peerMan.GetNetworkGraph(_pool);
        }
    }
}
