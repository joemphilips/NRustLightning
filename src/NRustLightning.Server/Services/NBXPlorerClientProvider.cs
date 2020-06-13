using System;
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
                    if (!string.IsNullOrEmpty(config.Value.NBXCookieFile))
                        c.SetCookieAuth(config.Value.NBXCookieFile);
                    c.SetClient(httpClientFactory.CreateClient(nameof(NBXplorerClientProvider)));
                    // check the connection by getting status.
                    // TODO: Prepare HostedService for waiting NBXplorer and bitcoind gets ready?
                    var status = c.GetStatus();
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