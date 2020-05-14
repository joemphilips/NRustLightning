using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
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
        private readonly Dictionary<EndPoint, ConnectionLoop> _connectionLoops = new Dictionary<EndPoint, ConnectionLoop>();
 
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
        }

        public override Task OnConnectedAsync(ConnectionContext connection)
        {
            if (_connectionLoops.ContainsKey(connection.RemoteEndPoint))
            {
                _logger.LogDebug($"connection from known peer: {connection.RemoteEndPoint}");
                return Task.CompletedTask;
            }
            
            _logger.LogDebug($"New inbound connection from {connection.RemoteEndPoint}");
            var descriptor = descriptorFactory.GetNewSocket(connection.Transport.Output);
            PeerManager.NewInboundConnection(descriptor);
            var conn = new ConnectionLoop(connection.Transport, descriptor, PeerManager, _loggerFactory.CreateLogger<ConnectionLoop>());
            _connectionLoops.Add(connection.RemoteEndPoint, conn);
            conn.Start(connection.ConnectionClosed);
            return conn.GetAwaiter();
        }

        /// <summary>
        /// Returns `false` if the peer is already connected. otherwise return `true`.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="pubkey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async ValueTask<bool> NewOutbound(EndPoint remoteEndPoint, PubKey pubkey, CancellationToken ct = default)
        {
            if (_connectionLoops.ContainsKey(remoteEndPoint))
            {
                _logger.LogError($"We have already connected to: {remoteEndPoint}.");
                return false;
            }

            try
            {
                var connectionContext = await _connectionFactory.ConnectAsync(remoteEndPoint, ct);
                var descriptor = descriptorFactory.GetNewSocket(connectionContext.Transport.Output);
                PeerManager.NewOutboundConnection(descriptor, pubkey.ToBytes());
                var conn = new ConnectionLoop(connectionContext.Transport, descriptor, PeerManager,
                    _loggerFactory.CreateLogger<ConnectionLoop>());
                _connectionLoops.Add(remoteEndPoint, conn);
                conn.Start(ct);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                _logger.LogError($"{ex.Message}");
                return false;
            }

            return true;
        }
        
        public async Task DisconnectPeer(EndPoint remoteEndPoint, CancellationToken ct = default)
        {
            if (_connectionLoops.TryGetValue(remoteEndPoint, out var conn))
            {
                await conn.DisposeAsync();
                _connectionLoops.Remove(remoteEndPoint);
            }
            else
            {
                _logger.LogWarning($"Could not disconnect {remoteEndPoint.ToEndpointString()}: peer unknown");
            }
        }
    }
}