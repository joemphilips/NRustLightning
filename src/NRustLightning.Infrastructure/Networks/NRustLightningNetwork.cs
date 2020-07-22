using System;
using System.Collections.Generic;
using DotNetLightning.Payment;
using NBitcoin;
using NBXplorer;
using Network = NBitcoin.Network;
using FFINetwork =  NRustLightning.Adaptors.Network;

namespace NRustLightning.Infrastructure.Networks
{
    public class NRustLightningNetwork
    {
        internal NRustLightningNetwork(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork, KeyPath baseKeyPath, string bolt11InvoicePrefix)
        {
            NbXplorerNetwork = nbXplorerNetwork;
            BaseKeyPath = baseKeyPath;
            BOLT11InvoicePrefix = bolt11InvoicePrefix;
            NBitcoinNetwork = networkSet.GetNetwork(networkType);
            CryptoCode = networkSet.CryptoCode.ToLowerInvariant();
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
        Dictionary<string, NRustLightningNetwork> _invoicePrefixToNetwork = new Dictionary<string, NRustLightningNetwork>();

        private void Add(INetworkSet networkSet, NetworkType networkType, NBXplorerNetwork nbXplorerNetwork, KeyPath baseKeyPath, string bolt11InvoicePrefix)
        {
            var n = new NRustLightningNetwork(networkSet, networkType, nbXplorerNetwork, baseKeyPath,
                bolt11InvoicePrefix);
            _Networks.Add(networkSet.CryptoCode.ToLowerInvariant(), n);
            _invoicePrefixToNetwork.Add(bolt11InvoicePrefix, n);
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
        }

        public IEnumerable<NRustLightningNetwork> GetAll() => _Networks.Values;

        public NRustLightningNetwork GetByCryptoCode(string cryptoCode)
        {
            return _Networks[cryptoCode.ToLowerInvariant()];
        }

        public NRustLightningNetwork? TryGetByInvoice(PaymentRequest payreq)
        {
            NRustLightningNetwork? result;
            _invoicePrefixToNetwork.TryGetValue(payreq.PrefixValue, out result);
            return result;
        }

        public NRustLightningNetwork GetByInvoice(PaymentRequest payreq)
        {
            return TryGetByInvoice(payreq) ?? Utils.Utils.Fail<NRustLightningNetwork>($"Unknown invoice prefix {payreq.PrefixValue}");
        }
    }
}