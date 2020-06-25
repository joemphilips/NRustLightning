using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Services
{
    public class NBXplorerListener : BackgroundService
    {
        private readonly INBXplorerClientProvider _clientProvider;
        private readonly IPeerManagerProvider _peerManagerProvider;

        public NBXplorerListener(INBXplorerClientProvider clientProvider, IPeerManagerProvider peerManagerProvider)
        {
            _clientProvider = clientProvider;
            _peerManagerProvider = peerManagerProvider;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var clis = _clientProvider.GetAll();
            {
                var sessions =
                    await Task.WhenAll(clis.Select(async cli => await cli.CreateWebsocketNotificationSessionAsync(stoppingToken).ConfigureAwait(false)));
                await Task.WhenAll(sessions.Select(async session => await session.ListenNewBlockAsync(stoppingToken).ConfigureAwait(false)));
                await Task.WhenAll(sessions.Select(async (s) => await ListenToSession(s, stoppingToken)));
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task ListenToSession(WebsocketNotificationSession session, CancellationToken stoppingToken)
        {
            var client = session.Client;
            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();
                var e = await session.NextEventAsync(stoppingToken);

                var peerManager = _peerManagerProvider.GetPeerManager(e.CryptoCode);
                if (peerManager != null)
                {
                    if (e is NewBlockEvent newBlockEvent)
                    {
                        var newBlock = await client.RPCClient.GetBlockAsync(newBlockEvent.Hash);
                        peerManager.BlockNotifier.BlockConnected(newBlock, (uint)newBlockEvent.Height);
                    }
                }
            }
        }
    }
}