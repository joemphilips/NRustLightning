using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;

namespace NRustLightning.Server.Tests.Support
{
    public class DBTrieRepositoryTester : IDisposable
    {
        public static DBTrieRepositoryTester Create([CallerMemberName]string? name = null)
        {
            return new DBTrieRepositoryTester(name);
        }

        public IRepository Repository;
        public FlatFileKeyRepository KeysRepository;
        public string DataDir;

        public Config Config;

        public DBTrieRepositoryTester(string? name)
        {
            DataDir = Path.Combine(Path.GetFullPath(name ?? Path.GetTempPath()), "DataDir");
            Directory.CreateDirectory(DataDir);
            var c = new Config();
            c.DataDir = DataDir;
            c.DBFilePath = Path.Combine(DataDir, "DB");
            Directory.CreateDirectory(c.DBFilePath);
            var l = LoggerFactory.Create(lb  => lb.AddConsole());
            Repository = new DbTrieRepository(Options.Create(c), l.CreateLogger<DbTrieRepository>());
            c.SeedFilePath = Path.Combine(DataDir, "node_secret");
            KeysRepository = new FlatFileKeyRepository(Options.Create(c), l.CreateLogger<FlatFileKeyRepository>());
            Config = c;
        }

        public void Dispose()
        {
            Directory.Delete(DataDir, true);
        }
    }
}