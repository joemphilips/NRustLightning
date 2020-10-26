namespace NRustLightning.GUI.Server.Services

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
open NRustLightning.GUI.Server
open NRustLightning.Infrastructure
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Networks

type WalletService(ctx: IRemoteContext, env: IWebHostEnvironment,
                   networkProvider: NRustLightningNetworkProvider, opts: IOptionsMonitor<WalletBiwaConfiguration>,
                   repository: Repository, rpcClientProvider: RPCClientProvider) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Wallet.WalletModule.WalletService>()

    let serializerOptions = JsonSerializerOptions()
    let repoSerializer = RepositorySerializer(networkProvider.GetByCryptoCode"btc")
    do repoSerializer.ConfigureSerializer(serializerOptions)
    
    override this.Handler = {
        GetBalance = fun walletId -> async {
            let! info = repository.GetWalletInfo(walletId) |> Async.AwaitTask
            match info with
            | None -> return None
            | Some info ->
                let rpc = rpcClientProvider.GetRPCClient(info.CryptoCode)
                match rpc with
                | None -> return None
                | Some rpc ->
                    let! b = rpc.GetBalanceAsync() |> Async.AwaitTask
                    return (b, LNMoney.Zero) |> Some
        }
        GetWalletInfo = fun walletId -> async {
            return! repository.GetWalletInfo(walletId) |> Async.AwaitTask
        }
    }