using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using NBitcoin.RPC;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Services
{
    public class RPCClientProvider : IRPCClientProvider
    {
        Dictionary<string, RPCClient> _ChainClients = new Dictionary<string, RPCClient>();

        public RPCClientProvider(NRustLightningNetworkProvider networkProvider, IOptionsMonitor<ChainConfiguration> chainConfiguration, IHttpClientFactory httpClientFactory)
        {
            foreach (var n in networkProvider.GetAll())
            {
                var chainConfig = chainConfiguration.Get(n.CryptoCode);
                if (!(chainConfig is null))
                {
                    var rpc = new RPCClient(chainConfig.CredentialString, chainConfig.NodeEndPoint, n.NBitcoinNetwork);
                    rpc.HttpClient = httpClientFactory.CreateClient(nameof(RPCClientProvider));
                    _ChainClients.Add(n.CryptoCode, rpc);
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