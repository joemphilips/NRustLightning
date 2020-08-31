using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models.Response;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Services;
using ResultUtils;
using UTXO = NRustLightning.Infrastructure.Models.Response.UTXO;

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

        public Task<UTXOChangesWithMetadata> ListUnspent(NRustLightningNetwork network)
        {
            var res= new UTXOChangesWithMetadata();
            var confirmed = new UTXOChangeWithSpentOutput();
            var unconfirmed = new UTXOChangeWithSpentOutput();
            confirmed.SpentOutPoint = new List<OutPoint>() { OutPoint.Zero };
            var coinBaseTx =
                Transaction.Parse(
                    "020000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff0401750101ffffffff0200f2052a0100000017a914d4bb8bf5f987cd463a2f5e6e4f04618c7aaed1b5870000000000000000266a24aa21a9ede2f61c3f71d1defd3fa999dfa36953755c690689799962b48bebd836974e8cf90120000000000000000000000000000000000000000000000000000000000000000000000000", network.NBitcoinNetwork);
            confirmed.UTXO =  new List<UTXOChangeWithMetadata>() { new UTXOChangeWithMetadata(new UTXO(new NBXplorer.Models.UTXO(coinBaseTx.Outputs.AsCoins().First())), UTXOKind.UserDeposit, new AddressTrackedSource(coinBaseTx.Outputs[0].ScriptPubKey.GetDestinationAddress(network.NBitcoinNetwork))) };
            res.Confirmed = confirmed;
            res.UnConfirmed = unconfirmed;
            return Task.FromResult(res);
        }

        public Task TrackSpendableOutput(NRustLightningNetwork network, SpendableOutputDescriptor desc,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}