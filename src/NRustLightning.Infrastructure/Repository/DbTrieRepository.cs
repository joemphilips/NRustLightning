using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
using NBitcoin.DataEncoders;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Extensions;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.RustLightningTypes;
using NRustLightning.Utils;

namespace NRustLightning.Infrastructure.Repository
{
    public class DbTrieRepository : IRepository
    {
        private readonly string _dbPath;
        private readonly IOptions<Config> _conf;
        private ILogger<DbTrieRepository> _logger;
        private DBTrieEngine _engine;
        private MemoryPool<byte> _pool;
        private ASCIIEncoder _ascii = new ASCIIEncoder();

        public DbTrieRepository(IOptions<Config> conf, ILogger<DbTrieRepository> logger)
        {
            _dbPath = conf.Value.DBFilePath;
            _conf = conf;
            _logger = logger;
            _pool = MemoryPool<byte>.Shared;
            _engine = OpenEngine(CancellationToken.None).GetAwaiter().GetResult();
            var pageSize = 8192;
            _engine.ConfigurePagePool(new PagePool(pageSize, ( _conf.Value.DBCacheMB * 1000 * 1000) / pageSize));
        }
        
        public async Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default)
        {
            if (paymentPreimage == null) throw new ArgumentNullException(nameof(paymentPreimage));
            using var tx = await _engine.OpenTransaction(ct);
            await tx.GetTable(DBKeys.HashToPreimage).Insert(paymentPreimage.Hash.ToBytes(false), paymentPreimage.ToByteArray());
            await tx.Commit();
        }


        public async Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
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
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            using var tx = await _engine.OpenTransaction(ct);
            using var row = await tx.GetTable(DBKeys.HashToInvoice).Get(hash.Value.ToString());
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
            if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));
            using var tx = await _engine.OpenTransaction(ct);
            await tx.GetTable(DBKeys.HashToInvoice).Insert(paymentRequest.PaymentHash.Value.ToString(), paymentRequest.ToString());
            await tx.Commit();
        }

        public async IAsyncEnumerable<EndPoint> GetAllRemoteEndPoint([EnumeratorCancellation] CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            var t = tx.GetTable(DBKeys.RemoteEndPoints);
            await foreach (var item in t.Enumerate().WithCancellation(ct))
            {
                var s = _ascii.EncodeData(item.Key.Span);
                yield return NBitcoin.Utils.ParseEndpoint(s, Constants.DefaultP2PPort);
            }
        }
        
        public async Task SetRemoteEndPoint(EndPoint remoteEndPoint, CancellationToken ct = default)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException(nameof(remoteEndPoint));
            var s = remoteEndPoint.ToEndpointString();
            using var tx = await _engine.OpenTransaction(ct);
            var t = tx.GetTable(DBKeys.RemoteEndPoints);
            await t.Insert(_ascii.DecodeData(s), ReadOnlyMemory<byte>.Empty);
            await tx.Commit();
        }

        public async Task RemoveRemoteEndPoint(EndPoint remoteEndPoint, CancellationToken ct = default)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException(nameof(remoteEndPoint));
            var s = remoteEndPoint.ToEndpointString();

            using var tx = await _engine.OpenTransaction(ct);
            var t = tx.GetTable(DBKeys.RemoteEndPoints);
            await t.Delete(s);
            await tx.Commit();
        }

        public async Task<NetworkGraph?> GetNetworkGraph(CancellationToken ct = default)
        {
            using var tx = await _engine.OpenTransaction(ct);
            var t = tx.GetTable(DBKeys.NetworkGraph);
            var row = await t.Get(DBKeys.NetworkGraphVersion);
            if (row is null) return null;
            var b = await row.ReadValue();
            return NetworkGraph.FromBytes(b.ToArray());
        }

        public async Task SetNetworkGraph(NetworkGraph g, CancellationToken ct = default)
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            using var tx = await _engine.OpenTransaction(ct);
            var table = tx.GetTable(DBKeys.NetworkGraph);
            await table.Insert(DBKeys.NetworkGraphVersion, g.ToBytes());
        }

        public async IAsyncEnumerable<SpendableOutputDescriptorWithMetadata> GetAllSpendableOutputDescriptors()
        {
            using var tx = await _engine.OpenTransaction();
            var table = tx.GetTable(DBKeys.OutpointsToStaticOutputDescriptor);
            await foreach (var item in table.Enumerate())
            {
                yield return SpendableOutputDescriptorWithMetadata.FromBytes((await item.ReadValue()).ToArray());
            }
        }

        public async IAsyncEnumerable<SpendableOutputDescriptorWithMetadata?> GetSpendableOutputDescriptors(IEnumerable<OutPoint> outpoints)
        {
            if (outpoints == null) throw new ArgumentNullException(nameof(outpoints));
            using var tx = await _engine.OpenTransaction();
            var table = tx.GetTable(DBKeys.OutpointsToStaticOutputDescriptor);
            foreach (var op in outpoints)
            {
                var raw = await table.Get(op.ToBytes());
                if (raw is null)
                {
                    yield return null;
                    continue;
                }

                var b = await raw.ReadValue();
                yield return SpendableOutputDescriptorWithMetadata.FromBytes(b.ToArray());
            }
        }

        public async Task SetSpendableOutputDescriptor(SpendableOutputDescriptorWithMetadata outputDescriptor)
        {
            if (outputDescriptor == null) throw new ArgumentNullException(nameof(outputDescriptor));
            using var tx = await _engine.OpenTransaction();
            var t = tx.GetTable(DBKeys.OutpointsToStaticOutputDescriptor);
            await t.Insert(outputDescriptor.Descriptor.OutPoint.Value.ToBytes(), outputDescriptor.ToBytes());
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