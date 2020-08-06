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


        public async Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            using var preimageRow = await tx.GetTable(DBKeys.HashToPreimage).Get(hash.ToBytes(false));
            return preimageRow is null ? null : Primitives.PaymentPreimage.Create((await preimageRow.ReadValue()).ToArray());
        }

        public async Task SetChannelManager(ChannelManager channelManager, CancellationToken ct = default)
        {
            if (channelManager == null) throw new ArgumentNullException(nameof(channelManager));
            var b = channelManager.Serialize(_pool);
            using var tx = await _engine.OpenTransaction(ct);
            var table = tx.GetTable(DBKeys.ChannelManager);
            await table.Insert(DBKeys.ChannelManagerVersion, b);
            await tx.Commit();
        }

        public async Task<(uint256, ChannelManager)?> GetChannelManager(ChannelManagerReadArgs readArgs, CancellationToken ct = default)
        {
            if (readArgs == null) throw new ArgumentNullException(nameof(readArgs));
            using var tx = await _engine.OpenTransaction(ct);
            using var chanManRow = await tx.GetTable(DBKeys.ChannelManager).Get(DBKeys.ChannelManagerVersion);
            if (chanManRow is null) return null;
            var val = await chanManRow.ReadValue();
            return val.IsEmpty ? default : ChannelManager.Deserialize(val, readArgs, _conf.Value.RustLightningConfig, _pool);
        }

        public async Task<(ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>)?> GetManyChannelMonitor(ManyChannelMonitorReadArgs readArgs, CancellationToken ct = default)
        {
            if (readArgs == null) throw new ArgumentNullException(nameof(readArgs));
            using var tx = await _engine.OpenTransaction(ct);
            using var manyChannelMonitorRow =
                await tx.GetTable(DBKeys.ManyChannelMonitor).Get(DBKeys.ManyChannelMonitorVersion);
            if (manyChannelMonitorRow is null) return null;
            var val = await manyChannelMonitorRow.ReadValue();
            return val.IsEmpty ? default : ManyChannelMonitor.Deserialize(readArgs, val, _pool);
        }

        public async Task SetManyChannelMonitor(ManyChannelMonitor manyChannelMonitor, CancellationToken ct = default)
        {
            if (manyChannelMonitor == null) throw new ArgumentNullException(nameof(manyChannelMonitor));
            var b = manyChannelMonitor.Serialize(_pool);
            using var tx = await _engine.OpenTransaction(ct);
            var table = tx.GetTable(DBKeys.ManyChannelMonitor);
            await table.Insert(DBKeys.ManyChannelMonitorVersion, b);
            await tx.Commit();
        }

        public async Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            using var row = await tx.GetTable(DBKeys.ChannelManager).Get(hash.ToBytes(false));
            if (row is null) return null;
            var r = PaymentRequest.Parse((await row.ReadValueString()));
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
                return await DBTrieEngine.OpenFromFolder(_dbPath);
            }
            catch when (tried < 10)
            {
                tried++;
                await Task.Delay(500, ct);
                goto retry;
            }
        }
    }
}