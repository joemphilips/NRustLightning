using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Services;
using NRustLightning.Utils;

namespace NRustLightning.Server.P2P
{
    public class P2PConnectionHandler: ConnectionHandler
    {
        public PeerManager PeerManager { get; }
        private readonly ISocketDescriptorFactory descriptorFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly P2PConnectionFactory _connectionFactory;
        private readonly ILogger<P2PConnectionHandler> _logger;
        private readonly ConcurrentDictionary<EndPoint, ConnectionLoop> _connectionLoops = new ConcurrentDictionary<EndPoint, ConnectionLoop>();
        private readonly MemoryPool<byte> _pool;
 
        public P2PConnectionHandler(ISocketDescriptorFactory descriptorFactory, PeerManagerProvider peerManager,
            ILoggerFactory loggerFactory, P2PConnectionFactory connectionFactory)
        {
            // TODO: Support other chains
            this.descriptorFactory = descriptorFactory ?? throw new ArgumentNullException(nameof(descriptorFactory));
            _loggerFactory = loggerFactory;
            _connectionFactory = connectionFactory;
            _logger = _loggerFactory.CreateLogger<P2PConnectionHandler>();
            var pmProvider = peerManager ?? throw new ArgumentNullException(nameof(peerManager));
            PeerManager = pmProvider.GetPeerManager("BTC");
            _logger.LogWarning("WARNING: it only supports BTC");
            _pool = MemoryPool<byte>.Shared;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            if (_connectionLoops.ContainsKey(connection.RemoteEndPoint))
            {
                _logger.LogWarning($"another inbound connection from a known peer: {connection.RemoteEndPoint.ToEndpointString()}. Ignoring");
                return;
            }
            
            _logger.LogDebug($"New inbound connection from {connection.RemoteEndPoint.ToEndpointString()}");
            var (descriptor, writeReceiver) = descriptorFactory.GetNewSocket(connection.Transport.Output);
            PeerManager.NewInboundConnection(descriptor);
            Action cleanup = () => { _connectionLoops.TryRemove(connection.RemoteEndPoint, out _); };
            var conn = new ConnectionLoop(connection.Transport, descriptor, PeerManager, _loggerFactory.CreateLogger<ConnectionLoop>(), writeReceiver, cleanup);
            _connectionLoops.TryAdd(connection.RemoteEndPoint, conn);
            conn.Start(connection.ConnectionClosed);
            await conn.ExecutionTask;
        }

        /// <summary>
        /// Returns `false` if the peer is already connected. otherwise return `true`.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="pubkey"></param>
        /// <param name="ct"></param>
        /// <returns>true if the peer is unknown</returns>
        public async ValueTask<bool> NewOutbound(EndPoint remoteEndPoint, PubKey pubkey, CancellationToken ct = default)
        {
            if (_connectionLoops.ContainsKey(remoteEndPoint))
            {
                _logger.LogError($"We have already connected to: {remoteEndPoint.ToEndpointString()}.");
                return false;
            }

            try
            {
                var connectionContext = await _connectionFactory.ConnectAsync(remoteEndPoint, ct);
                var (descriptor, writeReceiver) = descriptorFactory.GetNewSocket(connectionContext.Transport.Output);
                var initialSend = PeerManager.NewOutboundConnection(descriptor, pubkey.ToBytes());
                await connectionContext.Transport.Output.WriteAsync(initialSend, ct);
                var flushResult = connectionContext.Transport.Output.FlushAsync(ct);
                if (!flushResult.IsCompleted)
                {
                    await flushResult.ConfigureAwait(false);
                }
                
                Action cleanup = () => { _connectionLoops.TryRemove(connectionContext.RemoteEndPoint, out _); };
                var conn = new ConnectionLoop(connectionContext.Transport, descriptor, PeerManager,
                    _loggerFactory.CreateLogger<ConnectionLoop>(), writeReceiver, cleanup);
                _connectionLoops.TryAdd(remoteEndPoint, conn);
                Task.Run(() => conn.Start(ct));
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                _logger.LogError($"{ex.Message}:{Environment.NewLine}{ex.StackTrace}");
                return false;
            }

            return true;
        }

        public PubKey[] GetPeerNodeIds() => PeerManager.GetPeerNodeIds(_pool);
        
        public async Task<bool> DisconnectPeer(EndPoint remoteEndPoint)
        {
            if (_connectionLoops.TryRemove(remoteEndPoint, out var conn))
            {
                _logger.LogInformation($"Disconnecting peer {remoteEndPoint.ToEndpointString()}");
                await conn.DisposeAsync();
                return true;
            }
            else
            {
                _logger.LogWarning($"Could not disconnect {remoteEndPoint.ToEndpointString()}: peer unknown");
                return false;
            }
        }
    }
}