using System;
using System.Collections.Generic;
using System.Net;
using NRustLightning.Adaptors;

namespace NRustLightning.Server.Configuration
{
    public class Config
    {
        public IPEndPoint P2PIpEndPoint { get; set; } = Constants.DefaultP2PEndPoint;
        public Uri NBXplorerUri { get; set; } = new Uri("http://127.0.0.1:4774");
        public string ConfigurationFile { get; set; } = "nrustlightning.conf";
        public string DataDir { get; set; } = Constants.DataDirectoryPath;
        public RPCArgs RpcArgs { get; set; }
        public List<ChainConfiguration> ChainConfiguration { get; } = new List<ChainConfiguration>();
        
        public UserConfig RustLightningConfig { get; }
    }
}