[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Configuration.ConfigurationModule

open System
open System.Linq
open Elmish
open Humanizer
open MatBlazor
open Bolero
open Bolero.Remoting
open Bolero.Html
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Web
open NBitcoin
open NRustLightning.GUI.Client.Utils
open NRustLightning.GUI.Client.Components
open Bolero.Templating.Client

type ConfigurationService = {
    LoadConfig: unit -> Async<WalletBiwaConfiguration>
    Update: WalletBiwaConfiguration -> Async<Result<unit, string>>
}
    with
    interface IRemoteService with
        member this.BasePath = "/config-service"

type Model = {
    Configuration: Deferred<WalletBiwaConfiguration>
    ErrorMsg: string option
}

type Msg =
    | RPCHostInput of string
    | RPCPortInput of int
    | RPCPasswordInput of string
    | RPCUserInput of string
    | RPCCookiefileInput of string
    | NetworkInput of Network
    | InvalidInput of err: string
    | ApplyChanges
    | LoadConfig of AsyncOperationStatus<WalletBiwaConfiguration>
    | NoOp
    
let init = {
    Configuration = HasNotStartedYet
    ErrorMsg = None
}
    
let private updateConfig (update: WalletBiwaConfiguration -> WalletBiwaConfiguration) (model) =
    { model with Configuration = model.Configuration |> Deferred.map(update) }, Cmd.none
let update (service: ConfigurationService) msg model =
    match msg, model with
    | LoadConfig Started, { Configuration = HasNotStartedYet } ->
        let onSuccess = Finished >> LoadConfig
        { model with Configuration = InProgress }, Cmd.OfAsync.perform (service.LoadConfig) () (onSuccess)
    | LoadConfig Started, _ ->
        model, Cmd.none
    | LoadConfig(Finished c), _ ->
        { model with Configuration = Resolved(c) }, Cmd.none
    | RPCHostInput r, _ ->
        updateConfig (fun x -> { x with RPCHost = r }) model
    | RPCPortInput r, _ ->
        updateConfig (fun x -> { x with RPCPort = r }) model
    | RPCCookiefileInput r, _  ->
        updateConfig (fun x -> { x with RPCCookieFile = r }) model
    | RPCPasswordInput s, _ ->
        updateConfig (fun x -> { x with RPCPassword = s }) model
    | RPCUserInput s, _ ->
        updateConfig (fun x -> { x with RPCUser = s }) model
    | NetworkInput n, _ ->
        updateConfig (fun x -> { x with Network = n.ToString() }) model
    | ApplyChanges, { Configuration = Resolved(conf) } ->
        let onSuccess = function
            | Ok _ -> NoOp
            | Error x ->
                InvalidInput(x)
        { model with ErrorMsg = None },
        Cmd.OfAsync.perform
            (service.Update)
            (conf)
            (onSuccess)
    | ApplyChanges, _ -> model, Cmd.none
    | NoOp, _ -> model, Cmd.none
    | InvalidInput exn, _ ->
        { model with ErrorMsg = exn |> Some }, Cmd.none
        
        
let view (model: Model) dispatch =
    cond model.Configuration <| function
        | HasNotStartedYet -> empty
        | InProgress -> spinner
        | Resolved t ->
            div [] [
                comp<MatTextField<string>> [
                    "Value" => t.RPCHost
                    "Label" => "hostname for bitcoin rpc"
                    attr.callback "OnInput"
                        (fun (e: ChangeEventArgs) -> dispatch(RPCHostInput(unbox e.Value)))
                ] []
                comp<MatDivider> [] []
                comp<MatTextField<int>> [
                    "Value" => t.RPCPort
                    "Label" => "port for bitcoin rpc"
                    attr.callback "OnInput"
                        (fun (e: ChangeEventArgs) -> e.Value |> unbox |> RPCPortInput |> dispatch)
                ] []
                comp<MatDivider> [] []
                comp<MatTextField<string>> [
                    "Value" => t.RPCUser
                    "Label" => "username for bitcoin rpc"
                    attr.callback "OnInput"
                        (fun (e: ChangeEventArgs) -> e.Value |> unbox |> RPCUserInput |> dispatch)
                ] []
                comp<MatDivider> [] []
                comp<MatTextField<string>> [
                    "Value" => t.RPCPassword
                    "Label" => "password for bitcoin rpc"
                    attr.callback "OnInput"
                        (fun (e: ChangeEventArgs) -> e.Value |> unbox |> RPCPasswordInput |> dispatch)
                ] []
                comp<MatDivider> [] []
                
                comp<MatTextField<string>> [
                    "Value" => t.RPCCookieFile
                    "Label" => "Path to the Cookiefile for bitcoind"
                    attr.callback "OnInput" (fun (e: ChangeEventArgs) -> e.Value |> unbox |> RPCCookiefileInput |> dispatch)
                ] []
                comp<MatDivider> [] []
                
                comp<MatSelectItem<Network>> ["Items" => Network.GetNetworks().ToArray()
                                              "Label" => "Bitcoin Network"
                                              "Value" => t.GetNetwork()
                                              attr.callback "ValueChanged" (NetworkInput >> dispatch)] []
                
                cond <| (t.Validate()) <| function
                    | None ->
                        empty
                    | Some e ->
                        div [attr.style "color: red"] [
                            textf "Invalid Config: %s" (e)
                        ]
                cond <| (model.ErrorMsg) <| function
                    | None -> empty
                    | Some e ->
                        div [attr.style "color: red"] [
                            textf "Failed to validate config: %s" (e)
                        ]
                comp<MatDivider> [] []
                comp<MatButton> [
                    "Disabled" => t.Validate().IsSome
                    attr.callback "OnClick" (fun (_e: MouseEventArgs) -> ApplyChanges |> dispatch)
                ] [ text "Commit" ]
            ]
            
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        let service = this.Remote<ConfigurationService>()
        Program.mkProgram (fun _ -> init, Cmd.ofMsg (LoadConfig Started)) (update service) view
#if DEBUG
        |> Program.withHotReload
        |> Program.withConsoleTrace
#endif
