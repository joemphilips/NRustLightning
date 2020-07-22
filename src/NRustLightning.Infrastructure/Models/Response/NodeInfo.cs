using System.Collections.Generic;
using NRustLightning.Infrastructure.Models.Request;

namespace NRustLightning.Infrastructure.Models.Response
{
    public class NodeInfo
    {
        public int NumConnected { get; set; }
        
        public List<string> NodeIds { get; set; }

        public PeerConnectionString ConnectionString { get; set; }
    }
}