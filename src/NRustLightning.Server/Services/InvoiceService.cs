using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NRustLightning.RustLightningTypes;
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

        public InvoiceService(IInvoiceRepository invoiceRepository, EventAggregator eventAggregator, IPeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider)
        {
            _invoiceRepository = invoiceRepository;
            _eventAggregator = eventAggregator;
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
        }

        public async Task PayInvoice(PaymentRequest invoice, long? amountMSat = null)
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
                if (e is Event.PaymentFailed paymentFailed)
                    failureTcs.SetResult(paymentFailed);
                if (e is Event.PaymentSent paymentSent)
                    successTcs.SetResult(paymentSent);
            });
            peerMan.SendPayment(invoice.NodeIdValue.Item, invoice.PaymentHash, new List<RouteHint>(), LNMoney.MilliSatoshis(amount.Value), invoice.MinFinalCLTVExpiryDelta);
            var resultEvent = await await Task.WhenAny(failureTcs.Task, successTcs.Task);
            if (resultEvent is Event.PaymentFailed  paymentFailed)
            {
                if (paymentFailed.Item.RejectedByDest)
                    throw new NRustLightningException($"Failed to pay!");
            }

            if (resultEvent is Event.PaymentSent paymentSent)
            {
                await _invoiceRepository.SetPreimage(paymentSent.Item);
            }
        }
    }
}