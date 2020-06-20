using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
                while (true)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    var e = await await Task.WhenAny(sessions.Select(async session =>
                        await session.NextEventAsync(stoppingToken)));

                    var peerManager = _peerManagerProvider.GetPeerManager(e.CryptoCode);
                    if (peerManager != null)
                    {
                        if (e is NewBlockEvent newBlockEvent)
                        {
                            var cli = clis.First(c => c.CryptoCode == e.CryptoCode);
                            var newBlock = await cli.RPCClient.GetBlockAsync(newBlockEvent.Hash);
                            peerManager.BlockNotifier.BlockConnected(newBlock, (uint)newBlockEvent.Height);
                        }
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}