using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using NBitcoin.DataEncoders;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Services;
using NRustLightning.Utils;

namespace NRustLightning.Server
{
    public class P2PConnectionHandler: ConnectionHandler
    {
        public PeerManager PeerManager { get; }
        private readonly ISocketDescriptorFactory descriptorFactory;
        private readonly ILogger<P2PConnectionHandler> logger;
        public Dictionary<System.Net.EndPoint, ISocketDescriptor> EndpointsToDesc = new Dictionary<System.Net.EndPoint, ISocketDescriptor>();
 
        public P2PConnectionHandler(ISocketDescriptorFactory descriptorFactory, PeerManagerProvider peerManager, ILogger<P2PConnectionHandler> logger)
        {
            var pmProvider = peerManager ?? throw new ArgumentNullException(nameof(peerManager));
            // TODO: Support other chains
            PeerManager = pmProvider.GetPeerManager("BTC");
            logger.LogWarning("WARNING: it only supports BTC");
            this.descriptorFactory = descriptorFactory ?? throw new ArgumentNullException(nameof(descriptorFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task WriteLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor, CancellationToken ct)
        {
            bool shouldStop = false;
            while (!shouldStop)
            {
                PeerManager.WriteBufferSpaceAvail(socketDescriptor);
                logger.LogTrace("Flushing");
                var flushResult = await transport.Output.FlushAsync(ct).ConfigureAwait(false);
                logger.LogTrace($"flushed. Completed: {flushResult.IsCompleted}. Canceled: {flushResult.IsCanceled}");
                shouldStop = flushResult.IsCompleted || flushResult.IsCanceled;
            }
        }

        private async Task<bool> ReadLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor, CancellationToken ct)
        {
            var readResult = await transport.Input.ReadAsync(ct).ConfigureAwait(false);
            var buf = readResult.Buffer;
            if (buf.IsEmpty)
                throw new OperationCanceledException("Socket disconnected");
            foreach (var r in buf)
            {
                logger.LogTrace($"Received {Hex.Encode(r.Span)}");
                PeerManager.ReadEvent(socketDescriptor, r.Span);
                PeerManager.ProcessEvents();
            }
            transport.Input.AdvanceTo(buf.End);
            return readResult.IsCompleted;
        }
        private async Task ConnectionLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor, CancellationToken ct)
        {
            while (true)
            {
                if (socketDescriptor.Disconnected)
                {
                    logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                    return;
                }

                try
                {
                    var isCompleted = await ReadLoop(transport, socketDescriptor, ct);
                    if (isCompleted && !socketDescriptor.Disconnected)
                        await WriteLoop(transport, socketDescriptor, ct);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning($"Disconnecting peer since the socket is disconnected");
                    PeerManager.SocketDisconnected(socketDescriptor);
                    return;
                }
            }
        }
        
        public override Task OnConnectedAsync(ConnectionContext connection)
        {
            if (EndpointsToDesc.TryGetValue(connection.RemoteEndPoint, out var socketDescriptor))
            {
                logger.LogDebug($"connection from known peer: {connection.RemoteEndPoint}, {socketDescriptor.Index}");
                return ConnectionLoop(connection.Transport, socketDescriptor, connection.ConnectionClosed);
            }
            
            logger.LogDebug($"New inbound connection from {connection.RemoteEndPoint}");
            var descriptor = descriptorFactory.GetNewSocket(connection.Transport.Output);
            EndpointsToDesc.Add(connection.RemoteEndPoint, descriptor);
            PeerManager.NewInboundConnection(descriptor);
            return ConnectionLoop(connection.Transport, descriptor, connection.ConnectionClosed);
        }
    }
}