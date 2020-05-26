using System.Collections.Generic;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Server.Models.Response
{
    public class NodeInfo
    {
        public int NumConnected { get; set; }
        
        public List<string> NodeIds { get; set; }

        public PeerConnectionString ConnectionString { get; set; }
    }
}