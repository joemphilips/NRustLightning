using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using LSATAuthenticationHandler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Server.Entities;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;
using ResultUtils;

namespace NRustLightning.Server.Repository
{
    public class InMemoryInvoiceRepository : IInvoiceRepository, ILSATInvoiceProvider
    {
        private readonly IKeysRepository _keysRepository;
        private readonly ISystemClock _systemClock;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly ILogger<InMemoryInvoiceRepository> _logger;
        private readonly ConcurrentDictionary<Primitives.PaymentHash, (PaymentRequest, Primitives.PaymentPreimage)> _paymentRequests = new ConcurrentDictionary<Primitives.PaymentHash,(PaymentRequest, Primitives.PaymentPreimage)>();

        public InMemoryInvoiceRepository(IKeysRepository keysRepository, ISystemClock systemClock, NRustLightningNetworkProvider networkProvider, ILogger<InMemoryInvoiceRepository> logger)
        {
            _keysRepository = keysRepository;
            _systemClock = systemClock;
            _networkProvider = networkProvider;
            _logger = logger;
        }
        
        public Task SetPreimage(Primitives.PaymentPreimage paymentPreimage)
        {
            throw new System.NotImplementedException();
        }

        public Task<PaymentRequest> GetNewInvoice(NRustLightningNetwork network, InvoiceCreationOption option)
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
            var r = PaymentRequest.TryCreate(network.BOLT11InvoicePrefix,  option.Amount.ToFSharpOption(), _systemClock.UtcNow, nodeId, t, _keysRepository.AsMessageSigner());
            if (r.IsError)
            {
                throw new InvalidDataException($"Error when creating our payment request: {r.ErrorValue}");
            }

            _logger.LogDebug($"Publish new invoice with hash {paymentHash}");
            Debug.Assert(_paymentRequests.TryAdd(paymentHash, (r.ResultValue, paymentPreimage)));
            return Task.FromResult(r.ResultValue);
        }

        public async Task<(PaymentReceivedType, LNMoney)> PaymentReceived(Primitives.PaymentHash paymentHash, LNMoney amount, uint256? secret = null)
        {
            if (_paymentRequests.TryGetValue(paymentHash, out var item))
            {
                var (req, _preImage) = item;
                if (req.AmountValue is null)
                {
                    return (PaymentReceivedType.Ok, amount);
                }

                var intendedAmount = req.AmountValue.Value;
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

        public Task<Primitives.PaymentPreimage> GetPreimage(Primitives.PaymentHash hash)
        {
            return Task.FromResult(_paymentRequests[hash].Item2);
        }

        public Task<PaymentRequest> GetNewInvoiceAsync(LNMoney amount)
        {
            var n = _networkProvider.GetByCryptoCode("BTC");
            return GetNewInvoice(n, new InvoiceCreationOption {Amount = amount});
        }
    }
}