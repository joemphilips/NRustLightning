module NRustLightning.GUI.Client.Wallet

open NBitcoin
open Bolero.Remoting
open DotNetLightning.Utils
open System.Threading.Tasks


type WalletService = {
    GetBalance: string -> Async<Money * LNMoney>
}
    with
    interface IRemoteService with
        member this.BasePath = "/wallet"

type Model = {
    OnChainBalance: Money
    OffChainBalance: LNMoney
}

let init = {
    OnChainBalance = Money.Zero
    OffChainBalance = LNMoney.Zero
}

type Msg =
    | NoOp
    
let update msg model =
    match msg with
    | NoOp -> model