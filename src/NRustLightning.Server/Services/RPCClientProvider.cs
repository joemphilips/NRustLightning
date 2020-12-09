using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using NBitcoin.RPC;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Services
{
    public class RPCClientProvider : IRPCClientProvider
    {
        Dictionary<string, RPCClient> _ChainClients = new Dictionary<string, RPCClient>();

        public RPCClientProvider(IOptions<Config> config, IHttpClientFactory httpClientFactory)
        {
            foreach (var chainConfig in config.Value.ChainConfiguration)
            {
                var rpc = chainConfig?.Rpc;
                if (!(rpc is null))
                {
                    rpc.HttpClient = httpClientFactory.CreateClient(nameof(RPCClientProvider));
                    _ChainClients.Add(chainConfig!.CryptoCode, rpc);
                }
            }
        }

        public IEnumerable<RPCClient> GetAll() => _ChainClients.Values;

        public RPCClient? GetRpcClient(string cryptoCode)
        {
            _ChainClients.TryGetValue(cryptoCode, out var rpc);
            return rpc;
        }

        public RPCClient? GetRpcClient(NRustLightningNetwork n) => GetRpcClient(n.CryptoCode);
    }
}