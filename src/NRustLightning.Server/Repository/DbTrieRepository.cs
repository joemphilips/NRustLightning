using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DBTrie;
using DBTrie.Storage.Cache;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Adaptors;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Extensions;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Repository
{
    public class DbTrieRepository : IRepository, IAsyncDisposable
    {
        private readonly string _dbPath;
        private IKeysRepository _keysRepository;
        private ISystemClock _systemClock;
        private NRustLightningNetworkProvider _networkProvider;
        private ILogger<DbTrieRepository> _logger;
        private DBTrieEngine _engine;
        private MemoryPool<byte> _pool;

        public DbTrieRepository(IOptions<Config> conf, IKeysRepository keysRepository, ISystemClock systemClock, NRustLightningNetworkProvider networkProvider, ILogger<DbTrieRepository> logger)
        {
            _dbPath = conf.Value.InvoiceDBFilePath;
            _keysRepository = keysRepository;
            _systemClock = systemClock;
            _networkProvider = networkProvider;
            _logger = logger;
            _engine = DBTrieEngine.OpenFromFolder(_dbPath).Result;
            _engine.ConfigurePagePool(new PagePool(pageSize: conf.Value.DBCacheMB));
            _pool = MemoryPool<byte>.Shared;
        }
        
        public async Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction();
            await tx.GetTable(DBKeys.HashToPreimage).Insert(paymentPreimage.Hash.ToBytes(false), paymentPreimage.ToByteArray());
            await tx.Commit();
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
            
            using var tx = await _engine.OpenTransaction();
            var table = tx.GetTable(DBKeys.HashToPreimage);
            await table.Insert(paymentHash.ToBytes(false), paymentPreimage.ToByteArray());
            var table2 = tx.GetTable(DBKeys.HashToInvoice);
            await table2.Insert(paymentHash.ToBytes(false).ToHexString(), r.ResultValue.ToString());
            await tx.Commit();
            
            return r.ResultValue;
        }

        public async ValueTask DisposeAsync()
        {
            await _engine.DisposeAsync();
        }


        public async Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            using var preimageRow = await tx.GetTable(DBKeys.HashToPreimage).Get(hash.ToBytes(false));
            return Primitives.PaymentPreimage.Create((await preimageRow.ReadValue()).ToArray());
        }

        public async Task SetChannelManager(ChannelManager channelManager, CancellationToken ct = default)
        {
            var b = channelManager.Serialize(_pool);
            using var tx = await _engine.OpenTransaction(ct);
            var table = tx.GetTable(DBKeys.ChannelManager);
            await table.Insert(DBKeys.ChannelManagerVersion, b);
            await tx.Commit();
        }

        public async Task<ChannelManager?> GetChannelManager(ChannelManagerReadArgs readArgs, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            using var chanManRow = await tx.GetTable(DBKeys.ChannelManager).Get(DBKeys.ChannelManagerVersion);
            var val = await chanManRow.ReadValue();
            return val.IsEmpty ? null : ChannelManager.Deserialize(val.Span, readArgs);
        }

        public Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task SetInvoice(PaymentRequest paymentRequest, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

    }
}