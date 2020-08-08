using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Server.Tests.Stubs
{
    public class InMemoryRepository : IRepository
    {
        private ConcurrentDictionary<Primitives.PaymentHash, PaymentRequest> _hashToInvoice = new ConcurrentDictionary<Primitives.PaymentHash, PaymentRequest>();
        private ConcurrentDictionary<Primitives.PaymentHash, Primitives.PaymentPreimage> _hashToPreimage = new ConcurrentDictionary<Primitives.PaymentHash, Primitives.PaymentPreimage>();
        private ChannelManager? latestChannelManager = null;
        private ManyChannelMonitor? latestManyChannelMonitor = null;
        
        private List<EndPoint> EndPoints = new List<EndPoint>();
        public Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            _hashToPreimage.TryGetValue(hash, out var preimage);
            return Task.FromResult(preimage);
        }

        public Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default)
        {
            _hashToPreimage.TryAdd(paymentPreimage.Hash, paymentPreimage);
            return Task.CompletedTask;
        }

        public Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default)
        {
            _hashToInvoice.TryGetValue(hash, out var invoice);
            return Task.FromResult(invoice);
        }

        public Task SetInvoice(PaymentRequest paymentRequest, CancellationToken ct = default)
        {
            _hashToInvoice.TryAdd(paymentRequest.PaymentHash, paymentRequest);
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<EndPoint> GetAllRemoteEndPoint(CancellationToken ct = default)
        {
            foreach (var e in EndPoints)
            {
                yield return e;
            }
        }

        public Task SetRemoteEndPoint(EndPoint remoteEndPoint, CancellationToken ct = default)
        {
            EndPoints.Add(remoteEndPoint);
            return Task.CompletedTask;
        }

        public Task RemoveRemoteEndPoint(EndPoint remoteEndPoint, CancellationToken ct = default)
        {
            EndPoints.Remove(remoteEndPoint);
            return Task.CompletedTask;
        }

        public Task SetChannelManager(ChannelManager channelManager, CancellationToken ct = default)
        {
            latestChannelManager = channelManager;
            return Task.CompletedTask;
        }


        public Task<(uint256, ChannelManager)?> GetChannelManager(ChannelManagerReadArgs readArgs, CancellationToken ct = default)
        {
            return Task.FromResult(
                latestChannelManager is null ? ((uint256, ChannelManager)?)null : (NBitcoin.Network.RegTest.GenesisHash, latestChannelManager)
                );
        }
        public Task SetManyChannelMonitor(ManyChannelMonitor manyChannelMonitor, CancellationToken ct = default)
        {
            latestManyChannelMonitor = manyChannelMonitor;
            return Task.CompletedTask;
        }
        
        public Task<(ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>)?> GetManyChannelMonitor(ManyChannelMonitorReadArgs readArgs, CancellationToken ct = default)
        {
            var latestBlockHashes = new Dictionary<Primitives.LNOutPoint, uint256>();
            return Task.FromResult(
                latestChannelManager is null ? ((ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>)?)null : (latestManyChannelMonitor, latestBlockHashes)
                );
        }

    }
}