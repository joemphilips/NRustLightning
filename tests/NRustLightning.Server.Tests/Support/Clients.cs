using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using NBitcoin.RPC;
using NRustLightning.Client;

namespace NRustLightning.Server.Tests.Support
{
    public class Clients
    {
        public Clients(RPCClient bitcoinRPCClient, LndClient lndClient, CLightningClient cLightningClient, NRustLightningClient nRustLightningHttpClient, NBXplorer.ExplorerClient nbxClient)
        {
            BitcoinRPCClient = bitcoinRPCClient;
            LndClient = lndClient;
            CLightningClient = cLightningClient;
            NRustLightningHttpClient = nRustLightningHttpClient;
            NBXClient = nbxClient;
        }
        public readonly RPCClient BitcoinRPCClient;
        public readonly LndClient LndClient;
        public ILightningClient LndLNClient => (ILightningClient) LndClient;
        public readonly CLightningClient CLightningClient;
        public ILightningClient ClightningLNClient => (ILightningClient) CLightningClient;
        public readonly NRustLightningClient NRustLightningHttpClient;

        public readonly NBXplorer.ExplorerClient NBXClient;
    }
}