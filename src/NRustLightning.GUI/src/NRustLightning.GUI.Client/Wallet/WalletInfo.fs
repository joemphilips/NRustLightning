[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletInfoModule

open Bolero
open Elmish
open Microsoft.AspNetCore.Blazor.Components
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
    Id: WalletId
    OnChainBalance: Money option
    OffChainBalance: LNMoney option
}

type Msg =
    | NoOp
    
let update msg model =
    match msg with
    | NoOp -> model


let view _model _dispatch =
    text "TODO: show wallet info"

type App() =
    inherit ProgramComponent<Model, Msg>()
    
    [<Parameter>]
    member val Id = Unchecked.defaultof<WalletId> with get, set
    
    override this.Program =
        Program.mkSimple(fun _ -> { Model.Id = this.Id; OnChainBalance = None; OffChainBalance = None }) update view