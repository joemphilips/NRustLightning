using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using DotNetLightning.Utils;
using Macaroons;
using NBitcoin;

namespace NRustLightning.Server.Authentication
{
    /// <summary>
    /// When we verify a macaroon for its caveats, usually it check each caveats independently
    /// In case of LSAT, this does not work since the validity of a caveat depends on a previous caveat
    /// (more specifically, if there were two caveats with a same Condition, we usually check that the restriction is
    /// is the same with the one in previous or stricter than before.)
    /// So we can not just rely on Macaroons <see cref="Verifier"/> . The additional check is done with this ISatisfier
    /// interface
    /// </summary>
    public interface ISatisfier
    {
        /// <summary>
        /// This is the left side of caveat equation. e.g. for caveat "service=nrustlightning", it is "service"
        /// </summary>
        public string Condition { get; }

        /// <summary>
        /// ensures a caveat is in accordance with a previous one with the same condition. This is needed since caveats
        /// of the same condition can be used multiple times as long as they enforce more permissions than the previous.
        ///
        /// For example, we have a caveat that only allows us to use an LSAT for 7 more days. we can add another caveat
        /// that only allows for 3 more days of use and lend it to another party.
        /// </summary>
        /// <param name="previousCaveat"></param>
        /// <param name="currentCaveat"></param>
        public VerificationResult SatisfyPrevious(Caveat previousCaveat, Caveat currentCaveat);

        /// <summary>
        /// Satisfies the final caveat of an LSAT. If multiple caveats with the same condition exist, this will only
        /// be executed once all previous caveats are also satisfied.
        /// </summary>
        /// <param name="caveat"></param>
        public VerificationResult SatisfyFinal(Caveat caveat);
    }

    public class Service
    {
        public readonly string Name;
        public readonly byte ServiceTier;
        public readonly long? Price;

        public Service(string name, byte serviceTier, long? price = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ServiceTier = serviceTier;
            Price = price;
        }
    }

    public class ServiceSatisfier : ISatisfier
    {
        public string TargetService { get; }
        public string Condition { get; } = "service";
        
        public ServiceSatisfier(string targetService)
        {
            TargetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
        }

        private static List<Service> DecodeServicesCaveatValue(string s)
        {
            var result = new List<Service>();
            if (string.IsNullOrEmpty(s)) return result;

            var rawServices = s.Split(',');
            foreach (var rawService in rawServices)
            {
                var serviceInfo = rawService.Split(':');
                if (serviceInfo.Length != 2)
                {
                    throw new FormatException($"invalid service {s}");
                }
                var (name, tier) = (serviceInfo[0], byte.Parse(serviceInfo[1]));
                if (string.IsNullOrEmpty(name)) throw new FormatException($"empty service in {s}");
                result.Add(new Service(name, tier));
                
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousCaveat"></param>
        /// <param name="currentCaveat"></param>
        /// <exception cref="FormatException">when caveat encoding does not follow the format specified in LSAT spec</exception>
        public VerificationResult SatisfyPrevious(Caveat previousCaveat, Caveat currentCaveat)
        {
            var prevServices = DecodeServicesCaveatValue(previousCaveat.ToString());
            var prevAllowed = new Dictionary<string, Service>();
            foreach (var s in prevServices)
            {
                prevAllowed.AddOrReplace(s.Name, s);
            }

            var currentServices = DecodeServicesCaveatValue(currentCaveat.ToString());

            foreach (var s in currentServices)
            {
                if (!prevAllowed.ContainsKey(s.Name))
                {
                    return new VerificationResult($"service {s} was not previously allowed");
                }
            }

            return new VerificationResult();
        }

        public VerificationResult SatisfyFinal(Caveat caveat)
        {
            var services = DecodeServicesCaveatValue(caveat.ToString());
            foreach (var s in services)
            {
                if (s.Name == TargetService)
                {
                    return new VerificationResult();
                }
            }
            
            return new VerificationResult($"Target service {TargetService} not found");
        }
    }
    
    /// <summary>
    /// Implements a satisfier to determine whether the target capabilities for a service is authorized
    /// for a given LSAT.
    /// </summary>
    public class CapabilitiesSatisfier: ISatisfier {
        public string TargetCapability { get; }
        public string Condition { get; }
        
        public CapabilitiesSatisfier(string service, string targetCapability)
        {
            Condition = $"{service}{LSATDefaults.CapabilitiesConditionPrefix}";
            TargetCapability = targetCapability ?? throw new ArgumentNullException(nameof(targetCapability));
        }
        
        public VerificationResult SatisfyPrevious(Caveat previousCaveat, Caveat currentCaveat)
        {
            var prevCapabilities = previousCaveat.Value.Split(',');
            var allowed = new HashSet<string>(prevCapabilities);

            var currentCapabilities = currentCaveat.Value.Split(',');
            foreach (var c in currentCapabilities)
            {
                if (!allowed.Contains(c))
                {
                    return new VerificationResult($"capability {c} was not previously allowed");
                }
            }
            return new VerificationResult();
        }

        public VerificationResult SatisfyFinal(Caveat caveat)
        {
            var caps = caveat.Value.Split(',');
            foreach (var c in caps)
            {
                if (c == TargetCapability)
                    return new VerificationResult();
            }
            return new VerificationResult($"target capability {TargetCapability} not authorized");
        }
    }
}