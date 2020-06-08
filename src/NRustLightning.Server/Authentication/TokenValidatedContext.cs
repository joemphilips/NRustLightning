using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NRustLightning.Server.Authentication
{
    public class TokenValidatedContext : ResultContext<LSATAuthenticationOptions>
    {
        public TokenValidatedContext(HttpContext context, AuthenticationScheme scheme, LSATAuthenticationOptions options) : base(context, scheme, options)
        {
        }
    }
}