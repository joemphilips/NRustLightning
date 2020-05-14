using System.Net;
using BTCPayServer.Lightning;
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
    }
}