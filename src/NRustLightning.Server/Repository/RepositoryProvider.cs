using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Configuration.SubConfiguration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Repository
{
    public class RepositoryProvider
    {
        public Config Config { get; }
        
        Dictionary<string, IKeysRepository> _keysRepositories = new Dictionary<string, IKeysRepository>();
        Dictionary<string, IInvoiceRepository> _invoiceRepositories = new Dictionary<string, IInvoiceRepository>();
        Dictionary<string, RepositorySerializer> _repositorySerializers = new Dictionary<string, RepositorySerializer>();

        public RepositoryProvider(NRustLightningNetworkProvider networks, IOptions<Config> config, IServiceProvider serviceProvider)
        {
            Config = config.Value;
            var directory = Path.Combine(Config.DataDir, "db");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (var n in networks.GetAll())
            {
                var settings = GetChainSetting(n);
                if (settings != null)
                {
                    var keysRepository = serviceProvider.GetRequiredService<IKeysRepository>();
                    var serializer = new RepositorySerializer(n);
                    keysRepository.Serializer = serializer;
                    _keysRepositories.Add(n.CryptoCode, keysRepository);
                    _repositorySerializers.Add(n.CryptoCode, serializer);
                    
                    var invoiceRepository = serviceProvider.GetRequiredService<IInvoiceRepository>();
                    _invoiceRepositories.Add(n.CryptoCode, invoiceRepository);
                }
            }
        }
        
        private ChainConfiguration? GetChainSetting(NRustLightningNetwork n)
        {
            return Config.ChainConfiguration.FirstOrDefault(c => c.CryptoCode == n.CryptoCode);
        }

        public IEnumerable<IInvoiceRepository> GetAllInvoiceRepositories()
        {
            return _invoiceRepositories.Values;
        }

        public IEnumerable<IKeysRepository> GetAllKeysRepositories()
        {
            return _keysRepositories.Values;
        }

        public IInvoiceRepository? GetInvoiceRepository(NRustLightningNetwork network)
        {
            _invoiceRepositories.TryGetValue(network.CryptoCode, out var repo);
            return repo;
        }

        public IKeysRepository? GetKeysRepository(NRustLightningNetwork network)
        {
            _keysRepositories.TryGetValue(network.CryptoCode, out var repo);
            return repo;
        }
        
        public RepositorySerializer? GetSerializer(NRustLightningNetwork network)
        {
            _repositorySerializers.TryGetValue(network.CryptoCode, out var ser);
            return ser;
        }
    }
}