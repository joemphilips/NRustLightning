using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using NBXplorer;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Configuration.SubConfiguration;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Services
{
    public class NBXplorerClientProvider
    {
        Dictionary<string, ExplorerClient> explorerClients = new Dictionary<string, ExplorerClient>();

        public NBXplorerClientProvider(IOptions<Config> config, IOptionsMonitor<ChainConfiguration> chainConfig, NRustLightningNetworkProvider networkProvider, IHttpClientFactory httpClientFactory)
        {
            foreach (var n in networkProvider.GetAll())
            {
                var chainConf = chainConfig.Get(n.CryptoCode);
                if (!(chainConf is null))
                {
                    var c = new ExplorerClient(n.NbXplorerNetwork, config.Value.NBXplorerUri);
                    c.SetClient(httpClientFactory.CreateClient(nameof(NBXplorerClientProvider)));
                    explorerClients.Add(n.CryptoCode, c);
                }
            }
        }

        public ExplorerClient? GetClient(string cryptoCode)
        {
            explorerClients.TryGetValue(cryptoCode, out var c);
            return c;
        }
        public ExplorerClient? GetClient(NRustLightningNetwork n) => GetClient(n.CryptoCode);
    }
}