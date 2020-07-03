using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBXplorer.DerivationStrategy;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Tests.Stubs
{
    public class StubWalletService : IWalletService
    {
        private readonly IKeysRepository _keysRepository;
        private uint _derivationCount;

        public StubWalletService(IKeysRepository keysRepository)
        {
            _keysRepository = keysRepository;
        }
        private BitcoinExtKey GetBaseXPriv(NRustLightningNetwork network)
        {
            return new BitcoinExtKey(new ExtKey(_keysRepository.GetNodeSecret().ToBytes()), network.NBitcoinNetwork).Derive(network.BaseKeyPath);
        }
        
        public async ValueTask<DerivationStrategyBase> GetOurDerivationStrategyAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var baseKey = GetBaseXPriv(network);
            var strategy = network.NbXplorerNetwork.DerivationStrategyFactory.CreateDirectDerivationStrategy(baseKey.Neuter(),
                new DerivationStrategyOptions
                {
                    ScriptPubKeyType = ScriptPubKeyType.Segwit
                });
            return strategy;
        }

        public Task<Transaction> GetSendingTxAsync(BitcoinAddress destination, Money amount, NRustLightningNetwork network,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(network.NBitcoinNetwork.CreateTransaction());
        }

        public Task<Money> GetBalanceAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            return Task.FromResult(Money.Zero);
        }

        public Task<BitcoinAddress> GetNewAddressAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var p = this.GetBaseXPriv(network);
            var t = Task.FromResult(p.Derive(_derivationCount).Neuter().ExtPubKey.PubKey.WitHash.GetAddress(network.NBitcoinNetwork));
            _derivationCount++;
            return t;
        }

        public Task BroadcastAsync(Transaction tx, NRustLightningNetwork network)
        {
            return Task.CompletedTask;
        }
    }
}