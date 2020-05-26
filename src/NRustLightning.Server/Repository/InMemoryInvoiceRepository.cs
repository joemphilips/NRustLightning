using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Server.Entities;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Repository
{
    public class InMemoryInvoiceRepository : IInvoiceRepository
    {
        private readonly IKeysRepository _keysRepository;
        private readonly ConcurrentDictionary<Primitives.PaymentHash, (PaymentRequest, Primitives.PaymentPreimage)> _paymentRequests = new ConcurrentDictionary<Primitives.PaymentHash,(PaymentRequest, Primitives.PaymentPreimage)>();

        public InMemoryInvoiceRepository(IKeysRepository keysRepository)
        {
            _keysRepository = keysRepository;
        }
        
        public Task PaymentStarted(PaymentRequest bolt11)
        {
            throw new System.NotImplementedException();
        }

        public Task PaymentFinished(Primitives.PaymentPreimage paymentPreimage)
        {
            throw new System.NotImplementedException();
        }

        public PaymentRequest GetNewInvoice(NRustLightningNetwork network, Primitives.PaymentPreimage paymentPreimage,
            InvoiceCreationOption option)
        {
            if (network == null) throw new ArgumentNullException(nameof(network));
            if (paymentPreimage == null) throw new ArgumentNullException(nameof(paymentPreimage));

            var nodeId = Primitives.NodeId.NewNodeId(_keysRepository.GetNodeId());
            var taggedFields =
                new List<TaggedField>
                {
                    TaggedField.NewPaymentHashTaggedField(paymentPreimage.Hash),
                    TaggedField.NewNodeIdTaggedField(nodeId),
                    (option.EncodeDescriptionWithHash.HasValue && option.EncodeDescriptionWithHash.Value) ?
                        TaggedField.NewDescriptionHashTaggedField(new uint256(Hashes.SHA256(Encoding.UTF8.GetBytes(option.Description)))) :
                        TaggedField.NewDescriptionTaggedField(option.Description),
                };
            if (option.PaymentSecret != null)
            {
                taggedFields.Add(TaggedField.NewPaymentSecretTaggedField(option.PaymentSecret));
            }

            var t = new TaggedFields(taggedFields.ToFSharpList());
            var r = PaymentRequest.TryCreate(network.BOLT11InvoicePrefix,  option.Amount.ToFSharpOption(), DateTimeOffset.Now, nodeId, t);
            if (r.IsError)
            {
                throw new InvalidDataException($"Error when creating our payment request: {r.ErrorValue}");
            }

            _paymentRequests.TryAdd(paymentPreimage.Hash, (r.ResultValue, paymentPreimage));
            return r.ResultValue;
        }

        public (PaymentReceivedType, LNMoney) PaymentReceived(Primitives.PaymentHash paymentHash, LNMoney amount, uint256? secret = null)
        {
            if (_paymentRequests.TryGetValue(paymentHash, out var item))
            {
                var (req, _preImage) = item;
                if (req.AmountValue.ToNullable() is null)
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

        public Primitives.PaymentPreimage GetPreimage(Primitives.PaymentHash hash)
        {
            return _paymentRequests[hash].Item2;
        }
    }
}