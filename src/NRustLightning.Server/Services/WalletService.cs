using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Utils;

namespace NRustLightning.Server.Services
{
    /// <summary>
    /// Service responsible for handling on-chain funds.
    /// Most calls will be delegated to NBXplorer. But NBXplorer has no hot wallet feature so signing
    /// must be done in this class.
    /// </summary>
    public interface IWalletService
    {
        DerivationStrategyBase GetOurDerivationStrategy(NRustLightningNetwork network);
        Task<Transaction> GetSendingTxAsync(BitcoinAddress destination, Money amount, NRustLightningNetwork network, CancellationToken cancellationToken = default);
        Task<Money> GetBalanceAsync(NRustLightningNetwork network, CancellationToken ct = default);
        Task<BitcoinAddress> GetNewAddressAsync(NRustLightningNetwork network, CancellationToken ct = default);
    }

    public class WalletService : IWalletService
    {
        private readonly IKeysRepository _keysRepository;
        private readonly INBXplorerClientProvider _nbXplorerClientProvider;
        private readonly ConcurrentDictionary<NRustLightningNetwork, ExtKey> BaseXPrivs = new ConcurrentDictionary<NRustLightningNetwork, ExtKey>();

        /// <summary>
        /// Service for handling on-chain balance
        /// </summary>
        /// <param name="keysRepository"></param>
        /// <param name="nbXplorerClientProvider"></param>
        public WalletService(IKeysRepository keysRepository, INBXplorerClientProvider nbXplorerClientProvider)
        {
            _keysRepository = keysRepository;
            _nbXplorerClientProvider = nbXplorerClientProvider;
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

        public async Task<Transaction> GetSendingTxAsync(BitcoinAddress destination, Money amount, NRustLightningNetwork network, CancellationToken cancellationToken = default)
        {
            var deriv = GetOurDerivationStrategy(network);
            var nbXplorerClient = _nbXplorerClientProvider.GetClient(network);
            var req = new CreatePSBTRequest()
            {
                Destinations =
                    new []
                    {
                        new CreatePSBTDestination
                        {
                            Amount = amount,
                            Destination = destination
                        },
                    }.ToList()
            };
            var psbtResponse = await nbXplorerClient.CreatePSBTAsync(deriv, req, cancellationToken).ConfigureAwait(false);
            var psbt = SignPSBT(psbtResponse.PSBT, network);
            if (!psbt.IsAllFinalized())
            {
                psbt.Finalize();
            }

            return psbt.ExtractTransaction();
        }

        public async Task<Money> GetBalanceAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var cli = _nbXplorerClientProvider.GetClient(network);
            var derivationScheme = GetOurDerivationStrategy(network);
            var b = await cli.GetBalanceAsync(derivationScheme, ct);
            if (b.Total is Money m)
                return m;
            throw new NRustLightningException($"Unexpected money type {b.Total.GetType().Name}");
        }

        public async Task<BitcoinAddress> GetNewAddressAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var cli = _nbXplorerClientProvider.GetClient(network);
            var deriv = GetOurDerivationStrategy(network);
            var a = await cli.GetUnusedAsync(deriv, DerivationFeature.Deposit, 0, false, ct);
            return a.Address;
        }

        private PSBT SignPSBT(PSBT psbt, NRustLightningNetwork network)
        {
            if (BaseXPrivs.TryGetValue(network, out var xpriv))
            {
                psbt.SignAll(ScriptPubKeyType.Segwit, xpriv);
            }

            return psbt;
        }
    }
}