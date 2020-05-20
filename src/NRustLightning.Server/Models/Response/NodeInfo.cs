using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Newtonsoft.Json;
using NRustLightning.Server.JsonConverters;
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