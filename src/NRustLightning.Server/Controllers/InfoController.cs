using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public NodeInfo Get()
        {
            return new NodeInfo();
        }
    }
}
