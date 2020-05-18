using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NRustLightning.Server.P2P.Sockets;

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