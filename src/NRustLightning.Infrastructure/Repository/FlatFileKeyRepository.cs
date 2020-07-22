using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Utils;
using RustLightningTypes;

namespace NRustLightning.Infrastructure.Repository
{
    public class FlatFileKeyRepository : IKeysRepository
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<FlatFileKeyRepository> _logger;
        private KeysManager _keysManager;

        public FlatFileKeyRepository(IOptions<Config> config, ILogger<FlatFileKeyRepository> logger)
        {
            _config = config;
            _logger = logger;
            var seed = _config.Value.GetSeed().GetAwaiter().GetResult();
            _keysManager = new KeysManager(seed, DateTime.UtcNow);
            _logger.LogInformation($"Our nodeid is {GetNodeSecret().PubKey.ToHex()}");
        }

        public RepositorySerializer Serializer { get; set; }
        public Key GetNodeSecret() => _keysManager.GetNodeSecret();
        public Script GetDestinationScript() => _keysManager.GetDestinationScript();

        public PubKey GetShutdownKey() => _keysManager.GetShutdownKey();

        public ChannelKeys GetChannelKeys(bool inbound, ulong channelValueSatoshis) =>
            _keysManager.GetChannelKeys(inbound, channelValueSatoshis);

        public Tuple<Key, uint256> GetOnionRand() =>
            _keysManager.GetOnionRand();

        public uint256 GetChannelId() => _keysManager.GetChannelId();

    }
}