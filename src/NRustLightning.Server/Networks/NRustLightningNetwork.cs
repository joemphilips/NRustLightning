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
        internal NRustLightningNetwork(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork, KeyPath baseKeyPath, string bolt11InvoicePrefix)
        {
            NbXplorerNetwork = nbXplorerNetwork;
            BaseKeyPath = baseKeyPath;
            BOLT11InvoicePrefix = bolt11InvoicePrefix;
            NBitcoinNetwork = networkSet.GetNetwork(networkType);
            CryptoCode = networkSet.CryptoCode;
        }
        
        public NBXplorerNetwork NbXplorerNetwork { get; }

        public Network NBitcoinNetwork { get; }
        public string CryptoCode { get; }
        public string BOLT11InvoicePrefix { get; }
        
        public KeyPath BaseKeyPath { get; }

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

        private void Add(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork, KeyPath baseKeyPath, string bolt11InvoicePrefix)
        {
            _Networks.Add(networkSet.CryptoCode.ToLowerInvariant(), new NRustLightningNetwork(networkSet, networkType, nbXplorerNetwork, baseKeyPath, bolt11InvoicePrefix));
        }
        NBXplorerNetworkProvider nbXplorerNetworkProvider;
        public NRustLightningNetworkProvider(NetworkType networkType)
        {
            NetworkType = networkType;
            nbXplorerNetworkProvider = new NBXplorerNetworkProvider(NetworkType);
            var invoicePrefix =
                networkType == NetworkType.Mainnet ? "lnbc" :
                    networkType == NetworkType.Testnet ? "lntb" :
                    "lnbcrt";
            Add(Bitcoin.Instance, networkType, nbXplorerNetworkProvider.GetBTC(), new KeyPath("m/84'/0'"), invoicePrefix);
            invoicePrefix =
                networkType == NetworkType.Mainnet ? "lnltc" :
                networkType == NetworkType.Testnet ? "lntltc" :
                "lnrltc";
            Add(Monacoin.Instance, networkType, nbXplorerNetworkProvider.GetMONA(), new KeyPath("m/84'/1'"), invoicePrefix);
            invoicePrefix =
                networkType == NetworkType.Mainnet ? "lnmona" :
                networkType == NetworkType.Testnet ? "lntmona" :
                "lnrmona";
            Add(Litecoin.Instance, networkType, nbXplorerNetworkProvider.GetLTC(), new KeyPath("m/84'/22'"), invoicePrefix);
        }

        public IEnumerable<NRustLightningNetwork> GetAll() => _Networks.Values;

        public NRustLightningNetwork GetByCryptoCode(string cryptoCode)
        {
            return _Networks[cryptoCode.ToLowerInvariant()];
        }
    }
}