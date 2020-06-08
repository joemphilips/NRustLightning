using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Macaroons;

namespace NRustLightning.Server.Authentication.MacaroonMinter
{
    /// <summary>
    /// Creates and verifies LSAT-compliant macaroon.
    /// </summary>
    public class MacaroonMinter
    {
        public ILSATInvoiceProvider InvoiceProvider { get; }
        public IMacaroonSecretRepository MacaroonSecretRepository { get; }
        public IServiceLimiter ServiceLimiter { get; }

        public MacaroonMinter(ILSATInvoiceProvider invoiceProvider, IMacaroonSecretRepository macaroonSecretRepository, IServiceLimiter serviceLimiter)
        {
            InvoiceProvider = invoiceProvider ?? throw new ArgumentNullException(nameof(invoiceProvider));
            MacaroonSecretRepository = macaroonSecretRepository ?? throw new ArgumentNullException(nameof(macaroonSecretRepository));
            ServiceLimiter = serviceLimiter ?? throw new ArgumentNullException(nameof(serviceLimiter));
        }

        private LNMoney GetMaximumPrice(IEnumerable<Service> services)
        {
            return
                services
                    .Where(x => x.Price != null)
                    .Select(x => x.Price.Value)
                    .Max(x => LNMoney.MilliSatoshis(x));
        }

        private Caveat GetNewServiceCaveat(IList<Service> services)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<Caveat>> GetCaveatsForServices(IList<Service> services)
        {
            var capabilities = GetNewServiceCaveat(services);
            var capabilitiesCaveat = await ServiceLimiter.ServiceCapabilities();
            var constraintsCaveat = await ServiceLimiter.ServiceConstraints();
            throw new NotImplementedException();
        }

        public Task<(Macaroon, PaymentRequest)> MintNewLSAT()
        {
            throw new NotImplementedException();
        }
    }
}