using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.Utils;
using RustLightningTypes;

namespace NRustLightning.Infrastructure.Repository
{
    /// <summary>
    /// TODO: Stop using KeysManager and do not hold decrypted keys in memory.
    /// </summary>
    public class FlatFileKeyRepository : IKeysRepository
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<FlatFileKeyRepository> _logger;
        private KeysManager? _keysManager;
        private HexEncoder _hex;
        
        // sha256 hash of ascii encoded "nrustlightning"
        private const string HashSalt = "1c822a9db920d0cdab2f835ce4821a28bd8467f6c9fc666770e72d5c80d7f3bd";

        public FlatFileKeyRepository(IOptions<Config> config, ILogger<FlatFileKeyRepository> logger)
        {
            _config = config;
            _logger = logger;
            _hex = new HexEncoder();
        }

        public string? InitializeFromEncryptedSeed(string encryptedSeed, string pin)
        {
            byte[] seed;
            try
            {
                seed = DecryptEncryptedSeed(_hex.DecodeData(encryptedSeed), pin);
            }
            catch (NRustLightningException e)
            {
                return e.Message;
            }

            InitializeFromSeed(seed);
            return null;
        }

        public void InitializeFromSeed(string seed)
            => InitializeFromSeed(_hex.DecodeData(seed));
        
        private void InitializeFromSeed(byte[] seed)
        {
            if (seed == null) throw new ArgumentNullException(nameof(seed));
            if (seed.Length != 32) throw new ArgumentOutOfRangeException(nameof(seed), seed.Length, $"seed must be 32 bytes");
            _keysManager = new KeysManager(seed, DateTime.UtcNow);
            _logger.LogInformation($"Our nodeid is {GetNodeSecret().PubKey.ToHex()}");
        }

        public Task EncryptSeedAndSaveToFile(string seed, string pin) =>
            EncryptSeedAndSaveToFile(_hex.DecodeData(seed), pin);
        public async Task EncryptSeedAndSaveToFile(byte[] seed, string pin)
        {
            var password = new uint256(Hashes.SHA256(Encoding.UTF8.GetBytes(pin).Concat(_hex.DecodeData(HashSalt)).ToArray()));
            var encryptedSeed = DotNetLightning.Crypto.CryptoUtils.impl.encryptWithAD(0UL, password, ReadOnlySpan<byte>.Empty, seed);
            await File.WriteAllTextAsync(_config.Value.SeedFilePath, _hex.EncodeData(encryptedSeed));
        }

        public byte[] DecryptEncryptedSeed(byte[] encryptedSeed, string pin)
        {
            var password = new uint256(Hashes.SHA256(Encoding.UTF8.GetBytes(pin).Concat(_hex.DecodeData(HashSalt)).ToArray()));
            var r = DotNetLightning.Crypto.CryptoUtils.impl.decryptWithAD(0UL, password, new byte[0], encryptedSeed);
            if (r.IsError)
            {
                throw new NRustLightningException($"Pin code mismatch: {r.ErrorValue}");
            }
            var seed = r.ResultValue;
            if (seed.Length != 32)
            {
                throw new NRustLightningException($"Seed corrupted. bad length: {seed.Length}");
            }

            return seed;
        }
        public RepositorySerializer Serializer { get; set; }
        public Key GetNodeSecret() =>
            _keysManager?.GetNodeSecret() ?? Utils.Utils.Fail<Key>("Failed to get node_secret: KeysRepository not initialized");
        public Script GetDestinationScript() =>
            _keysManager?.GetDestinationScript() ?? Utils.Utils.Fail<Script>("Failed to get destination script: KeysRepository not initialized");

        public PubKey GetShutdownKey() =>
            _keysManager?.GetShutdownKey() ?? Utils.Utils.Fail<PubKey>("Failed to get shutdown key. KeysRepository not initialized");

        public ChannelKeys GetChannelKeys(bool inbound, ulong channelValueSatoshis) =>
            _keysManager?.GetChannelKeys(inbound, channelValueSatoshis) ?? Utils.Utils.Fail<ChannelKeys>("Failed to get channel keys. Keys Repository not initialized");

        public Tuple<Key, uint256> GetOnionRand() =>
            _keysManager?.GetOnionRand() ?? Utils.Utils.Fail<Tuple<Key, uint256>>("Failed to get onion rand. Keys Repository not initialized");

        public uint256 GetChannelId() =>
            _keysManager?.GetChannelId() ?? Utils.Utils.Fail<uint256>("Failed to get onion rand. Keys Repository not initialized");

    }
}