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
                   opts: IOptionsMonitor<WalletBiwaConfiguration>,
                   keysRepository: IKeysRepository,
                   httpClientFactory: IHttpClientFactory,
                   repository: Repository) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Wallet.WalletService>()

    override this.Handler = {
        GetBalance = fun walletId -> async {
            let! info = repository.GetWalletInfo(walletId) |> Async.AwaitTask
            match info with
            | None -> return None
            | Some info ->
                let rpc = opts.CurrentValue.GetRPCClient(httpClientFactory.CreateClient"WalletService")
                let! b = rpc.GetBalanceAsync() |> Async.AwaitTask
                return (b, LNMoney.Zero) |> Some
        }
        GetWalletInfo = fun walletId -> async {
            return! repository.GetWalletInfo(walletId) |> Async.AwaitTask
        }
        TrackNewWallet = fun (name, cipherSeed) -> async {
            let n = opts.CurrentValue.GetNetwork()
            let rpc = opts.CurrentValue.GetRPCClient(httpClientFactory.CreateClient"WalletService")
            let extKey = ExtKey(cipherSeed.Entropy)
            let path = ("0'") |> KeyPath
            let accountBasePrivKey = extKey.Derive(path)
            let accountBase = accountBasePrivKey.Neuter() |> fun x -> BitcoinExtPubKey(x, n)
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