using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Configuration.SubConfiguration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Services
{
    public class NBXplorerClientProvider : INBXplorerClientProvider
    {
        private readonly ILogger<NBXplorerClientProvider> _logger;
        Dictionary<string, ExplorerClient> explorerClients = new Dictionary<string, ExplorerClient>();

        public NBXplorerClientProvider(IOptions<Config> config, IOptionsMonitor<ChainConfiguration> chainConfig, NRustLightningNetworkProvider networkProvider, IHttpClientFactory httpClientFactory, ILogger<NBXplorerClientProvider> logger)
        {
            _logger = logger;
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

                    int sleepMs = 50;
                    Exception e = null;
                    int maxRetry = 6;
                    for (int count = 0; count <= maxRetry; count++)
                    {
                        try
                        {
                            var _ = c.GetStatus();
                            e = null;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Failed to connect to nbxplorer. retrying in {sleepMs} milliseconds...");
                            e = ex;
                            Thread.Sleep(sleepMs);
                            sleepMs *= 2;
                        }
                    }

                    if (e != null)
                    {
                        _logger.LogCritical($"Failed to connect nbxplorer. check your settings.");
                        throw e;
                    }

                    explorerClients.Add(n.CryptoCode, c);
                }
            }
        }

        public ExplorerClient GetClient(string cryptoCode)
        {
            explorerClients.TryGetValue(cryptoCode, out var c);
            return c ?? Utils.Utils.Fail<ExplorerClient>($"Unknown cryptoCode {cryptoCode}");
        }
        public ExplorerClient GetClient(NRustLightningNetwork n) => GetClient(n.CryptoCode);
    }
}