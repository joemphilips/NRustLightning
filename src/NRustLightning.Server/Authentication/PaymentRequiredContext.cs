using DotNetLightning.Payment;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NRustLightning.Server.Authentication
{
    public class PaymentRequiredContext : PropertiesContext<LSATAuthenticationOptions>
    {
        public PaymentRequiredContext(HttpContext context, AuthenticationScheme scheme, LSATAuthenticationOptions options, AuthenticationProperties properties) : base(context, scheme, options, properties)
        {
        }
        
        /// <summary>
        /// The invoice we are going to send to the client.
        /// </summary>
        public PaymentRequest? Bolt11Invoice { get; set; }
        
        /// <summary>
        /// gets or sets the "error" value returned to the caller as part of the WWW-Authenticate header.
        /// This property may be null when <see cref="LSATAuthenticationOptions.IncludeErrorDetails"/> is false.
        /// </summary>
        public string? Error { get; set; }
        
        /// <summary>
        /// Gets or sets the "error_description" value returned to the caller as part of the
        /// WWW-Authenticate header. This property may be null when
        /// <see cref="LSATAuthenticationOptions.IncludeErrorDetails"/> is set to `false`
        /// </summary>
        public string? ErrorDescription { get; set; }
        
    }
}