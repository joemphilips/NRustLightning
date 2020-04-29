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

        private async Task ConnectionLoop(IDuplexPipe transport, ISocketDescriptor socketDescriptor)
        {
            while (true)
            {
                if (socketDescriptor.Disconnected)
                {
                    logger.LogInformation("Disconnecting peer since rust-lightning has requested");
                    return;
                }

                var readResult = await transport.Input.ReadAsync();
                var buf = readResult.Buffer;
                foreach (var r in buf)
                {
                    logger.LogDebug($"Received {Hex.Encode(r.Span)}");
                    PeerManager.ReadEvent(socketDescriptor, r.Span);
                }
                transport.Input.AdvanceTo(buf.End);
                if (readResult.IsCompleted)
                {
                    break;
                }
            }
        }
        
        public override Task OnConnectedAsync(ConnectionContext connection)
        {
            if (EndpointsToDesc.TryGetValue(connection.RemoteEndPoint, out var socketDescriptor))
            {
                return ConnectionLoop(connection.Transport, socketDescriptor);
            }

            logger.LogDebug($"New inbound connection from {connection.RemoteEndPoint}");
            var descriptor = descriptorFactory.GetNewSocket(connection.Transport.Output);
            PeerManager.NewInboundConnection(descriptor);
            return ConnectionLoop(connection.Transport, descriptor);
        }
    }
}