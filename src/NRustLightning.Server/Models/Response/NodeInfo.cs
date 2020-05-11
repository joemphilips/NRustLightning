using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Server.Models.Response
{
    public class NodeInfo
    {
        public int NumConnected { get; set; }

        public PeerConnectionString ConnectionString { get; set; }
    }
}