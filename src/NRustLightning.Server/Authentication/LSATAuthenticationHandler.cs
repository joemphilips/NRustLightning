using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Macaroons;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Authentication
{
    /// <summary>
    /// Authenticate a user with macaroon. The macaroon must have caveat of the form 'services = foo_service'
    /// where foo_service is the name of your service you have specified in <see cref="LSATAuthenticationOptions"/>
    /// If the client did not send a valid macaroon, it will return "402 payment required" http response. with macaroon
    /// and an invoice (So the consumer of this API must specify the way to get invoice somehow.), only after user payed
    /// to that invoice, the macaroon will be valid.
    /// For more detail about LSAT protocol, see: https://github.com/lightninglabs/LSAT
    /// </summary>
    public class LSATAuthenticationHandler : AuthenticationHandler<LSATAuthenticationOptions>
    {
        private readonly IMacaroonSecretRepository _macaroonSecretRepository;
        private HexEncoder _hex;
        public Verifier MacaroonVerifier { get; set; }
        public ILSATInvoiceProvider? _InvoiceRepository;
        public LSATAuthenticationHandler(IOptionsMonitor<LSATAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IMacaroonSecretRepository macaroonSecretRepository, IServiceProvider serviceProvider) : base(options, logger, encoder, clock)
        {
            _macaroonSecretRepository = macaroonSecretRepository;
            _InvoiceRepository = serviceProvider.GetService<ILSATInvoiceProvider>();
            _hex = new HexEncoder();
        }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where
        /// processing is occuring. If it is not provided a default instance is supplied which does nothing when
        /// the methods are called
        /// </summary>
        protected new LSATAuthenticationEvents Events
        {
            get => (LSATAuthenticationEvents) base.Events;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new LSATAuthenticationEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            Macaroon? macaroon;
            Primitives.PaymentPreimage? preimage;
            try
            {
                var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
                await Events.OnMessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application sets macaroon or preimage somewhere else, use that. 
                macaroon = messageReceivedContext.Macaroon;
                preimage = messageReceivedContext.Preimage;
                if (macaroon is null || preimage is null)
                {
                    var authHeader = Request.Headers[HeaderNames.Authorization];

                    // If no authorization header found, nothing to process further
                    if (string.IsNullOrEmpty(authHeader))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    var header = ((string) authHeader).TrimStart();
                    if (header.StartsWith("LSAT", StringComparison.OrdinalIgnoreCase))
                    {
                        var token = header.Substring("LSAT ".Length).Trim();
                        var s = token.Split(":");
                        if (s.Length == 2)
                        {
                            (macaroon, preimage) = (Macaroon.Deserialize(s[0]),
                                Primitives.PaymentPreimage.Create(_hex.DecodeData(s[1])));
                        }
                    }
                    // if no token found, no further work possible
                    if (macaroon is null || preimage is null)
                    {
                        return AuthenticateResult.NoResult();
                    }
                }

                var id = MacaroonIdentifier.TryParse(macaroon.Identifier.ToString());
                if (id.IsError) return AuthenticateResult.Fail(id.ErrorValue);
                if (id.ResultValue is MacaroonIdentifier.UnknownVersion v) return AuthenticateResult.Fail($"Unknown macaroon version {v}");
                switch (id.ResultValue.Tag)
                {
                    case MacaroonIdentifier.Tags.V0:
                        var v0 = ((MacaroonIdentifier.V0)id.ResultValue).Item;
                        if (!v0.PaymentHash.Equals(preimage.Hash)) return AuthenticateResult.Fail($"Invalid payment preimage ({preimage}) for {v0.PaymentHash}");
                        break;
                    default:
                        throw new Exception("Unreachable!");
                }

                var secret = await _macaroonSecretRepository.GetSecretById(id.ResultValue);
                var satisfiers = new List<ISatisfier>() {new ServiceSatisfier(Options.ServiceName)};
                var verificationResult = macaroon.VerifyLSATCaveats(macaroon.Caveats, satisfiers, secret);
                if (!verificationResult.Success)
                    return AuthenticateResult.Fail(verificationResult.ToString());

                // successfully verified user request.
                // Add all capabilities as claims.
                var claimsList = new List<Claim>();
                foreach (var c in macaroon.Caveats)
                {
                    claimsList.Add(new Claim(c.Condition, c.Value));
                }
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claimsList));

                var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                {
                    Principal = principal,
                };
                await Events.OnTokenValidated(tokenValidatedContext);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, null, LSATDefaults.Scheme));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private async Task<Macaroon> GetAuthMacaroon(Primitives.PaymentHash paymentHash)
        {
            var secret = RandomUtils.GetUInt256().ToString();
            var id = MacaroonIdentifier.NewV0(MacaroonIdentifierV0.Create(paymentHash));
            await _macaroonSecretRepository.SetSecret(id, secret);
            var idPacket = new Packet(id.ToHex(), DataEncoding.Hex);
            var m = new Macaroon(new Packet(Options.ServiceLocation), new Packet(secret, DataEncoding.Hex), idPacket);
            m.AddFirstPartyCaveat($"services={Options.ServiceName}:{Options.ServiceTier}");
            foreach (var c in Options.MacaroonCaveats)
            {
                m.AddFirstPartyCaveat(c);
            }
            return m;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var paymentRequiredContext = new PaymentRequiredContext(Context, Scheme, Options, properties);
            await Events.OnPaymentRequired(paymentRequiredContext);

            PaymentRequest? invoice = null;
            if (paymentRequiredContext.Bolt11Invoice != null)
            {
                invoice = paymentRequiredContext.Bolt11Invoice;
            }
            else if (_InvoiceRepository != null)
            {
                invoice = await _InvoiceRepository.GetNewInvoiceAsync(Options.InvoiceCreationOption);
            }
            if (invoice is null)
            {
                Logger.LogError(
                    "Failed to get invoice for LSAT! You must set your invoice to PaymentRequiredContext");
                return;
            }

            invoice = paymentRequiredContext.Bolt11Invoice;
            var m = await GetAuthMacaroon(invoice?.PaymentHash).ConfigureAwait(false);

            Response.StatusCode = 402;
            Response.Headers.AddOrReplace(HeaderNames.WWWAuthenticate, $"LSAT macaroon={m.Serialize()}, invoice={invoice}");
        }
    }
}