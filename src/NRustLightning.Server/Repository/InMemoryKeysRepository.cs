using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;

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
            logger.LogInformation($"and secret is {hex.EncodeData(Secret.ToBytes())}");
        }

        public Key GetNodeSecret() => Secret;
   }
}