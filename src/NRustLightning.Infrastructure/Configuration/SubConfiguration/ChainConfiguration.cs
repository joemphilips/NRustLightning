using NBitcoin.RPC;

namespace NRustLightning.Infrastructure.Configuration.SubConfiguration
{
    public class ChainConfiguration
    {
        public string CredentialString { get; }
        public string CryptoCode { get; internal set; }
        public RPCClient Rpc { get; set; }
    }
}