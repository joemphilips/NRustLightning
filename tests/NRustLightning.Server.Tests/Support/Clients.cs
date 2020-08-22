using System;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using NBitcoin;
using NBitcoin.RPC;
using NRustLightning.Client;
using System.Linq;
using Xunit;

namespace NRustLightning.Server.Tests.Support
{
    public class Clients
    {
        public Clients(RPCClient bitcoinRPCClient, LndClient lndClient, CLightningClient cLightningClient, NRustLightningClient nRustLightningHttpClient, NBXplorer.ExplorerClient nbxClient)
        {
            BitcoinRPCClient = bitcoinRPCClient ?? throw new ArgumentNullException(nameof(bitcoinRPCClient));
            LndClient = lndClient ?? throw new ArgumentNullException(nameof(lndClient));
            CLightningClient = cLightningClient ?? throw new ArgumentNullException(nameof(cLightningClient));
            NRustLightningHttpClient = nRustLightningHttpClient ?? throw new ArgumentNullException(nameof(nRustLightningHttpClient));
            NBXClient = nbxClient ?? throw new ArgumentNullException(nameof(nbxClient));
        }
        public readonly RPCClient BitcoinRPCClient;
        public readonly LndClient LndClient;
        public ILightningClient LndLNClient => (ILightningClient) LndClient;
        public readonly CLightningClient CLightningClient;
        public ILightningClient ClightningLNClient => (ILightningClient) CLightningClient;
        public readonly NRustLightningClient NRustLightningHttpClient;

        public readonly NBXplorer.ExplorerClient NBXClient;

        public async Task OutBoundConnectAll()
        {
            var clightningPeerInfo = await CLightningClient.ListPeersAsync();
            var lndPeerInfo = await LndClient.SwaggerClient.ListPeersAsync();

            var nrlInfo = await NRustLightningHttpClient.GetInfoAsync();
            if (clightningPeerInfo.Any(x => x.Id.Equals(nrlInfo.ConnectionString.NodeId.ToHex()))
                && lndPeerInfo.Peers.Any(x => x.Pub_key.Equals(nrlInfo.ConnectionString.NodeId.ToHex())))
                return;

            var lndConnString = (await LndLNClient.GetInfo()).NodeInfoList.First().ToConnectionString();
            var clightningConnString = (await ClightningLNClient.GetInfo()).NodeInfoList.First().ToConnectionString();
            await NRustLightningHttpClient.ConnectAsync(lndConnString);
            await NRustLightningHttpClient.ConnectAsync(clightningConnString);

            await Support.Utils.Retry(4, TimeSpan.FromSeconds(2), async () =>
            {
                clightningPeerInfo = await CLightningClient.ListPeersAsync();
                lndPeerInfo = await LndClient.SwaggerClient.ListPeersAsync();
                nrlInfo = await NRustLightningHttpClient.GetInfoAsync();
                return
                    nrlInfo.NodeIds.Count >= 2 &&
                    clightningPeerInfo.Any(x => x.Id.Equals(nrlInfo.ConnectionString.NodeId.ToHex())) &&
                    lndPeerInfo.Peers.Any(x => x.Pub_key.Equals(nrlInfo.ConnectionString.NodeId.ToHex()));
            }, "Failed to establish outbound connection");
        }

        public async Task PrepareFunds()
        {
            var nrlBalance = (await NRustLightningHttpClient.GetWalletInfoAsync()).OnChainBalanceSatoshis;
            if (nrlBalance > 2000000)
            {
                return;
            }
            
            var clAddress = (await CLightningClient.NewAddressAsync());
            var lndAddress = BitcoinAddress.Create((await LndClient.SwaggerClient.NewWitnessAddressAsync()).Address, Network.RegTest);
            var nrlAddress = (await NRustLightningHttpClient.GetNewDepositAddressAsync()).Address;
            foreach (var addr in new[] {clAddress, lndAddress, nrlAddress})
            {
                await this.BitcoinRPCClient.SendToAddressAsync(addr,Money.Coins(3m));
                await this.BitcoinRPCClient.SendToAddressAsync(addr,Money.Coins(3m));
                await this.BitcoinRPCClient.SendToAddressAsync(addr,Money.Coins(3m));
            }

            await this.BitcoinRPCClient.GenerateAsync(20);
            // check wallet info and nbxplorer info is synchronized.
            await Support.Utils.Retry(5, TimeSpan.FromSeconds(3), async () =>
            {
                var walletInfo = await this.NRustLightningHttpClient.GetWalletInfoAsync();
                if (walletInfo.OnChainBalanceSatoshis == 0)
                    return false;
                var explorerInfo = await NBXClient.GetBalanceAsync(walletInfo.DerivationStrategy);
                return (NBitcoin.Money.Satoshis(walletInfo.OnChainBalanceSatoshis).Equals(explorerInfo.Total));
            }, "walletinfo in nbxplorer and nrustlightning not synchronized");
            await WaitChainSync();
        }
        
        private async Task WaitChainSync()
        {
            await Support.Utils.Retry(6, TimeSpan.FromSeconds(3), async () =>
            {
                var blockHeights =
                    await Task.WhenAll(
                        Task.Run(async () =>
                        {
                            var info = await LndClient.SwaggerClient.GetInfoAsync();
                            return info.Block_height.Value;
                        }),
                        Task.Run(async () =>
                        {
                            var i = await CLightningClient.GetInfoAsync();
                            return (long)i.BlockHeight;
                        }),
                        Task.Run(async () =>
                        {
                            var i = await NBXClient.GetStatusAsync();
                            return (long)i.ChainHeight;
                        }),
                        Task.Run(async () =>
                        {
                            var i = await BitcoinRPCClient.GetBlockchainInfoAsync();
                            return (long) i.Blocks;
                        })
                    );
                return !(blockHeights.Any(h => h != blockHeights[0]));
            }, "Block height never synchronized!");
            await Task.Delay(1000);

        }

        public async Task CreateEnoughTxToEstimateFee()
        {
            try
            {
                await this.NBXClient.GetFeeRateAsync(30);
            }
            catch
            {

                var txPerBlock = 6;
                var nBlock = 6;
                for (int i = 0; i < nBlock; i++)
                {
                    for (int j = 0; j < txPerBlock; j++)
                    {
                        var addr = new Key().PubKey.GetSegwitAddress(Network.RegTest);
                        await this.BitcoinRPCClient.SendToAddressAsync(addr, Money.Coins(0.1m));
                    }

                    await BitcoinRPCClient.GenerateAsync(1);
                }

                await WaitChainSync();
            }
        }
    }
}