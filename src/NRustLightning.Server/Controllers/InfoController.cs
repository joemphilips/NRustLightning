using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.P2P;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Services;

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
        private readonly Config config;

        public InfoController(IKeysRepository keysRepository, IOptions<Config> config, P2PConnectionHandler connectionHandler, NRustLightningNetworkProvider networkProvider, RepositoryProvider repositoryProvider)
        {
            this.keysRepository = keysRepository;
            _connectionHandler = connectionHandler;
            _networkProvider = networkProvider;
            _repositoryProvider = repositoryProvider;
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

    }
}
