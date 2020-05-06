using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        public NodeInfo Get()
        {
            return new NodeInfo();
        }
    }
}
