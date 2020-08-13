using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Server.P2P;

namespace NRustLightning.Server.Services
{

    public class ChannelProcessorHolder
    {
    }
    
    public class ChannelProcessor
    {
        private readonly ChannelProvider _channelProvider;
        private readonly P2PConnectionHandler _p2PConnectionHandler;
        private readonly ILogger<ChannelProcessor> _logger;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetwork _network;
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        private Task _task;

        public ChannelProcessor(ChannelProvider channelProvider, P2PConnectionHandler p2PConnectionHandler, ILogger<ChannelProcessor> logger, PeerManagerProvider peerManagerProvider, NRustLightningNetwork network)
        {
            _channelProvider = channelProvider;
            _p2PConnectionHandler = p2PConnectionHandler;
            _logger = logger;
            _peerManagerProvider = peerManagerProvider;
            _network = network;
            _task = HandleOutboundConnectQueue();
        }

        private async Task HandleOutboundConnectQueue()
        {
            var t = _channelProvider.GetOutboundConnectionRequestQueue("BTC");
            var chanMan = _peerManagerProvider.GetPeerManager(_network).ChannelManager;
            var knownChannels = chanMan.ListChannels(_pool);
            while (await t.Reader.WaitToReadAsync())
            {
                var req = await t.Reader.ReadAsync();
                try
                {
                    await _p2PConnectionHandler.NewOutbound(req.EndPoint, req.NodeId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    _logger.LogError($"Failed to resume the connection for {req} force-closing the channel.");
                    var channelDetail = knownChannels.First(c => c.RemoteNetworkId.Equals(req.NodeId));
                    chanMan.ForceCloseChannel(channelDetail.ChannelId);
                }
            }
        }
        
    }
}