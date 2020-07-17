using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTrie;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using LSATAuthenticationHandler;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Entities;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Utils;
using ResultUtils;

namespace NRustLightning.Server.Repository
{
    public class DBTrieInvoiceRepository : IInvoiceRepository, IAsyncDisposable
    {
        private readonly string _dbPath;
        private IKeysRepository _keysRepository;
        private ISystemClock _systemClock;
        private NRustLightningNetworkProvider _networkProvider;
        private ILogger<DBTrieInvoiceRepository> _logger;
        private DBTrieEngine? _engine;

        public DBTrieInvoiceRepository(IOptions<Config> conf, IKeysRepository keysRepository, ISystemClock systemClock, NRustLightningNetworkProvider networkProvider, ILogger<DBTrieInvoiceRepository> logger)
        {
            _dbPath = conf.Value.InvoiceDBFilePath;
            _keysRepository = keysRepository;
            _systemClock = systemClock;
            _networkProvider = networkProvider;
            _logger = logger;
        }
        
        public Task PaymentStarted(PaymentRequest bolt11)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPreimage(Primitives.PaymentPreimage paymentPreimage)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PaymentRequest> GetNewInvoice(NRustLightningNetwork network, InvoiceCreationOption option)
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
            
            _engine ??= await DBTrieEngine.OpenFromFolder(_dbPath);
            var tx = await _engine.OpenTransaction();
            var table = tx.GetTable("hash-preimage");
            await table.Insert(paymentHash.ToBytes(false), paymentPreimage.ToByteArray());
            var table2 = tx.GetTable("hash-invoice");
            await table2.Insert(paymentHash.ToBytes(false).ToHexString(), r.ResultValue.ToString());
            await tx.Commit();
            
            return r.ResultValue;
        }

        public async ValueTask DisposeAsync()
        {
            if (_engine != null) await _engine.DisposeAsync();
        }

        public async Task<(PaymentReceivedType, LNMoney)> PaymentReceived(Primitives.PaymentHash paymentHash, LNMoney amount, uint256? secret = null)
        {
            _engine ??= await DBTrieEngine.OpenFromFolder(_dbPath);
            var tx = await _engine.OpenTransaction();
            var invoiceRow = await tx.GetTable("hash-invoice").Get(paymentHash.ToBytes(false));
            if (invoiceRow != null)
            {
                var res = PaymentRequest.Parse(await invoiceRow.ReadValueString());
                if (res.IsError)
                    throw new NRustLightningException($"Failed to read payment request for {res.ErrorValue}");
                var req = res.ResultValue;
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

        public async Task<Primitives.PaymentPreimage> GetPreimage(Primitives.PaymentHash hash)
        {
            _engine ??= await DBTrieEngine.OpenFromFolder(_dbPath);
            var tx = await _engine.OpenTransaction();
            var preimageRow = await tx.GetTable("hash-preimage").Get(hash.ToBytes(false));
            return Primitives.PaymentPreimage.Create((await preimageRow.ReadValue()).ToArray());
        }
    }
}