module NRustLightning.GUI.Client.Wallet.WalletInfoModule

open System.Text
open Elmish
open NRustLightning.GUI.Client.Wallet.Utils
open NBitcoin
open DotNetLightning.Utils

open Bolero.Html

type WalletInfo = {
    Id: WalletId
    OnChainBalance: Money
    OffChainBalance: LNMoney
    CryptoCode: string
    Name: string
}

type Model = {
    Id: WalletId option
    OnChainBalance: Money option
    OffChainBalance: LNMoney option
}

type Msg =
    | NoOp
    
let init = {
    Id = None
    OnChainBalance = None
    OffChainBalance = None
}

let update msg model =
    match msg with
    | NoOp -> model, Cmd.none


let view _model _dispatch =
    empty

