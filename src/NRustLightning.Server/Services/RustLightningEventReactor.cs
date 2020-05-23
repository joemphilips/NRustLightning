using System;
using System.Threading;
using System.Threading.Tasks;
using NRustLightning.RustLightningTypes;
using Microsoft.Extensions.Hosting;
using NRustLightning.Server.P2P;

namespace NRustLightning.Server.Services
{
    public class RustLightningEventReactor : IHostedService
    {
        private readonly P2PConnectionHandler _connectionHandler;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly PeerManager _peerManager;
        public RustLightningEventReactor(P2PConnectionHandler connectionHandler, PeerManagerProvider peerManagerProvider)
        {
            _connectionHandler = connectionHandler;
            _peerManagerProvider = peerManagerProvider;
            _peerManager = peerManagerProvider.GetPeerManager("BTC");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (await _connectionHandler.EventNotify.Reader.WaitToReadAsync(cancellationToken))
            {
                var chanMan = _peerManager.ChannelManager;
                var events = chanMan.GetAndClearPendingEvents();
                foreach (var e in events)
                {
                    if (e is Event.FundingGenerationReady f)
                    {
                        var d = f.Item;
                        chanMan.FundingTransactionGenerated();
                    }
                    else if (e is Event.FundingBroadcastSafe fundingBroadcastSafe)
                    {
                    }
                    else if (e is Event.PaymentReceived paymentReceived)
                    {
                    }
                    else if (e is Event.PaymentSent paymentSent)
                    {
                    }
                    else if (e is Event.PaymentFailed paymentFailed)
                    {
                    }
                    else if (e is Event.PendingHTLCsForwardable _)
                    {
                        _peerManager.ChannelManager.ProcessPendingHTLCForwards();
                    }
                    else if (e is Event.SpendableOutputs spendableOutputs)
                    {
                    }
                    else
                    {
                        throw new Exception("Unreachable!");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}