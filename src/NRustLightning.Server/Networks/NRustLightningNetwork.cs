using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Altcoins;
using NBXplorer;
using Network = NBitcoin.Network;
using FFINetwork =  NRustLightning.Adaptors.Network;

namespace NRustLightning.Server.Networks
{
    public class NRustLightningNetwork
    {
        internal NRustLightningNetwork(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork)
        {
            NbXplorerNetwork = nbXplorerNetwork;
            NBitcoinNetwork = networkSet.GetNetwork(networkType);
            CryptoCode = networkSet.CryptoCode;
        }
        
        public NBXplorerNetwork NbXplorerNetwork { get; }

        public Network NBitcoinNetwork { get; }
        public string CryptoCode { get; }

        public bool SupportCookieAuthentication { get; internal set; } = true;

        internal FFINetwork FFINetwork => NBitcoinNetwork.NetworkType switch
            {
                NetworkType.Mainnet => FFINetwork.MainNet,
                NetworkType.Regtest => FFINetwork.RegTest,
                NetworkType.Testnet => FFINetwork.TestNet,
                _ => throw new Exception($"Unreachable!")
            };
    }

    public class NRustLightningNetworkProvider
    {
        public NetworkType NetworkType { get; }
        Dictionary<string, NRustLightningNetwork> _Networks = new Dictionary<string, NRustLightningNetwork>();

        private void Add(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork)
        {
            _Networks.Add(networkSet.CryptoCode, new NRustLightningNetwork(networkSet, networkType, nbXplorerNetwork));
        }
        NBXplorerNetworkProvider nbXplorerNetworkProvider;
        public NRustLightningNetworkProvider(NetworkType networkType)
        {
            nbXplorerNetworkProvider = new NBXplorerNetworkProvider(NetworkType);
            NetworkType = networkType;
            Add(Bitcoin.Instance, networkType, nbXplorerNetworkProvider.GetBTC());
            Add(Monacoin.Instance, networkType, nbXplorerNetworkProvider.GetMONA());
            Add(Litecoin.Instance, networkType, nbXplorerNetworkProvider.GetLTC());
        }

        public IEnumerable<NRustLightningNetwork> GetAll() => _Networks.Values;
    }
}