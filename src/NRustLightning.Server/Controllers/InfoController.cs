using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly IKeysRepository keysRepository;
        private readonly Config config;

        public InfoController(IKeysRepository keysRepository, IOptions<Config> config)
        {
            this.keysRepository = keysRepository;
            this.config = config.Value;
        }
        
        [HttpGet]
        public async Task<NodeInfo> Get()
        {
            return new NodeInfo
            {
                ConnectionString = new PeerConnectionString(keysRepository.GetNodeId(), config.P2PExternalIp)
            };
        }
    }
}
