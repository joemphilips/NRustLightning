using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Server.Repository
{
    public class FlatFileKeyRepository : IKeysRepository
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<FlatFileKeyRepository> _logger;
        private Key Secret;

        public FlatFileKeyRepository(IOptions<Config> config, ILogger<FlatFileKeyRepository> logger)
        {
            this._config = config;
            this._logger = logger;
            var filePath = Path.Join(config.Value.DataDir, "node_secret");
            if (File.Exists(filePath))
            {
                Secret = new Key(File.ReadAllBytes(filePath));
            }
            else
            {
                Secret = new Key(RandomUtils.GetBytes(32));
                this._logger.LogInformation($"Could not find key file in {filePath} . So creating new key");
                File.WriteAllBytes(filePath, Secret.ToBytes());
            }
            NodeId = Secret.PubKey;
            logger.LogInformation($"Our nodeid is {NodeId}");
        }

        public RepositorySerializer Serializer { get; set; }
        public IKeysInterface GetKeysInterface(byte[] seed)
        {
            return new KeysManager(seed, DateTime.UtcNow);
        }

        public PubKey NodeId { get; set; }

        public Key GetNodeSecret() => Secret;

        public PubKey GetNodeId() => NodeId;
    }
}