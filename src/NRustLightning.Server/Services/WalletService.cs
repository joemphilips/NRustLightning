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
        private readonly DerivationStrategyFactory _derivationStrategyFactory;
        private readonly ConcurrentDictionary<NRustLightningNetwork, ExtKey> BaseXPrivs = new ConcurrentDictionary<NRustLightningNetwork, ExtKey>();

        public WalletService(IKeysRepository keysRepository, DerivationStrategyFactory derivationStrategyFactory)
        {
            _keysRepository = keysRepository;
            _derivationStrategyFactory = derivationStrategyFactory;
        }
        
        public IHDScriptPubKey GetOurXPub(NRustLightningNetwork network)
        {
            var hdKey = new ExtKey(_keysRepository.GetNodeSecret().ToBytes());
            return hdKey.Derive(1).AsHDScriptPubKey(ScriptPubKeyType.Segwit);
        }

        public DerivationStrategyBase GetOurDerivationStrategy(NRustLightningNetwork network)
        {
            ExtKey baseKey;
            if (!BaseXPrivs.TryGetValue(network, out baseKey))
            {
                baseKey = new ExtKey(_keysRepository.GetNodeSecret().ToBytes()).Derive(network.BaseKeyPath);
                BaseXPrivs.TryAdd(network, baseKey);
            }
            return _derivationStrategyFactory.CreateDirectDerivationStrategy(baseKey.Neuter());
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