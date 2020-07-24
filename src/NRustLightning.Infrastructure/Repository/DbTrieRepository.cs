using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DBTrie;
using DBTrie.Storage.Cache;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Crypto;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Extensions;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Infrastructure.Repository
{
    public class DbTrieRepository : IRepository
    {
        private readonly string _dbPath;
        private readonly IOptions<Config> _conf;
        private IKeysRepository _keysRepository;
        private NRustLightningNetworkProvider _networkProvider;
        private ILogger<DbTrieRepository> _logger;
        private DBTrieEngine _engine;
        private MemoryPool<byte> _pool;

        public DbTrieRepository(IOptions<Config> conf, IKeysRepository keysRepository, NRustLightningNetworkProvider networkProvider, ILogger<DbTrieRepository> logger)
        {
            _dbPath = conf.Value.InvoiceDBFilePath;
            _conf = conf;
            _keysRepository = keysRepository;
            _networkProvider = networkProvider;
            _logger = logger;
            _pool = MemoryPool<byte>.Shared;
            _engine = OpenEngine(CancellationToken.None).GetAwaiter().GetResult();
            var pageSize = 8192;
            _engine.ConfigurePagePool(new PagePool(pageSize, ( _conf.Value.DBCacheMB * 1000 * 1000) / pageSize));
        }
        
        public async Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
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
            var r = PaymentRequest.TryCreate(network.BOLT11InvoicePrefix,  option.Amount.ToFSharpOption(), DateTimeOffset.UtcNow, nodeId, t, _keysRepository.AsMessageSigner());
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

        public async Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            using var chanManRow = await tx.GetTable(DBKeys.ChannelManager).Get(hash.ToBytes(false));
            var r = PaymentRequest.Parse((await chanManRow.ReadValueString()));
            if (r.IsError)
            {
                _logger.LogError($"Failed to get invoice for hash {hash}. {r.ErrorValue}");
                return null;
            }

            return r.ResultValue;
        }

        public async Task SetInvoice(PaymentRequest paymentRequest, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            await tx.GetTable(DBKeys.HashToPreimage).Insert(paymentRequest.PaymentHash.Value.ToString(), paymentRequest.ToString());
            await tx.Commit();
        }

        private async ValueTask<DBTrieEngine> OpenEngine(CancellationToken ct)
        {
            int tried = 0;
            retry:
            try
            {
                return await DBTrie.DBTrieEngine.OpenFromFolder(_dbPath);
            }
            catch when (tried < 10)
            {
                tried++;
                await Task.Delay(500, ct);
                goto retry;
            }
            catch
            {
                throw;
            }
        }
    }
}