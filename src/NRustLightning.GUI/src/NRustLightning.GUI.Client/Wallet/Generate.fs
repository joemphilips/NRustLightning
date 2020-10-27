[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletGenerateModule

open Elmish
open Bolero
open Bolero.Html

type Model = {
    Foo: string
}

type Msg =
    | NoOp

let init = { Foo = "" }

let update msg model =
    match msg with
    | NoOp -> model
    
let view _model _dispatch =
    text "TODO"
    
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        Program.mkSimple (fun _ -> init) update view