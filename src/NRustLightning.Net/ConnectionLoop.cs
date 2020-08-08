using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRustLightning.Adaptors;

namespace NRustLightning.Net
{
    public class ConnectionLoop : IAsyncDisposable
    {
        public PeerManager PeerManager { get; }
        private readonly IDuplexPipe _transport;
        private readonly SocketDescriptor _socketDescriptor;
        private readonly ILogger<ConnectionLoop> _logger;
        private readonly ChannelReader<byte> _writeAvailReceiver;
        private readonly ChannelWriter<byte> _eventNotify;
        private readonly Func<Task>? _cleanup;
        private readonly Guid _id;
        public Task? ExecutionTask { get; private set; }

        private CancellationTokenSource? _cts;

        public ConnectionLoop(IDuplexPipe transport, SocketDescriptor socketDescriptor, PeerManager peerManager,
            ILogger<ConnectionLoop> logger, ChannelReader<byte> writeAvailReceiver, ChannelWriter<byte> eventNotify, Func<Task>? cleanup = null)
        {
            PeerManager = peerManager;
            _transport = transport;
            _socketDescriptor = socketDescriptor;
            _logger = logger;
            _writeAvailReceiver = writeAvailReceiver;
            _eventNotify = eventNotify;
            _cleanup = cleanup;
            _id = Guid.NewGuid();
        }

        public ConnectionLoop Start(CancellationToken ct = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _logger.LogTrace($"Starting connection loop: {_id}");
            ExecutionTask = StartLoop(_cts.Token);
            return this;
        }

        public async ValueTask DisposeAsync()
        {
            if (_cleanup != null)
                await _cleanup.Invoke();
            _cts?.Cancel();
            _logger.LogTrace($"Disposing connection loop: {_id}");
            if (ExecutionTask != null)
                await ExecutionTask;
        }

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
                _logger.LogInformation(
                    $"rust-lightning returned error. disconnecting");
                _logger.LogError($"{ex.Message}: {ex.StackTrace}");
                // we don't have to call `PeerManager.SocketDisconnected` here Since rust-lightning already knows the peer
                // is disconnected.
            }
            catch (RLDisconnectedException)
            {
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Disconnecting peer since the socket is disconnected");
                PeerManager.SocketDisconnected(_socketDescriptor);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"error while processing connection {_id}. Closing");
                _logger.LogError($"{ex.Message}: {ex.StackTrace}");
                PeerManager.SocketDisconnected(_socketDescriptor);
            }
            finally
            {
                await _transport.Output.CompleteAsync();
                await _transport.Input.CompleteAsync();
                _eventNotify.TryWrite(1);
                if (_cleanup != null)
                    await _cleanup.Invoke();
            }
        }

        private async Task DoRead(CancellationToken ct)
        {
            await ReadLoop(ct).ConfigureAwait(false);
        }
        
        private async Task ReadLoop(CancellationToken ct)
        {
            while (true)
            {
                if (_socketDescriptor.ReadPaused)
                {
                    await Task.Delay(10, ct);
                    continue;
                }
                ct.ThrowIfCancellationRequested();
                var readResult = await _transport.Input.ReadAsync(ct).ConfigureAwait(false);
                var buf = readResult.Buffer;
                if (buf.IsEmpty)
                    throw new OperationCanceledException("socket disconnected");
                foreach (var r in buf)
                {
                    PrepareReadWriteCall();
                    if (PeerManager.TryReadEvent(_socketDescriptor, r.Span, out var shouldPause, out var ffiResult))
                    {
                        _socketDescriptor.ReadPaused = shouldPause;
                    }
                    else
                    {
                        _logger.LogError($"read_event call to rust-lightning failed. closing connection. {ffiResult}");
                        throw new RLDisconnectedException();
                    }
                    _socketDescriptor.BlockDisconnectSocket = false;
                }
                PeerManager.ProcessEvents();
                _eventNotify.TryWrite(1);

                _transport.Input.AdvanceTo(buf.End);
            }
        }

        private async Task DoWrite(CancellationToken ct)
        {
            await WriteLoop(ct).ConfigureAwait(false);
        }
        
        private async Task WriteLoop(CancellationToken ct)
        {
            while (await _writeAvailReceiver.WaitToReadAsync(ct))
            {
                _writeAvailReceiver.TryRead(out _);
                ct.ThrowIfCancellationRequested();
                PrepareReadWriteCall();
                PeerManager.WriteBufferSpaceAvail(_socketDescriptor);
                _socketDescriptor.BlockDisconnectSocket = false;
            }
        }
        /// <summary>
        /// When the socket is disconnected internally, we throw this to do cleanup
        /// without calling `PeerManager.SocketDisconnected`
        /// </summary>
        private class RLDisconnectedException : Exception
        {
        }
        /// <summary>
        /// </summary>
        /// <returns>should we shutdown this connection or not</returns>
        private void PrepareReadWriteCall()
        {
            if (_socketDescriptor.Disconnected)
            {
                _logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                throw new RLDisconnectedException();
            }
            _socketDescriptor.BlockDisconnectSocket = true;
        }

    }
}