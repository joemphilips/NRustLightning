using System;
using NBitcoin.RPC;

namespace NRustLightning.Server
{
    public class ChainConfiguration
    {
        public Uri NodeEndPoint { get; internal set; }
        public string CredentialString { get; }
    }
}