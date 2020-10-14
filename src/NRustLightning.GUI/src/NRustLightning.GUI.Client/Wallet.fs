module NRustLightning.GUI.Client.Wallet

open NBitcoin
open Bolero.Remoting
open DotNetLightning.Utils
open System.Threading.Tasks


type WalletService = {
    GetBalance: unit -> Task<Money * LNMoney>
}
    with
    interface IRemoteService with
        member this.BasePath = "/wallet"

type Model = {
    OnChainBalance: Money
    OffChainBalance: LNMoney
}
type Msg =
    | NoOp