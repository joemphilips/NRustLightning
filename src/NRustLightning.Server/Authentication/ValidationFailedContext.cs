using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NRustLightning.Server.Authentication
{
    public class ValidationFailedContext : HandleRequestContext<LSATAuthenticationOptions>
    {
        protected ValidationFailedContext(HttpContext context, AuthenticationScheme scheme, LSATAuthenticationOptions options) : base(context, scheme, options)
        {
        }
    }
}