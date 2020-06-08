using System.Collections.Generic;
using DotNetLightning.Utils;
using Microsoft.AspNetCore.Authentication;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Server.Authentication
{
    public class LSATAuthenticationOptions : AuthenticationSchemeOptions
    {
        public LSATAuthenticationOptions() : base() {}
        /// <summary>
        /// If true, we will return "error" value to the caller as part of the WWW-Authenticate header.
        /// </summary>
        public bool IncludeErrorDetails { get; set; } = true;
        
        /// <summary>
        /// a list of custom caveats to restrict the client's capability.
        /// By default it will only have a 'service={ServiceName}:{ServiceTier}'
        /// </summary>
        public IList<string> MacaroonCaveats { get; set; } = new List<string>();

        /// <summary>
        /// Service name by which a user refers to this server.
        /// </summary>
        public string ServiceName { get; set; } = "unnamed-service";

        /// <summary>
        /// Macaroon needs to know the public name of your server, so you must set it here.
        /// </summary>
        public string ServiceLocation { get; set; } = "https://localhost";

        public LNMoney InvoiceAmount = LNMoney.One;
        
        /// <summary>
        /// When you update capabilities or constraints for this service, you must increment this number.
        /// In this way users can detect the upgrade and seamlessly upgrade the stale macaroon by revoking it
        /// and mining a new one. see https://github.com/lightninglabs/LSAT/blob/master/macaroons.md#target-services
        /// for the detail.
        /// </summary>
        public int ServiceTier { get; set; } = 0;
        
        /// <summary>
        /// The object provided by the application to process events raised by the bearer authentication handler.
        /// The application may implement the interface fully, or it may create an instance of LSATAuthenticationEvents
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new LSATAuthenticationEvents Events
        {
            get => (LSATAuthenticationEvents) base.Events;
            set => base.Events = value;
        }
    }
}