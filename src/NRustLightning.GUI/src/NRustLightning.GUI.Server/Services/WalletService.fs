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
open NBitcoin.Scripting
open NRustLightning.GUI.Client.Configuration
open NRustLightning.GUI.Server
open NRustLightning.Infrastructure.Interfaces
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Networks

type WalletService(ctx: IRemoteContext, env: IWebHostEnvironment,
                   networkProvider: NRustLightningNetworkProvider, opts: IOptionsMonitor<WalletBiwaConfiguration>,
                   keysRepository: IKeysRepository,
                   repository: Repository, rpcClientProvider: RPCClientProvider) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Wallet.WalletService>()

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
        TrackCipherSeed = fun cipherSeed -> async {
            let cryptoCode = "btc" // todo: support altchains?
            let n = networkProvider.GetByCryptoCode(cryptoCode)
            let rpc = rpcClientProvider.GetRPCClient(cryptoCode).Value
            let extKey = ExtKey(cipherSeed.Entropy)
            let path = (n.BaseKeyPath.ToString() + "0'") |> KeyPath
            let accountBasePrivKey = extKey.Derive(path)
            let accountBase = accountBasePrivKey.Neuter() |> fun x -> BitcoinExtPubKey(x, n.NBitcoinNetwork)
            let od =
                PubKeyProvider.HD(accountBase, path, PubKeyProvider.DeriveType.HARDENED)
                |> fun x -> PubKeyProvider.NewOrigin(RootedKeyPath(extKey.ParentFingerprint, path), x)
                |> OutputDescriptor.NewWPKH
            let repo = FlatSigningRepository()
            let importAddresses =
                let r = ResizeArray()
                let arg = ImportMultiAddress()
                arg.Desc <- od
                r.Add(arg)
                r.ToArray()
            let! resp = rpc.ImportMultiAsync(importAddresses, true, repo) |> Async.AwaitTask
            return failwith "TODO"
        }
    }