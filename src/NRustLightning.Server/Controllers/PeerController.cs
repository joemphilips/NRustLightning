using System.Net.Http;
using System.Threading.Tasks;
using DotNetLightning.Peer;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Net;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.P2P;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    // [Authorize(AuthenticationSchemes = "LSAT", Policy = "Readonly")]
    public class PeerController : ControllerBase
    {
        private readonly P2PConnectionHandler _connectionHandler;
        public PeerManagerProvider PeerManagerProvider { get; }
        public ISocketDescriptorFactory SocketDescriptorFactory { get; }

        public PeerController(PeerManagerProvider peerManagerProvider, ISocketDescriptorFactory socketDescriptorFactory, P2PConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
            PeerManagerProvider = peerManagerProvider;
            SocketDescriptorFactory = socketDescriptorFactory;
        }
        
        [HttpPost]
        [Route("connect")]
        public async Task<bool> Connect(
            //[ModelBinder(BinderType = typeof(PeerConnectionStringModelBinders))]
            [FromBody]string connectionString)
        {
            if (PeerConnectionString.TryCreate(connectionString, out var conn))
            {
                var isNewPeer =
                    await _connectionHandler.NewOutbound(conn.EndPoint, conn.NodeId);
                return isNewPeer;
            }

            throw new HttpRequestException($"Invalid connection string {connectionString}");
        }
        
        [HttpDelete]
        [Route("disconnect")]
        public async Task<bool> Disconnect([FromBody] string connectionString)
        {
            if (PeerConnectionString.TryCreate(connectionString, out var conn))
            {
                return await _connectionHandler.DisconnectPeer(conn.EndPoint);
            }
            throw new HttpRequestException($"Invalid connection string {connectionString}");
        }
    }
}