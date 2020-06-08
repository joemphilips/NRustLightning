using DotNetLightning.Utils;
using Macaroons;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NRustLightning.Server.Authentication
{
    public class MessageReceivedContext: HandleRequestContext<LSATAuthenticationOptions>
    {
        public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, LSATAuthenticationOptions options) : base(context, scheme, options)
        {
        }
        /// <summary>
        /// Payment preimage from a client. This will give the app an opportunity to retrieve the preimage from an alternative location.
        /// </summary>
        public Primitives.PaymentPreimage? Preimage { get; set; }
        /// <summary>
        /// Macaroon from a client. This will give the app an opportunity to retrieve the macaroon from an alternative location.
        /// </summary>
        public Macaroon? Macaroon { get; set; }
    }
}