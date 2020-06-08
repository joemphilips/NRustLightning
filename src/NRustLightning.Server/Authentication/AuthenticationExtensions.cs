using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Macaroons;
using Microsoft.AspNetCore.Authentication;
using NBitcoin;

namespace NRustLightning.Server.Authentication
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// See <see cref="LSATAuthenticationHandler"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLSATAuthentication(this AuthenticationBuilder builder,
            Action<LSATAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<LSATAuthenticationOptions, LSATAuthenticationHandler>(LSATDefaults.Scheme, configureOptions);
        }
        /// <summary>
        /// See <see cref="LSATAuthenticationHandler"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLSATAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<LSATAuthenticationOptions, LSATAuthenticationHandler>(LSATDefaults.Scheme, null);
        }

        internal static VerificationResult VerifyLSATCaveats(this Macaroon m, IList<Caveat> caveats,
            IList<ISatisfier> satisfiers, string secret)
        {
            var caveatSatisfiers = new Dictionary<string, ISatisfier>(satisfiers.Count);
            foreach (var s in satisfiers)
            {
                caveatSatisfiers.AddOrReplace(s.Condition, s);
            }
            
            var relevantCaveats = new Dictionary<string, Caveat>();
            foreach (var c in caveats)
            {
                if (!caveatSatisfiers.ContainsKey(c.Condition))
                    continue;
                
                relevantCaveats.AddOrReplace(c.Condition, c);
            }

            foreach (var kv in relevantCaveats)
            {
                var (condition, caveat) = (kv.Key, kv.Value);
                var s = caveatSatisfiers[condition];
                for (int i = 0; i < caveats.Count - 1; i++)
                {
                    var prev = caveats[i];
                    var curr = caveats[i + 1];
                    var vResult = s.SatisfyPrevious(prev, curr);
                    if (!vResult.Success) return vResult;
                }

                var vResultFinal = s.SatisfyFinal(caveats.Last());
                if (!vResultFinal.Success) return vResultFinal;
            }
            
            // Finally we are checking each caveats.
            var v = new Verifier();
            foreach (var c in caveats)
            {
                v.SatisfyExact(c.CId);
            }
            return m.Verify(v, secret);
        }
    }
}