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
                var events = _peerManager.ChannelManager.GetAndClearPendingEvents();
                foreach (var e in events)
                {
                    if (e is Event.FundingGenerationReady f)
                    {
                        var d = f.Item;
                    }
                    else if (e is Event.PendingHTLCsForwardable _)
                    {
                        _peerManager.ChannelManager.ProcessPendingHTLCForwards();
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