namespace NRustLightning.GUI.Client.Wallet

open Bolero.Remoting
open DotNetLightning.Crypto
open NBitcoin
open DotNetLightning.Utils
open NRustLightning.GUI.Client.Wallet

type WalletService = {
    GetBalance: WalletId -> Async<(Money * LNMoney) option>
    GetWalletInfo: WalletId -> Async<(WalletInfo) option>
    TrackNewWallet: string * CipherSeed -> Async<Result<unit, string>>
}
    with
    interface IRemoteService with
        member this.BasePath = "/wallet"

