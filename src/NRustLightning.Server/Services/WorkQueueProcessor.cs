using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetLightning.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.P2P;

namespace NRustLightning.Server.Services
{
    public class WorkQueueProcessors
    {
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly Dictionary<string, WorkQueueProcessor> _processors = new Dictionary<string, WorkQueueProcessor>();
        public WorkQueueProcessors(INBXplorerClientProvider clientProvider, NRustLightningNetworkProvider networkProvider,
            IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _networkProvider = networkProvider;
            foreach (var n in networkProvider.GetAll())
            {
                var cli = clientProvider.TryGetClient(n.CryptoCode);
                if (cli is null)
                    continue;
                var p = new WorkQueueProcessor(
                    serviceProvider.GetRequiredService<ChannelProvider>(),
                    serviceProvider.GetRequiredService<P2PConnectionHandler>(),
                    loggerFactory.CreateLogger<WorkQueueProcessor>(),
                    serviceProvider.GetRequiredService<PeerManagerProvider>(),
                    n,
                    serviceProvider.GetRequiredService<WalletService>()
                );
                _processors.Add(n.CryptoCode, p);
            }
        }
    }

    /// <summary>
    /// TODO: dispose when the all work done?
    /// </summary>
    public class WorkQueueProcessor
    {
        private readonly ChannelProvider _channelProvider;
        private readonly P2PConnectionHandler _p2PConnectionHandler;
        private readonly ILogger<WorkQueueProcessor> _logger;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetwork _network;
        private readonly WalletService _walletService;
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        private Task[] _tasks;

        public WorkQueueProcessor(ChannelProvider channelProvider, P2PConnectionHandler p2PConnectionHandler,
            ILogger<WorkQueueProcessor> logger, PeerManagerProvider peerManagerProvider, NRustLightningNetwork network,
            WalletService walletService)
        {
            _channelProvider = channelProvider;
            _p2PConnectionHandler = p2PConnectionHandler;
            _logger = logger;
            _peerManagerProvider = peerManagerProvider;
            _network = network;
            _walletService = walletService;
            _tasks = new [] { HandleOutboundConnectQueue(), HandleSpendableOutputEventQueue() };
        }

        private async Task HandleOutboundConnectQueue()
        {
            var t = _channelProvider.GetOutboundConnectionRequestQueue(_network.CryptoCode);
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

        private async Task HandleSpendableOutputEventQueue()
        {
            var t = _channelProvider.GetSpendableOutputDescriptorChannel(_network.CryptoCode);
            while (await t.Reader.WaitToReadAsync())
            {
                var desc = await t.Reader.ReadAsync();
                await _walletService.SaveSpendableOutput(_network, desc);
                await _walletService.TrackSpendableOutput(_network, desc);
            }
        }
        
    }
}