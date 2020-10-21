namespace NRustLightning.GUI.Server

open System.Net.Http
open System.Text.Json
open DotNetLightning.Utils
open FSharp.Control.Tasks
open Microsoft.Extensions.Options
open NBitcoin
open Microsoft.AspNetCore.Hosting
open Bolero.Remoting
open Bolero.Remoting.Server
open NBitcoin.RPC
open NRustLightning.GUI.Client.Configuration
open NRustLightning.Infrastructure
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Networks

type WalletService(ctx: IRemoteContext, env: IWebHostEnvironment,
                   networkProvider: NRustLightningNetworkProvider, opts: IOptionsMonitor<WalletBiwaConfiguration>,
                   httpClientFactory: IHttpClientFactory) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Wallet.WalletService>()

    let serializerOptions = JsonSerializerOptions()
    let repoSerializer = RepositorySerializer(networkProvider.GetByCryptoCode"btc")
    do repoSerializer.ConfigureSerializer(serializerOptions)

    override this.Handler = {
        GetBalance = fun cryptoCode -> async {
            let n = networkProvider.GetByCryptoCode(cryptoCode)
            let rpc =
                opts.CurrentValue.GetRPCClient(n.NBitcoinNetwork, httpClientFactory.CreateClient("WalletService:" + cryptoCode))
            let! b = rpc.GetBalanceAsync() |> Async.AwaitTask
            return (b, LNMoney.Zero)
        }
    }