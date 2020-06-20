using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using NBitcoin;
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

        public async Task ConnectAll()
        {
            var clightningInfo = await ClightningLNClient.GetInfo();
            await NRustLightningHttpClient.ConnectAsync(clightningInfo.NodeInfoList.FirstOrDefault().ToConnectionString());
            var lndInfo = await LndLNClient.GetInfo();
            await NRustLightningHttpClient.ConnectAsync(lndInfo.NodeInfoList.FirstOrDefault().ToConnectionString());
            await LndLNClient.ConnectTo(clightningInfo.NodeInfoList.FirstOrDefault());
        }

        public async Task PrepareFunds()
        {
            var nrlBalance = (await NRustLightningHttpClient.GetWalletInfoAsync()).BalanceSatoshis;
            if (nrlBalance > 0)
            {
                return;
            }
            
            var clAddress = (await CLightningClient.NewAddressAsync());
            var lndAddress = BitcoinAddress.Create((await LndClient.SwaggerClient.NewWitnessAddressAsync()).Address, Network.RegTest);
            var nrlAddress = (await NRustLightningHttpClient.GetNewDepositAddressAsync()).Address;
            foreach (var addr in new[] {clAddress, lndAddress, nrlAddress})
            {
                await this.BitcoinRPCClient.GenerateToAddressAsync(1, addr);
            }

            await this.BitcoinRPCClient.GenerateAsync(Network.RegTest.Consensus.CoinbaseMaturity);
        }
    }
}