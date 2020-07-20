using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        ValueTask<DerivationStrategyBase> GetOurDerivationStrategyAsync(NRustLightningNetwork network, CancellationToken ct = default);
        Task<Transaction> GetSendingTxAsync(BitcoinAddress destination, Money amount, NRustLightningNetwork network, CancellationToken cancellationToken = default);
        Task<Money> GetBalanceAsync(NRustLightningNetwork network, CancellationToken ct = default);
        Task<BitcoinAddress> GetNewAddressAsync(NRustLightningNetwork network, CancellationToken ct = default);
        Task BroadcastAsync(Transaction tx, NRustLightningNetwork network);
    }

    public class WalletService : IWalletService
    {
        private readonly IKeysRepository _keysRepository;
        private readonly INBXplorerClientProvider _nbXplorerClientProvider;
        private readonly ILogger<WalletService> _logger;
        private readonly ConcurrentDictionary<NRustLightningNetwork, BitcoinExtKey> BaseXPrivs = new ConcurrentDictionary<NRustLightningNetwork, BitcoinExtKey>();
        private readonly ConcurrentDictionary<NRustLightningNetwork, DerivationStrategyBase> DerivationStrategyBaseCache = new ConcurrentDictionary<NRustLightningNetwork, DerivationStrategyBase>();
        
        /// <summary>
        /// To prevent using same outpoint when we try to create more than one channel in a short timespan, we want to
        /// hold the information about which outpoint has already been used (but not actually broadcasted.)
        /// </summary>
        private SemaphoreSlim _outpointAssumedAsSpentLock = new SemaphoreSlim(1, 1);
        private readonly FixedSizeQueue<OutPoint> _outpointAssumedAsSpent  = new FixedSizeQueue<OutPoint>(30);

        /// <summary>
        /// Service for handling on-chain balance
        /// </summary>
        /// <param name="keysRepository"></param>
        /// <param name="nbXplorerClientProvider"></param>
        public WalletService(IKeysRepository keysRepository, INBXplorerClientProvider nbXplorerClientProvider, ILogger<WalletService> logger)
        {
            _keysRepository = keysRepository;
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _logger = logger;
        }
        
        private BitcoinExtKey GetBaseXPriv(NRustLightningNetwork network)
        {
            BitcoinExtKey baseKey;
            if (!BaseXPrivs.TryGetValue(network, out baseKey))
            {
                baseKey = new BitcoinExtKey(new ExtKey(_keysRepository.GetNodeSecret().ToBytes()).Derive(network.BaseKeyPath), network.NBitcoinNetwork);
                BaseXPrivs.TryAdd(network, baseKey);
            }
            return baseKey;
        }

        public async ValueTask<DerivationStrategyBase> GetOurDerivationStrategyAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            DerivationStrategyBase strategy;
            if (DerivationStrategyBaseCache.TryGetValue(network, out strategy))
            {
                return strategy;
            }
            var baseKey = GetBaseXPriv(network);
            strategy = network.NbXplorerNetwork.DerivationStrategyFactory.CreateDirectDerivationStrategy(baseKey.Neuter(),
                new DerivationStrategyOptions
                {
                    ScriptPubKeyType = ScriptPubKeyType.Segwit,
                });

            _logger.LogInformation($"Tracking new xpub ({strategy}) for {network.CryptoCode} with nbxplorer");
            var cli = _nbXplorerClientProvider.GetClient(network);
            await cli.TrackAsync(strategy, ct);
            DerivationStrategyBaseCache[network] = strategy;
            return strategy;
        }

        public async Task<Transaction> GetSendingTxAsync(BitcoinAddress destination, Money amount, NRustLightningNetwork network, CancellationToken cancellationToken = default)
        {
            var deriv = await GetOurDerivationStrategyAsync(network, cancellationToken).ConfigureAwait(false);
            var nbXplorerClient = _nbXplorerClientProvider.GetClient(network);
            await _outpointAssumedAsSpentLock.WaitAsync(cancellationToken);
            try
            {
                var req = new CreatePSBTRequest()
                {
                    Destinations =
                        new[]
                        {
                            new CreatePSBTDestination
                            {
                                Amount = amount,
                                Destination = destination
                            },
                        }.ToList(),
                    ExcludeOutpoints = _outpointAssumedAsSpent.ToList()
                };
                var psbtResponse = await nbXplorerClient.CreatePSBTAsync(deriv, req, cancellationToken)
                    .ConfigureAwait(false);
                var psbt = SignPSBT(psbtResponse.PSBT, network);
                if (!psbt.IsAllFinalized())
                {
                    psbt.Finalize();
                }

                var tx = psbt.ExtractTransaction();
                foreach (var prevOut in tx.Inputs.Select(txIn => txIn.PrevOut))
                {
                    _outpointAssumedAsSpent.Enqueue(prevOut);
                }
                return tx;
            }
            finally
            {
                _outpointAssumedAsSpentLock.Release();
            }
        }

        public async Task<Money> GetBalanceAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var cli = _nbXplorerClientProvider.GetClient(network);
            var derivationScheme = await GetOurDerivationStrategyAsync(network, ct);
            var b = await cli.GetBalanceAsync(derivationScheme, ct);
            if (b.Total is Money m)
                return m;
            throw new NRustLightningException($"Unexpected money type {b.Total.GetType().Name}");
        }

        public async Task<BitcoinAddress> GetNewAddressAsync(NRustLightningNetwork network, CancellationToken ct = default)
        {
            var cli = _nbXplorerClientProvider.GetClient(network);
            var deriv = await GetOurDerivationStrategyAsync(network, ct);
            var a = await cli.GetUnusedAsync(deriv, DerivationFeature.Deposit, 0, false, ct);
            return a.Address;
        }

        public async Task BroadcastAsync(Transaction tx, NRustLightningNetwork n)
        {
            await _nbXplorerClientProvider.GetClient(n).BroadcastAsync(tx);
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