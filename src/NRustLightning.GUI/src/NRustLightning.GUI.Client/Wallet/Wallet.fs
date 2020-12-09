[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletModule

open Bolero.Remoting
open Bolero
open Bolero.Html
open Bolero.Templating.Client
open Elmish

type Model = {
    WalletInfos: WalletInfoModule.Model seq
}

type Msg =
    | NoOp
    
let init = {
    WalletInfos = [||]
}

let update _service msg (model: Model) =
    match msg with
    | NoOp -> model, Cmd.none
    
    
let view (_model: Model) (_dispatch) =
    div [attr.classes ["mat-layout-grid"]] [
        div [attr.classes ["mat-layout-grid-inner"; "mat-elevation-3"] ] [
            text "TODO: show wallet info"
        ]
    ]
    
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        let service = this.Remote<WalletService>()
        Program.mkProgram (fun _ -> init, Cmd.ofMsg (NoOp)) (update service) view
#if DEBUG
        |> Program.withHotReload
#endif
