[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.InitializationModule

open System.Threading.Tasks
open Bolero
open Bolero.Html
open Elmish
open MatBlazor
open Microsoft.AspNetCore.Blazor.Components
open NBitcoin

type Model = {
    Mnemonic: Mnemonic option
}

type Msg =
    | NoOp
    
let init = {
    Mnemonic = None
}

let update msg model =
    match msg with
    | NoOp -> model


let view _model _dispatch =
    comp<MatCard> [] []

type App() =
    inherit ProgramComponent<Model, Msg>()
    
    override this.Program =
        Program.mkSimple(fun _ -> init) update view