using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using NRustLightning.Interfaces;
using Microsoft.Extensions.Logging;
using NRustLightning.Utils;

namespace NRustLightning.Server.P2P
{
    internal class ConnectionLoop : IAsyncDisposable
    {
        public PeerManager PeerManager { get; }
        private readonly IDuplexPipe _transport;
        private readonly ISocketDescriptor _socketDescriptor;
        private readonly ILogger<ConnectionLoop> _logger;
        public Task ExecutionTask { get; private set; }

        private CancellationTokenSource _cts;

        public ConnectionLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor, PeerManager peerManager, ILogger<ConnectionLoop> logger)
        {
            PeerManager = peerManager;
            _transport = transport;
            _socketDescriptor = socketDescriptor;
            _logger = logger;
        }

        public void Start(CancellationToken ct = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            ExecutionTask = StartLoop(_cts.Token);
        }

        
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            await ExecutionTask;
        }

        public Task GetAwaiter() => this.ExecutionTask;
        
        private async Task StartLoop(CancellationToken ct)
        {
            
            while (true)
            {
                if (_socketDescriptor.Disconnected)
                {
                    _logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                    return;
                }

                try
                {
                    var isCompleted = await ReadLoop(ct).ConfigureAwait(false);
                    if (isCompleted && !_socketDescriptor.Disconnected)
                        await WriteLoop(ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning($"Disconnecting peer since the socket is disconnected");
                    PeerManager.SocketDisconnected(_socketDescriptor);
                    return;
                }
            }
        }
        
        private async Task WriteLoop(CancellationToken ct)
        {
            bool shouldStop = false;
            while (!shouldStop)
            {
                PeerManager.WriteBufferSpaceAvail(_socketDescriptor);
                _logger.LogTrace("Flushing");
                var flushResult = await _transport.Output.FlushAsync(ct).ConfigureAwait(false);
                _logger.LogTrace($"flushed. Completed: {flushResult.IsCompleted}. Canceled: {flushResult.IsCanceled}");
                shouldStop = flushResult.IsCompleted || flushResult.IsCanceled;
            }
        }

        private async Task<bool> ReadLoop(CancellationToken ct)
        {
            var readResult = await _transport.Input.ReadAsync(ct).ConfigureAwait(false);
            var buf = readResult.Buffer;
            if (buf.IsEmpty)
                throw new OperationCanceledException("Socket disconnected");
            foreach (var r in buf)
            {
                _logger.LogTrace($"Received {Hex.Encode(r.Span)}");
                PeerManager.ReadEvent(_socketDescriptor, r.Span);
                PeerManager.ProcessEvents();
            }
            _transport.Input.AdvanceTo(buf.End);
            return readResult.IsCompleted;
        }

    }
}