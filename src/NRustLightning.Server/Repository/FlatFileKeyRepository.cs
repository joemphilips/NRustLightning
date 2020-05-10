using System.IO;
using System.Security.Permissions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Repository
{
    public class FlatFileKeyRepository : IKeysRepository
    {
        private readonly IOptions<Config> config;
        private readonly ILogger<FlatFileKeyRepository> logger;
        private Key Secret;
        public PubKey NodeId { get; set; }

        public FlatFileKeyRepository(IOptions<Config> config, ILogger<FlatFileKeyRepository> logger)
        {
            this.config = config;
            this.logger = logger;
            var filePath = Path.Join(config.Value.DataDir, "node_secret");
            if (File.Exists(filePath))
            {
                Secret = new Key(File.ReadAllBytes(filePath));
            }
            else
            {
                Secret = new Key(RandomUtils.GetBytes(32));
                this.logger.LogInformation($"Could not find key file in {filePath} . So creating new key");
                File.WriteAllBytes(filePath, Secret.ToBytes());
            }
            NodeId = Secret.PubKey;
            logger.LogInformation($"Our nodeid is {NodeId}");
        }

        public Key GetNodeSecret() => Secret;

        public PubKey GetNodeId() => NodeId;
    }
}