using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using NBitcoin.RPC;
using NRustLightning.Client;

namespace NRustLightning.Server.Tests.Support
{
    public class Clients
    {
        public Clients(RPCClient bitcoinRPCClient, LndClient lndClient, CLightningClient cLightningClient, NRustLightningClient httpClient)
        {
            BitcoinRPCClient = bitcoinRPCClient;
            LndClient = lndClient;
            CLightningClient = cLightningClient;
            HttpClient = httpClient;
        }
        public readonly RPCClient BitcoinRPCClient;
        public readonly LndClient LndClient;
        public readonly CLightningClient CLightningClient;
        public readonly NRustLightningClient HttpClient;
    }
}