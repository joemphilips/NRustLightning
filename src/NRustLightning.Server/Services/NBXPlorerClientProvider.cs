using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Configuration.SubConfiguration;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;

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

                    var timeout = new CancellationTokenSource(4000);
                    try
                    {
                        c.WaitServerStarted(timeout.Token);
                        c.GetStatus();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Failed to connect to nbxplorer! cryptoCode: {n.CryptoCode} url: {config.Value.NBXplorerUri}. cookiefile: {config.Value.NBXCookieFile}" );
                        throw;
                    }

                    explorerClients.Add(n.CryptoCode, c);
                }
            }

            if (explorerClients.Count == 0)
            {
                throw new NRustLightningException("Found zero valid nbxplorer instance to connect");
            }
        }

        public ExplorerClient GetClient(string cryptoCode)
        {
            explorerClients.TryGetValue(cryptoCode.ToLowerInvariant(), out var c);
            return c ?? Infrastructure.Utils.Utils.Fail<ExplorerClient>($"Unknown cryptoCode {cryptoCode}");
        }

        public ExplorerClient? TryGetClient(string cryptoCode)
        {
            explorerClients.TryGetValue(cryptoCode.ToLowerInvariant(), out var c);
            return c;
        }
        public IEnumerable<ExplorerClient> GetAll()
        {
            return explorerClients.Values;
        }
    }
}