using System;
using NBitcoin.RPC;

namespace NRustLightning.Server.Configuration.SubConfiguration
{
    public class ChainConfiguration
    {
        public string CredentialString { get; }
        public string CryptoCode { get; internal set; }
        public RPCClient Rpc { get; set; }
    }
}