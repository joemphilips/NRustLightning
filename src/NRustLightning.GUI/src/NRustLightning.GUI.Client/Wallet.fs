module NRustLightning.GUI.Client.WalletModule

open NBitcoin
open Bolero
open Bolero.Remoting
open Bolero.Html
open Bolero.Templating.Client
open DotNetLightning.Utils
open System.Threading.Tasks
open MatBlazor
open Elmish

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
    
let update _service msg model =
    match msg with
    | NoOp -> model, Cmd.none
    
let view (_model: Model) (_dispatch) =
    div [attr.classes ["may-layout-grid"]] [
        div [attr.classes ["may-layout-grid-inner"] ] [
            div [attr.classes ["may-layout-grid-cell"] ] [
                text "TODO: show wallet info"
            ]
            div [attr.classes ["may-layout-grid-cell"] ] [
                text "TODO: show wallet info"
            ]
            div [attr.classes ["may-layout-grid-cell"] ] [
                text "TODO: show wallet info"
            ]
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
