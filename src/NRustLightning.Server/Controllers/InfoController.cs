using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.P2P;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly IKeysRepository keysRepository;
        private readonly P2PConnectionHandler _connectionHandler;
        private readonly Config config;

        public InfoController(IKeysRepository keysRepository, IOptions<Config> config, P2PConnectionHandler connectionHandler)
        {
            this.keysRepository = keysRepository;
            _connectionHandler = connectionHandler;
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
