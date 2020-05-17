using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using NRustLightning.Interfaces;
using Microsoft.Extensions.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Utils;

namespace NRustLightning.Server.P2P
{
    internal class ConnectionLoop : IAsyncDisposable
    {
        public PeerManager PeerManager { get; }
        private readonly IDuplexPipe _transport;
        private readonly ISocketDescriptor _socketDescriptor;
        private readonly ILogger<ConnectionLoop> _logger;
        private readonly Guid _id;
        public Task ExecutionTask { get; private set; }

        private CancellationTokenSource _cts;

        public ConnectionLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor, PeerManager peerManager, ILogger<ConnectionLoop> logger)
        {
            PeerManager = peerManager;
            _transport = transport;
            _socketDescriptor = socketDescriptor;
            _logger = logger;
            _id = Guid.NewGuid();
        }

        public void Start(CancellationToken ct = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _logger.LogTrace($"Starting connection loop: {_id}");
            ExecutionTask = StartLoop(_cts.Token);
            _logger.LogTrace($"Finish starting loop");
        }

        
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _logger.LogTrace($"Disposing connection loop: {_id}");
            await ExecutionTask;
        }

        public Task GetAwaiter() => this.ExecutionTask;
        
        private async Task StartLoop(CancellationToken ct)
        {
            
            try
            {
                var readTask = DoRead(ct).ConfigureAwait(false);
                var writeTask = DoWrite(ct).ConfigureAwait(false);
                await readTask;
                await writeTask;
            }
            catch (FFIException ex)
            {
                _logger.LogInformation($"rust-lightning returned error when calling write_buffer_space_avail. disconnecting");
                _logger.LogError($"{ex.Message}: {ex.StackTrace}");
                // we don't have to call `PeerManager.SocketDisconnected` here Since rust-lightning already knows the peer
                // is disconnected.
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Disconnecting peer since the socket is disconnected");
                PeerManager.SocketDisconnected(_socketDescriptor);
            }
        }

        private async Task DoRead(CancellationToken ct)
        {
            try
            {
                await ReadLoop(ct).ConfigureAwait(false);
            }
            finally
            {
                await _transport.Output.CompleteAsync();
            }
        }
        
        private async Task ReadLoop(CancellationToken ct)
        {
            while (true)
            {
                if (_socketDescriptor.Disconnected)
                {
                    _logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                    return;
                }

                var readResult = await _transport.Input.ReadAsync(ct).ConfigureAwait(false);
                var buf = readResult.Buffer;
                if (buf.IsEmpty)
                    throw new OperationCanceledException("Socket disconnected");
                foreach (var r in buf)
                {
                    _logger.LogTrace($"Received {Hex.Encode(r.Span)}");
                    var _shouldPause = PeerManager.ReadEvent(_socketDescriptor, r.Span);
                }
                PeerManager.ProcessEvents();

                _transport.Input.AdvanceTo(buf.End);
            }
        }

        private async Task DoWrite(CancellationToken ct)
        {
            await WriteLoop(ct).ConfigureAwait(false);
        }
        
        private async Task WriteLoop(CancellationToken ct)
        {
            while (true)
            {
                if (_socketDescriptor.Disconnected)
                {
                    _logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                    break;
                }

                // We don't have to do anything besides calling this (i.e. we don't have to
                // do anything with _transport.Output, like writing or flushing.)
                // because this will call into `SocketDescriptor.SendData` and do those works.
                PeerManager.WriteBufferSpaceAvail(_socketDescriptor);
            }
        }

    }
}