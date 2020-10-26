[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletModule

open NBitcoin
open Bolero
open Bolero.Remoting
open Bolero.Html
open Bolero.Templating.Client
open DotNetLightning.Utils
open Elmish

open NRustLightning.GUI.Client.Wallet.Utils

type WalletService = {
    GetBalance: WalletId -> Async<(Money * LNMoney) option>
    GetWalletInfo: WalletId -> Async<(WalletInfoModule.WalletInfo) option>
}
    with
    interface IRemoteService with
        member this.BasePath = "/wallet"

type Page =
    | [<EndPoint "/">] Overview
    | [<EndPoint "/init">] Init of PageModel<InitializationModule.Model>
    | [<EndPoint "/{id}">] Info of WalletInfoModule.Model

type Model = {
    WalletInfos: WalletInfoModule.Model seq
    Page: Page
}

type Msg =
    | WalletInfoMsg of WalletId *  WalletInfoModule.Msg
    | SetPage of Page
    | NoOp
    
let init = {
    WalletInfos = [||]
    Page = Overview
}

let defaultModel = function
    | Info _ -> ()
    | Overview -> ()
    | Init model ->
        Router.definePageModel model { InitializationModule.Model.Mnemonic = None }
let router = Router.inferWithModel SetPage (fun m -> m.Page) defaultModel
    
let update _service msg (model: Model) =
    match msg with
    | WalletInfoMsg(wId, msg) ->
        let modelsAndCmds = model.WalletInfos |> Seq.map(fun w -> if w.Id = Some wId then WalletInfoModule.update msg w else (w, Cmd.none))
        let m = modelsAndCmds |> Seq.map fst
        let cmds = modelsAndCmds |> Seq.map snd
        { model with WalletInfos = m }, (Cmd.batch cmds |> Cmd.map WalletInfoMsg)
    | SetPage page ->
        { model with Page = page }, Cmd.none
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
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif
