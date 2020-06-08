using System.Collections.Generic;
using System.Threading.Tasks;
using Macaroons;

namespace NRustLightning.Server.Authentication.MacaroonMinter
{
    public interface IServiceLimiter
    {
        Task<IEnumerable<Caveat>> ServiceCapabilities();
        Task<IEnumerable<Caveat>> ServiceConstraints();
    }
}