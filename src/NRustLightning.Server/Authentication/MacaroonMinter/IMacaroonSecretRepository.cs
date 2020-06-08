using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using NBitcoin;

namespace NRustLightning.Server.Authentication.MacaroonMinter
{
    public interface IMacaroonSecretRepository
    {
        public Task<string> GetSecretById(MacaroonIdentifier id);
        public Task SetSecret(MacaroonIdentifier id, string secret);
    }

    public class InMemoryMacaroonSecretRepository : IMacaroonSecretRepository
    {
        public ConcurrentDictionary<MacaroonIdentifier, string> secretMap = new ConcurrentDictionary<MacaroonIdentifier, string>();

        public Task<string> GetSecretById(MacaroonIdentifier id)
        {
            if (secretMap.TryGetValue(id, out var secret))
                return Task.FromResult(secret);
            
            throw new InvalidDataException($"Unknown macaroon id! {id}");
        }

        public Task SetSecret(MacaroonIdentifier id, string secret)
        {
            secretMap.AddOrReplace(id, secret);
            return Task.CompletedTask;
        }
    }
}