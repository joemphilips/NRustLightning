using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Entities;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using RustLightningTypes;

namespace NRustLightning.Server.Services
{
    public class InvoiceService
    {
        private readonly IRepository _repository;
        private readonly EventAggregator _eventAggregator;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IOptions<Config> _config;

        public InvoiceService(IRepository repository, EventAggregator eventAggregator, PeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider, ILogger<InvoiceService> logger, IOptions<Config> config)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
            _logger = logger;
            _config = config;
        }
        
        public async Task<(PaymentReceivedType, LNMoney)> PaymentReceived(Primitives.PaymentHash paymentHash, LNMoney amount, uint256? secret = null)
        {
            var invoice = await _repository.GetInvoice(paymentHash);
            if (invoice != null)
            {
                if (invoice.AmountValue is null)
                {
                    return (PaymentReceivedType.Ok, amount);
                }

                var intendedAmount = invoice.AmountValue.Value;
                if (amount.MilliSatoshi < intendedAmount.MilliSatoshi)
                {
                    return (PaymentReceivedType.AmountTooLow, intendedAmount);
                }

                if (intendedAmount.MilliSatoshi * 2 < amount.MilliSatoshi)
                    return (PaymentReceivedType.AmountTooHigh, intendedAmount);

                return (PaymentReceivedType.Ok, intendedAmount);
            }
            return (PaymentReceivedType.UnknownPaymentHash, amount);
        }

        public async Task PayInvoice(PaymentRequest invoice, long? amountMSat = null, CancellationToken ct = default)
        {
            var amount = invoice.AmountValue?.Value.MilliSatoshi ?? amountMSat;
            if (amount is null)
                throw new NRustLightningException($"You must specify payment amount if it is not included in invoice");
            
            var n = _networkProvider.TryGetByInvoice(invoice);
            if (n is null)
            {
                throw new NRustLightningException($"Unknown invoice prefix {invoice.PrefixValue}");
            }
            var peerMan = _peerManagerProvider.GetPeerManager(n);
            var failureTcs = new TaskCompletionSource<Event>();
            var successTcs = new TaskCompletionSource<Event>();
            _eventAggregator.Subscribe<Event>(e =>
            {
                if (e is Event.PaymentFailed paymentFailed && paymentFailed.Item.PaymentHash.Equals(invoice.PaymentHash))
                {
                    _logger.LogError($"Payment for invoice ({invoice}) failed");
                    failureTcs.SetResult(paymentFailed);
                }

                if (e is Event.PaymentSent paymentSent && paymentSent.Item.Hash.Equals(invoice.PaymentHash))
                {
                    _logger.LogInformation($"Payment for invoice ({invoice}) succeed");
                    successTcs.SetResult(paymentSent);
                }
            });
            peerMan.SendPayment(invoice.NodeIdValue.Item, invoice.PaymentHash, new List<RouteHint>(), LNMoney.MilliSatoshis(amount.Value), invoice.MinFinalCLTVExpiryDelta);
            peerMan.ProcessEvents();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_config.Value.PaymentTimeoutSec));
            var delay = Task.Delay(TimeSpan.FromMilliseconds(-1), cts.Token);
            var task =  Task.WhenAny(failureTcs.Task, successTcs.Task);
            await Task.WhenAny(task, delay);
            if (cts.IsCancellationRequested)
            {
                throw new NRustLightningException($"Payment for {invoice} did not finish in {_config.Value.PaymentTimeoutSec} seconds");
            }
            var resultEvent = await await task;
            if (resultEvent is Event.PaymentFailed  paymentFailed)
            {
                if (paymentFailed.Item.RejectedByDest)
                    throw new NRustLightningException($"Failed to pay! rejected by destination");
                throw new NRustLightningException($"Failed to pay!");
            }

            if (resultEvent is Event.PaymentSent paymentSent)
            {
                await _repository.SetPreimage(paymentSent.Item);
            }
        }
    }
}