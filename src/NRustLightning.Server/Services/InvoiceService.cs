using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Entities;
using NRustLightning.Infrastructure.Extensions;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
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
        private readonly IKeysRepository _keysRepository;

        public InvoiceService(IRepository repository, EventAggregator eventAggregator, PeerManagerProvider peerManagerProvider, NRustLightningNetworkProvider networkProvider, ILogger<InvoiceService> logger, IOptions<Config> config, IKeysRepository keysRepository)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
            _logger = logger;
            _config = config;
            _keysRepository = keysRepository;
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

        public async Task<PaymentRequest> GetNewInvoice(NRustLightningNetwork network, InvoiceCreationOption option, CancellationToken ct = default)
        {
            if (network == null) throw new ArgumentNullException(nameof(network));
            
            Primitives.PaymentPreimage paymentPreimage = Primitives.PaymentPreimage.Create(RandomUtils.GetBytes(32));

            var nodeId = Primitives.NodeId.NewNodeId(_keysRepository.GetNodeId());
            var paymentHash = paymentPreimage.Hash;
            var taggedFields =
                new List<TaggedField>
                {
                    TaggedField.NewPaymentHashTaggedField(paymentHash),
                    TaggedField.NewNodeIdTaggedField(nodeId),
                    (option.EncodeDescriptionWithHash.HasValue && option.EncodeDescriptionWithHash.Value) ?
                        TaggedField.NewDescriptionHashTaggedField(new uint256(Hashes.SHA256(Encoding.UTF8.GetBytes(option.Description)), false)) :
                        TaggedField.NewDescriptionTaggedField(option.Description),
                };
            if (option.PaymentSecret != null)
            {
                taggedFields.Add(TaggedField.NewPaymentSecretTaggedField(option.PaymentSecret));
            }

            var t = new TaggedFields(taggedFields.ToFSharpList());
            var r = PaymentRequest.TryCreate(network.BOLT11InvoicePrefix,  option.Amount.ToFSharpOption(), DateTimeOffset.UtcNow, nodeId, t, _keysRepository.AsMessageSigner());
            if (r.IsError)
            {
                throw new InvalidDataException($"Error when creating our payment request: {r.ErrorValue}");
            }

            _logger.LogDebug($"Publish new invoice with hash {paymentHash}");
            await _repository.SetInvoice(r.ResultValue, ct);
            await _repository.SetPreimage(paymentPreimage, ct);
            return r.ResultValue;
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