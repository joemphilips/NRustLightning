using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PeerController : ControllerBase
    {
        private readonly IConnectionFactory connectionFactory;
        public PeerManagerProvider PeerManagerProvider { get; }
        public ISocketDescriptorFactory SocketDescriptorFactory { get; }

        public PeerController(PeerManagerProvider peerManagerProvider, ISocketDescriptorFactory socketDescriptorFactory, IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            PeerManagerProvider = peerManagerProvider;
            SocketDescriptorFactory = socketDescriptorFactory;
        }
        
        [HttpPost]
        [Route("{cryptoCode}/connect")]
        public async Task<bool> Connect(string cryptoCode, PeerConnectionString connectionString)
        {
            var peer = PeerManagerProvider.GetPeerManager(cryptoCode) ?? throw new NotSupportedException(cryptoCode);
            var conn = await connectionFactory.ConnectAsync(connectionString.EndPoint);
            peer.NewOutboundConnection(SocketDescriptorFactory.GetNewSocket(conn.Transport.Output), connectionString.NodeId.Value.ToBytes());
            throw new NotImplementedException();
        }
    }
}