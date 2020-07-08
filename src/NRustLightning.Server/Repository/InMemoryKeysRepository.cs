using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Server.Repository
{
    public class InMemoryKeysRepository : IKeysRepository
    {
        private readonly ILogger<InMemoryKeysRepository> logger;
        private Key Secret;
        private PubKey NodeId;
        private HexEncoder hex;
        public InMemoryKeysRepository(ILogger<InMemoryKeysRepository> logger)
        {
            this.logger = logger;
            Secret = new Key(RandomUtils.GetBytes(32));
            NodeId = Secret.PubKey;
            
            hex = new HexEncoder();
            logger.LogInformation($"Our node id is {hex.EncodeData(NodeId.ToBytes())}");
        }

        public RepositorySerializer Serializer { get; set; }
        public IKeysInterface GetKeysInterface(byte[] seed)
        {
            return new KeysManager(seed, DateTime.UtcNow);
        }

        public Key GetNodeSecret() => Secret;
        public PubKey GetNodeId()
        {
            return NodeId;
        }
    }
}