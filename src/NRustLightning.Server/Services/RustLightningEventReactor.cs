using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using Microsoft.Extensions.DependencyInjection;
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
    public class RustLightningEventReactors : IHostedService
    {
        public Dictionary<string, RustLightningEventReactor> Reactors = new Dictionary<string, RustLightningEventReactor>();
        public RustLightningEventReactors(NRustLightningNetworkProvider networkProvider , INBXplorerClientProvider clientProvider, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            foreach (var n in networkProvider.GetAll())
            {
                var cli = clientProvider.TryGetClient(n);
                if (cli != null) // it means we want  to support that chain.
                {
                    var reactor = new RustLightningEventReactor(
                        serviceProvider.GetRequiredService<P2PConnectionHandler>(),
                        serviceProvider.GetRequiredService<IPeerManagerProvider>(),
                        serviceProvider.GetRequiredService<INBXplorerClientProvider>(),
                        serviceProvider.GetRequiredService<IWalletService>(),
                        n,
                        loggerFactory.CreateLogger(nameof(RustLightningEventReactor)+ $":{n.CryptoCode}"),
                        serviceProvider.GetRequiredService<IInvoiceRepository>()
                    );
                    Reactors.Add(n.CryptoCode, reactor);
                }
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Reactors.Values.Select(r => r.StartAsync(cancellationToken)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Reactors.Values.Select(r => r.StopAsync(cancellationToken)));
        }
    }
    
    public class RustLightningEventReactor : BackgroundService
    {
        private readonly P2PConnectionHandler _connectionHandler;
        private readonly IPeerManagerProvider _peerManagerProvider;
        private readonly IWalletService _walletService;
        private readonly PeerManager _peerManager;
        private readonly MemoryPool<byte> _pool;
        private readonly NRustLightningNetwork _network;
        private readonly ILogger _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        private readonly Dictionary<uint256, Transaction> _pendingFundingTx = new Dictionary<uint256, Transaction>();
        private ExplorerClient _nbXplorerClient;
        private Random _random  = new Random();

        public RustLightningEventReactor(
            P2PConnectionHandler connectionHandler,
            IPeerManagerProvider peerManagerProvider,
            INBXplorerClientProvider nbXplorerClientProvider,
            IWalletService walletService,
            NRustLightningNetwork network,
            ILogger logger,
            IInvoiceRepository invoiceRepository)
        {
            _pool = MemoryPool<byte>.Shared;
            _connectionHandler = connectionHandler;
            _peerManagerProvider = peerManagerProvider;
            _nbXplorerClient = nbXplorerClientProvider.GetClient(network);
            _walletService = walletService;
            _network = network;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
            _peerManager = peerManagerProvider.GetPeerManager(network.CryptoCode);
        }
        
        private async Task HandleEvent(Event e, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handling event {e}");
            var chanMan = _peerManager.ChannelManager;
            if (e is Event.FundingGenerationReady f) {
                var outputAddress = f.Item.OutputScript.GetWitScriptAddress(_network.NBitcoinNetwork);
                var tx = await _walletService.GetSendingTxAsync(outputAddress, f.Item.ChannelValueSatoshis, _network, cancellationToken);
                var nOut =
                        tx.Outputs.Select((txo, i) => new {Index = i, Spk = txo.ScriptPubKey} )
                        .First((item) => item.Spk == outputAddress.ScriptPubKey).Index;
                var fundingTxo = new OutPoint(tx.GetHash(), nOut);
                Debug.Assert(_pendingFundingTx.TryAdd(tx.GetHash(), tx));
                _logger.LogDebug($"Finished creating funding tx {tx.ToHex()}; id: {tx.GetHash()}");
                
                chanMan.FundingTransactionGenerated(f.Item.TemporaryChannelId.Value, fundingTxo);
            }
            else if (e is Event.FundingBroadcastSafe fundingBroadcastSafe)
            {
                var h = fundingBroadcastSafe.Item.OutPoint.Item.Hash;
                if (!_pendingFundingTx.TryGetValue(h, out var fundingTx))
                {
                    _logger.LogCritical($"RL asked us to broadcast unknown funding tx for id: ({h})! this should never happen.");
                    return;
                }
                await _nbXplorerClient.BroadcastAsync(fundingTx, cancellationToken).ConfigureAwait(false);
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
                await Task.Delay(wait, cancellationToken);
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
            else
            {
                throw new Exception("Unreachable!");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting event reactor for {this._network.CryptoCode}");
            try
            {
                while (await _connectionHandler.EventNotify.Reader.WaitToReadAsync(cancellationToken))
                {
                    var _ = await _connectionHandler.EventNotify.Reader.ReadAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var events = _peerManager.ChannelManager.GetAndClearPendingEvents(_pool);
                    await Task.WhenAll(events.Select(async e =>
                        await HandleEvent(e, cancellationToken).ConfigureAwait(false)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message}: {ex.StackTrace}");
            }
        }
    }
}