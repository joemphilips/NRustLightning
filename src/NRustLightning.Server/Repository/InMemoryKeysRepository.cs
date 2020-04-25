using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Repository
{
    public class InMemoryKeysRepository : IKeysRepository
    {
        private byte[] Blob;
        public InMemoryKeysRepository()
        {
            Blob = RandomUtils.GetBytes(32);
        }

        public byte[] GetNodeSecret() => Blob;
   }
}