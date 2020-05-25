using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NRustLightning.RustLightningTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.Altcoins.Elements;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.P2P;
using NRustLightning.Server.Entities;

namespace NRustLightning.Server.Services
{
    public class RustLightningEventReactor : IHostedService
    {
        private readonly P2PConnectionHandler _connectionHandler;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NBXplorerClientProvider _nbXplorerClientProvider;
        private readonly WalletService _walletService;
        private readonly PeerManager _peerManager;
        private readonly MemoryPool<byte> _pool;
        private readonly NRustLightningNetwork _network;
        private readonly ILogger<RustLightningEventReactor> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        private readonly Dictionary<uint256, Transaction> _pendingFundingTx = new Dictionary<uint256, Transaction>();
        private ExplorerClient _nbXplorerClient;
        private Random _random  = new Random();

        public RustLightningEventReactor(
            P2PConnectionHandler connectionHandler,
            PeerManagerProvider peerManagerProvider,
            NBXplorerClientProvider nbXplorerClientProvider,
            WalletService walletService,
            NRustLightningNetwork network,
            ILogger<RustLightningEventReactor> logger,
            IInvoiceRepository invoiceRepository)
        {
            _pool = MemoryPool<byte>.Shared;
            _connectionHandler = connectionHandler;
            _peerManagerProvider = peerManagerProvider;
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _nbXplorerClient = nbXplorerClientProvider.GetClient(_network);
            _walletService = walletService;
            _network = network;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
            _peerManager = peerManagerProvider.GetPeerManager("BTC");
        }
        
        private async Task HandleEvent(Event e, CancellationToken cancellationToken)
        {
            var chanMan = _peerManager.ChannelManager;
            if (e is Event.FundingGenerationReady f) { _logger.LogTrace($"Funding generation ready {f.Item}"); var deriv = _walletService.GetOurDerivationStrategy(_network); var req = new CreatePSBTRequest() {
                    Destinations = new List<CreatePSBTDestination>(new []
                    {
                        new CreatePSBTDestination
                        {
                            Amount = f.Item.ChannelValueSatoshis,
                            Destination = f.Item.OutputScript.GetWitScriptAddress(_network.NBitcoinNetwork),
                        },
                    })
                };
                var psbtResponse = await _nbXplorerClient.CreatePSBTAsync(deriv, req, cancellationToken).ConfigureAwait(false);
                var psbt = _walletService.SignPSBT(psbtResponse.PSBT, _network);
                if (!psbt.IsAllFinalized())
                {
                    psbt.Finalize();
                }

                var tx = psbt.ExtractTransaction();
                var nOut =
                    tx.Outputs.Count == 1 ?
                        0 :
                    tx.Outputs[0].ScriptPubKey.GetWitScriptAddress(_network.NBitcoinNetwork) !=
                           psbtResponse.ChangeAddress
                    ? 0
                    : 1;
                var fundingTxo = new OutPoint(tx.GetHash(), nOut);
                if (_pendingFundingTx.TryAdd(tx.GetHash(), tx))
                {
                    chanMan.FundingTransactionGenerated(f.Item.TemporaryChannelId.Value.ToBytes(), fundingTxo);
                }
            }
            else if (e is Event.FundingBroadcastSafe fundingBroadcastSafe)
            {
                if (!_pendingFundingTx.TryGetValue(fundingBroadcastSafe.Item.OutPoint.Item.Hash, out var fundingTx))
                {
                    _logger.LogCritical($"RL asked us to broadcast unknown funding tx! this should never happen.");
                    return;
                }
                await _nbXplorerClient.BroadcastAsync(fundingTx).ConfigureAwait(false);
            }
            else if (e is Event.PaymentReceived paymentReceived)
            {
                var a = paymentReceived.Item.Amount;
                var hash = paymentReceived.Item.PaymentHash;
                var secret = paymentReceived.Item.PaymentSecret.GetOrDefault();
                var (result, intendedAmount) = _invoiceRepository.PaymentReceived(hash,a,secret);
                if (result == PaymentReceivedType.UnknownPaymentHash)
                {
                    _logger.LogError($"Received payment for unknown payment_hash({hash}). ignoring.");
                    return;
                }
                var preImage = _invoiceRepository.GetPreimage(hash);
                if (result == PaymentReceivedType.Ok)
                {
                    chanMan.ClaimFunds(preImage, secret, (ulong)intendedAmount.MilliSatoshi);
                }
                chanMan.FailHTLCBackwards(hash, preImage);
            }
            else if (e is Event.PaymentSent paymentSent)
            {
                await _invoiceRepository.PaymentFinished(paymentSent.Item);
            }
            else if (e is Event.PaymentFailed paymentFailed)
            {
                _logger.LogInformation($"payment failed: {paymentFailed.Item.PaymentHash}" + (paymentFailed.Item.RejectedByDest ? "rejected by destination" : ""));
            }
            else if (e is Event.PendingHTLCsForwardable pendingHtlCsForwardable)
            {
                var wait = pendingHtlCsForwardable.Item.TimeToWait(_random);
                await Task.Delay(wait);
                _peerManager.ChannelManager.ProcessPendingHTLCForwards();
            }
            else if (e is Event.SpendableOutputs spendableOutputs)
            {
                foreach (var o in spendableOutputs.Item)
                {
                    _logger.LogDebug($"New spendable on-chain funds txid: {o.OutPoint.Item.Hash}. vout: {o.OutPoint.Item.N}");
                    // Do nothing. nbxplorer should handle it for us.
                }
            }
            throw new Exception("Unreachable!");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (await _connectionHandler.EventNotify.Reader.WaitToReadAsync(cancellationToken))
            {
                var events = _peerManager.ChannelManager.GetAndClearPendingEvents(_pool);
                // TODO: Consider using IAsyncEnumerable or Channel to speed up.
                foreach (var e in events)
                {
                    await HandleEvent(e, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}