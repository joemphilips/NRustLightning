using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NRustLightning.Net.Sockets;

namespace NRustLightning.Server.P2P
{
    public class P2PConnectionFactory : IConnectionFactory
    {
        public ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new SocketConnection(endpoint).StartAsync();
        }
    }
}