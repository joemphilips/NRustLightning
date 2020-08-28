using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetLightning.Crypto;
using DotNetLightning.Utils;
using Microsoft.Extensions.DependencyInjection;
using NRustLightning.RustLightningTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.Altcoins.Elements;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Infrastructure.Entities;
using NRustLightning.Infrastructure.Extensions;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.P2P;
using NRustLightning.Utils;

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
                        serviceProvider.GetRequiredService<PeerManagerProvider>(),
                        serviceProvider.GetRequiredService<IWalletService>(),
                        n,
                        serviceProvider.GetRequiredService<EventAggregator>(),
                        loggerFactory.CreateLogger(nameof(RustLightningEventReactor)+ $":{n.CryptoCode}"),
                        serviceProvider.GetRequiredService<InvoiceService>(),
                        serviceProvider.GetRequiredService<ChannelProvider>(),
                        serviceProvider.GetRequiredService<IRepository>()
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
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly IWalletService _walletService;
        private readonly MemoryPool<byte> _pool;
        private readonly NRustLightningNetwork _network;
        private readonly EventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private readonly InvoiceService _invoiceService;
        private readonly ChannelProvider _channelProvider;
        private readonly IRepository _repository;

        private readonly Dictionary<uint256, Transaction> _pendingFundingTx = new Dictionary<uint256, Transaction>();
        private Random _random  = new Random();

        public RustLightningEventReactor(
            P2PConnectionHandler connectionHandler,
            PeerManagerProvider peerManagerProvider,
            IWalletService walletService,
            NRustLightningNetwork network,
            EventAggregator eventAggregator,
            ILogger logger,
            InvoiceService invoiceService,
            ChannelProvider channelProvider,
            IRepository repository)
        {
            _pool = MemoryPool<byte>.Shared;
            _connectionHandler = connectionHandler;
            _peerManagerProvider = peerManagerProvider;
            _walletService = walletService;
            _network = network;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _invoiceService = invoiceService;
            _channelProvider = channelProvider;
            _repository = repository;
        }
        
        private async Task HandleEvent(Event e, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handling event {e}");
            _eventAggregator.Publish(e);
            var peerManager = _peerManagerProvider.GetPeerManager(_network);
            var chanMan = peerManager.ChannelManager;
            if (e is Event.FundingGenerationReady f)
            {
                var witScriptId = PayToWitScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(f.Item.OutputScript) ?? Infrastructure.Utils.Utils.Fail<WitScriptId>($"Failed to extract wit script from {f.Item.OutputScript.ToHex()}! this should never happen.");
                var outputAddress = witScriptId.GetAddress(_network.NBitcoinNetwork);
                var tx = await _walletService.GetSendingTxAsync(outputAddress, f.Item.ChannelValueSatoshis, _network, cancellationToken);
                var nOut =
                        tx.Outputs.Select((txo, i) => (txo, i))
                        .First(item => item.txo.ScriptPubKey == outputAddress.ScriptPubKey).i;
                var fundingTxo = new OutPoint(tx.GetHash(), nOut);
                Debug.Assert(_pendingFundingTx.TryAdd(tx.GetHash(), tx));
                _logger.LogDebug($"Finished creating funding tx {tx.ToHex()}; id: {tx.GetHash()}");
                
                chanMan.FundingTransactionGenerated(f.Item.TemporaryChannelId.Value, fundingTxo);
                peerManager.ProcessEvents();
            }
            else if (e is Event.FundingBroadcastSafe fundingBroadcastSafe)
            {
                var h = fundingBroadcastSafe.Item.OutPoint.Item.Hash;
                if (!_pendingFundingTx.TryGetValue(h, out var fundingTx))
                {
                    _logger.LogCritical($"RL asked us to broadcast unknown funding tx for id: ({h})! this should never happen.");
                    return;
                }
                await _walletService.BroadcastAsync(fundingTx, _network).ConfigureAwait(false);
            }
            else if (e is Event.PaymentReceived paymentReceived)
            {
                var a = paymentReceived.Item.Amount;
                var hash = paymentReceived.Item.PaymentHash;
                var secret = paymentReceived.Item.PaymentSecret.GetOrDefault();
                var (result, intendedAmount) = await _invoiceService.PaymentReceived(hash,a,secret);
                _logger.LogDebug($"Received payment of type {result}. Payment hash {hash}. PaymentSecret: {secret}");
                if (result == PaymentReceivedType.UnknownPaymentHash)
                {
                    _logger.LogError($"Received payment for unknown payment_hash({hash}). ignoring.");
                    return;
                }
                var preImage = await _repository.GetPreimage(hash);
                _logger.LogDebug($"preimage {preImage.ToHex()}");
                if (result == PaymentReceivedType.Ok)
                {
                    if (chanMan.ClaimFunds(preImage, secret, (ulong) intendedAmount.MilliSatoshi))
                    {
                        peerManager.ProcessEvents();
                    }
                }
                else
                {
                    if (chanMan.FailHTLCBackwards(hash, secret))
                    {
                        peerManager.ProcessEvents();
                    }
                    else
                    {
                        _logger.LogCritical($"RL told us that preimage ({preImage}) and/or its hash ({hash}) is unknown to us. This should never happen");
                    }
                }
            }
            else if (e is Event.PaymentSent paymentSent)
            {
                await _repository.SetPreimage(paymentSent.Item);
            }
            else if (e is Event.PaymentFailed paymentFailed)
            {
                _logger.LogInformation($"payment failed: {paymentFailed.Item.PaymentHash}" + (paymentFailed.Item.RejectedByDest ? "rejected by destination" : ""));
            }
            else if (e is Event.PendingHTLCsForwardable pendingHtlCsForwardable)
            {
#if DEBUG
                var wait = TimeSpan.Zero;
#else
                var wait = pendingHtlCsForwardable.Item.TimeToWait(_random);
#endif
                await Task.Delay(wait, cancellationToken);
                peerManager.ChannelManager.ProcessPendingHTLCForwards();
                peerManager.ProcessEvents();
            }
            else if (e is Event.SpendableOutputs spendableOutputs)
            {
                var chan = _channelProvider.GetSpendableOutputDescriptorChannel(_network.CryptoCode);
                foreach (var o in spendableOutputs.Item)
                {
                    _logger.LogInformation($"New spendable on-chain funds txid: {o.OutPoint.Item.Hash}. vout: {o.OutPoint.Item.N}");
                    await chan.Writer.WriteAsync(o, cancellationToken);
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
                    foreach (var (peerManager, manyChannelMonitor) in _peerManagerProvider.GetAllWithManyChannelMonitors())
                    {
                        var _ = await _connectionHandler.EventNotify.Reader.ReadAsync(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                        var e1 = peerManager.ChannelManager.GetAndClearPendingEvents(_pool);
                        var e2 = manyChannelMonitor.GetAndClearPendingEvents(_pool);
                        var events = e1.Concat(e2);
                        await Task.WhenAll(events.Select(async e =>
                            await HandleEvent(e, cancellationToken).ConfigureAwait(false)));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message}: {ex.StackTrace}");
            }
        }
    }
}