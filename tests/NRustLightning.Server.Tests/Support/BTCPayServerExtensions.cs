using System;
using System.Net;
using BTCPayServer.Lightning;
using NBitcoin;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Server.Tests.Support
{
    public static class BTCPayServerExtensions
    {
        public static PeerConnectionString ToConnectionString(this NodeInfo nodeInfo)
        {
            var ep = NBitcoin.Utils.ParseEndpoint($"{nodeInfo.Host}:{nodeInfo.Port}", nodeInfo.Port);
            return new PeerConnectionString(nodeInfo.NodeId, ep);
        }

        public static NodeInfo ToNodeInfo(this PeerConnectionString connectionString)
        {
            var str = connectionString.EndPoint.ToEndpointString();
            // trim protocol part
            var index = str.IndexOf("://", StringComparison.InvariantCulture);
            if (index != -1)
            {
                str = str[(index + 2)..];
            }

            var hostAndPort = str.Split(":");
            var (host, port) = (hostAndPort[0], hostAndPort[1]);
            return new NodeInfo(connectionString.NodeId, host, Int32.Parse(port));
        }
    }
}