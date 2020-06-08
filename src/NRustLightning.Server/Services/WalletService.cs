using System;
using System.Collections.Concurrent;
using NBitcoin;
using NBXplorer.DerivationStrategy;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Services
{
    public class WalletService
    {
        private readonly IKeysRepository _keysRepository;
        private readonly NBXplorerClientProvider _nbXplorerClientProvider;
        private readonly ConcurrentDictionary<NRustLightningNetwork, ExtKey> BaseXPrivs = new ConcurrentDictionary<NRustLightningNetwork, ExtKey>();

        public WalletService(IKeysRepository keysRepository, NBXplorerClientProvider nbXplorerClientProvider)
        {
            _keysRepository = keysRepository;
            _nbXplorerClientProvider = nbXplorerClientProvider;
        }
        
        public IHDScriptPubKey GetOurXPub(NRustLightningNetwork network)
        {
            var hdKey = new ExtKey(_keysRepository.GetNodeSecret().ToBytes());
            return hdKey.Derive(1).AsHDScriptPubKey(ScriptPubKeyType.Segwit);
        }

        private ExtKey GetBaseXPriv(NRustLightningNetwork network)
        {
            ExtKey baseKey;
            if (!BaseXPrivs.TryGetValue(network, out baseKey))
            {
                baseKey = new ExtKey(_keysRepository.GetNodeSecret().ToBytes()).Derive(network.BaseKeyPath);
                BaseXPrivs.TryAdd(network, baseKey);
            }
            return baseKey;
        }

        public DerivationStrategyBase GetOurDerivationStrategy(NRustLightningNetwork network)
        {
            var baseKey = GetBaseXPriv(network);
            var strategy = network.NbXplorerNetwork.DerivationStrategyFactory.CreateDirectDerivationStrategy(baseKey.Neuter(),
                new DerivationStrategyOptions
                {
                    ScriptPubKeyType = ScriptPubKeyType.Segwit
                });
            return strategy;
        }

        public BitcoinAddress GetNewAddress(NRustLightningNetwork network)
        {
            return GetBaseXPriv(network).Neuter().Derive(1).GetPublicKey().GetSegwitAddress(network.NBitcoinNetwork);
        }

        public PSBT SignPSBT(PSBT psbt, NRustLightningNetwork network)
        {
            if (BaseXPrivs.TryGetValue(network, out var xpriv))
            {
                psbt.SignAll(ScriptPubKeyType.Segwit, xpriv);
            }

            return psbt;
        }
    }
}