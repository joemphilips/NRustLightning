namespace NRustLightning.GUI.Server

open System.Text.Json
open FSharp.Control.Tasks
open NBitcoin
open Microsoft.AspNetCore.Hosting
open Bolero.Remoting
open Bolero.Remoting.Server
open NRustLightning.Infrastructure
open NRustLightning.Infrastructure.Repository
open NRustLightning.Infrastructure.Networks

type WalletService(ctx: IRemoteContext, env: IWebHostEnvironment, network: NRustLightningNetwork) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Wallet.WalletService>()

    let serializerOptions = JsonSerializerOptions()
    let repoSerializer = RepositorySerializer(network)
    do repoSerializer.ConfigureSerializer(serializerOptions)

    override this.Handler = {
        GetBalance = fun _ -> task {
            return failwith ""
        }
    }