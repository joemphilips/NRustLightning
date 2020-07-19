using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Utils;
using RustLightningTypes;

namespace NRustLightning.Server.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly EventAggregator _eventAggregator;
        private readonly IPeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IOptions<Config> _config;

        public InvoiceService(IInvoiceRepository invoiceRepository, EventAggregator eventAggregator, IPeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider, ILogger<InvoiceService> logger, IOptions<Config> config)
        {
            _invoiceRepository = invoiceRepository;
            _eventAggregator = eventAggregator;
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
            _logger = logger;
            _config = config;
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
                    _logger.LogError($"Payment with hash ({invoice.PaymentHash}) failed");
                    failureTcs.SetResult(paymentFailed);
                }

                if (e is Event.PaymentSent paymentSent && paymentSent.Item.Hash.Equals(invoice.PaymentHash))
                {
                    _logger.LogError($"Payment with hash ({invoice.PaymentHash}) succeed");
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
                await _invoiceRepository.SetPreimage(paymentSent.Item);
            }
        }
    }
}