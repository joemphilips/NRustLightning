using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Crypto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;
using NBitcoin;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Interfaces;

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
        private readonly RepositoryProvider _repositoryProvider;
        private readonly ConcurrentDictionary<NRustLightningNetwork, BitcoinExtKey> BaseXPrivs = new ConcurrentDictionary<NRustLightningNetwork, BitcoinExtKey>();
        private readonly ConcurrentDictionary<NRustLightningNetwork, DerivationStrategyBase> DerivationStrategyBaseCache = new ConcurrentDictionary<NRustLightningNetwork, DerivationStrategyBase>();
        
        /// <summary>
        /// To prevent using same outpoint when we try to create more than one channel in a short timespan, we want to
        /// hold the information about which outpoint has already been used (but not actually broadcasted.)
        /// </summary>
        private SemaphoreSlim _outpointAssumedAsSpentLock = new SemaphoreSlim(1, 1);
        private readonly FixedSizeQueue<OutPoint> _outpointAssumedAsSpent  = new FixedSizeQueue<OutPoint>(30);
        private readonly ISecp256k1 _secp256k1Ctx = CryptoUtils.impl.newSecp256k1();

        /// <summary>
        /// Service for handling on-chain balance
        /// </summary>
        /// <param name="keysRepository"></param>
        /// <param name="nbXplorerClientProvider"></param>
        public WalletService(IKeysRepository keysRepository, INBXplorerClientProvider nbXplorerClientProvider, ILogger<WalletService> logger, RepositoryProvider repositoryProvider)
        {
            _keysRepository = keysRepository;
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _logger = logger;
            _repositoryProvider = repositoryProvider;
        }
        
        private BitcoinExtKey GetBaseXPriv(NRustLightningNetwork network)
        {
            if (BaseXPrivs.TryGetValue(network, out var baseKey)) return baseKey;
            baseKey = new BitcoinExtKey(new ExtKey(_keysRepository.GetNodeSecret().ToBytes()).Derive(network.BaseKeyPath), network.NBitcoinNetwork);
            BaseXPrivs.TryAdd(network, baseKey);
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
                var psbt = await SignPSBT(psbtResponse.PSBT, network);
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
            {
                m += await GetAmountForLNOutput(network);
                return m;
            }

            throw new NRustLightningException($"Unexpected money type {b.Total.GetType().Name}");
        }

        /// <summary>
        /// Get total UTXO originated as <see cref="SpendableOutputDescriptor"/> which we currently have.
        /// TODO: Stop using Bitcoin-core RPC feature (TrackAsync and ListUnspentAsync) and batch query to nbx when https://github.com/dgarage/NBXplorer/issues/291 is ready.
        /// </summary>
        /// <returns></returns>
        private async Task<Money> GetAmountForLNOutput(NRustLightningNetwork network)
        {
            var repo = _repositoryProvider.GetRepository(network);
            var cli = _nbXplorerClientProvider.GetClient(network);
            var ourAddreses = repo.GetAllSpendableOutputDescriptors().ToEnumerable().Select(desc =>
                desc.Output.ScriptPubKey.GetDestinationAddress(network.NBitcoinNetwork)).ToArray();
            var currentHeight = await cli.RPCClient.GetBlockCountAsync();
            var coins = await cli.RPCClient.ListUnspentAsync(0, currentHeight, ourAddreses);
            return coins.Where(coin => ourAddreses.Contains(coin.Address)).Sum(x => x.Amount);
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

        /// <summary>
        /// 1. Track the output in NBXplorer
        /// 2. Track the output in bitcoin-ore
        /// 3. Save the output and its metadata to our repository
        /// </summary>
        /// <param name="network"></param>
        /// <param name="o"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SaveSpendableOutput(NRustLightningNetwork network, SpendableOutputDescriptor o, CancellationToken ct = default)
        {
            var repo = _repositoryProvider.GetRepository(network);
            await repo.SetSpendableOutputDescriptor(o, ct);
        }

        public async Task TrackSpendableOutput(NRustLightningNetwork network,
            SpendableOutputDescriptor desc, CancellationToken ct = default)
        {
            var client = _nbXplorerClientProvider.GetClient(network);
            switch (desc)
            {
                case SpendableOutputDescriptor.StaticOutput staticOutput:
                {
                    var addr = staticOutput.Item.Output.ScriptPubKey.GetDestinationAddress(network.NBitcoinNetwork);
                    var s = new AddressTrackedSource(addr);
                    await client.TrackAsync(s, ct);
                    await client.RPCClient.ImportAddressAsync(addr);
                    break;
                }
                case SpendableOutputDescriptor.DynamicOutputP2WSH p2wshOutput:
                {
                    var (param1, param2) = p2wshOutput.Item.KeyDerivationParams.ToValueTuple();
                    var amountSat = (ulong)p2wshOutput.Item.Output.Value.Satoshi;
                    var channelKeys = _keysRepository.DeriveChannelKeys(amountSat, param1, param2);
                    var delayedPaymentKey = Generators.derivePrivKey(_secp256k1Ctx, channelKeys.DelayedPaymentBaseKey, p2wshOutput.Item.PerCommitmentPoint.PubKey);
                    var toSelfDelay = p2wshOutput.Item.ToSelfDelay;
                    var revokeableRedeemScript = _keysRepository.GetRevokeableRedeemScript(p2wshOutput.Item.RemoteRevocationPubKey, toSelfDelay, delayedPaymentKey.PubKey);
                    Debug.Assert(p2wshOutput.Item.Output.ScriptPubKey.Equals(revokeableRedeemScript.WitHash.ScriptPubKey));

                    var addr =
                        revokeableRedeemScript.WitHash.ScriptPubKey.GetDestinationAddress(network.NBitcoinNetwork);
                    var s = new AddressTrackedSource(addr);
                    await client.TrackAsync(s, ct);
                    await client.RPCClient.ImportAddressAsync(addr);
                    break;
                }
                case SpendableOutputDescriptor.StaticOutputRemotePayment remoteOutput:
                {
                    var (p1, p2 ) = remoteOutput.Item.KeyDerivationParams.ToValueTuple();
                    var amountSat = (ulong) remoteOutput.Item.Output.Value;
                    var keys = _keysRepository.DeriveChannelKeys(amountSat, p1, p2);
                    Debug.Assert(
                        keys.PaymentKey.PubKey.WitHash.ScriptPubKey.Equals(remoteOutput.Item.Output.ScriptPubKey));
                    
                    var addr = remoteOutput.Item.Output.ScriptPubKey.GetDestinationAddress(network.NBitcoinNetwork);
                    await client.TrackAsync(new AddressTrackedSource(addr), ct);
                    await client.RPCClient.ImportAddressAsync(addr);
                    break;
                }
            }
        }

        
        private async Task<PSBT> SignPSBT(PSBT psbt, NRustLightningNetwork network)
        {
            var repo = _repositoryProvider.GetRepository(network);
            var outpoints = psbt.Inputs.Select(txIn => txIn.PrevOut);
            var signingKeys = new List<Key>();
            await foreach (var (desc, i) in repo.GetSpendableOutputDescriptors(outpoints).Select((x, i) => (x, i)))
            {
                if (desc is null)
                {
                    // We don't have any information in repo. This probably means the UTXO is not LN-origin
                    // (i.e. those are the funds user provided by on-chain)
                    continue;
                }
                switch (desc)
                {
                    case SpendableOutputDescriptor.StaticOutput _:
                        // signature for this input will be generated by Destination key (see below).
                        // so no need for any operation.
                        break;
                    case SpendableOutputDescriptor.DynamicOutputP2WSH p2wshOutput:
                    {
                        var (param1, param2) = p2wshOutput.Item.KeyDerivationParams.ToValueTuple();
                        var amountSat = (ulong) p2wshOutput.Item.Output.Value.Satoshi;
                        var channelKeys = _keysRepository.DeriveChannelKeys(amountSat, param1, param2);
                        var delayedPaymentKey = Generators.derivePrivKey(_secp256k1Ctx,
                            channelKeys.DelayedPaymentBaseKey, p2wshOutput.Item.PerCommitmentPoint.PubKey);
                        var toSelfDelay = p2wshOutput.Item.ToSelfDelay;
                        var revokeableRedeemScript =
                            _keysRepository.GetRevokeableRedeemScript(p2wshOutput.Item.RemoteRevocationPubKey,
                                toSelfDelay, delayedPaymentKey.PubKey);
                        Debug.Assert(
                            p2wshOutput.Item.Output.ScriptPubKey.Equals(revokeableRedeemScript.WitHash.ScriptPubKey));
                        psbt.AddScripts(revokeableRedeemScript);
                        psbt.Inputs[i].SetSequence(toSelfDelay);
                        signingKeys.Add(delayedPaymentKey);
                        break;
                    }
                    case SpendableOutputDescriptor.StaticOutputRemotePayment remoteOutput:
                    {
                        var (p1, p2 ) = remoteOutput.Item.KeyDerivationParams.ToValueTuple();
                        var amountSat = (ulong) remoteOutput.Item.Output.Value;
                        var keys = _keysRepository.DeriveChannelKeys(amountSat, p1, p2);
                        Debug.Assert(
                            keys.PaymentKey.PubKey.WitHash.ScriptPubKey.Equals(remoteOutput.Item.Output.ScriptPubKey));
                        signingKeys.Add(keys.PaymentKey);
                        break;
                    }
                    default:
                        throw new Exception($"Unreachable! Unknown output descriptor type {desc}");
                }
            }
            
            // sign for user provided on-chain funds utxos.
            if (BaseXPrivs.TryGetValue(network, out var xpriv))
            {
                psbt.SignAll(ScriptPubKeyType.Segwit, xpriv);
            }
            
            // sign for static-outputs. Which RL gave us as a result of off-chain balance handling.
            var destinationKey = _keysRepository.GetDestinationKey();
            psbt.SignWithKeys(destinationKey);
            
            // sign with other keys which we have saved in repo.
            foreach (var sk in signingKeys)
            {
                psbt.SignWithKeys(sk);
            }

            return psbt;
        }
    }
}