[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Configuration.ConfigurationModule

open Elmish
open Humanizer
open MatBlazor
open Bolero
open Bolero.Remoting
open Bolero.Html
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Web
open NRustLightning.GUI.Client.Utils
open NRustLightning.GUI.Client.Components
open Bolero.Templating.Client

type ConfigurationService = {
    LoadConfig: unit -> Async<WalletBiwaConfiguration>
    Update: WalletBiwaConfiguration -> Async<Result<unit, string>>
}
    with
    interface IRemoteService with
        member this.BasePath = "/config"

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
    | InvalidInput of exn
    | TryApplyChanges
    | ApplyChanges
    | LoadConfig of AsyncOperationStatus<WalletBiwaConfiguration>
    
let init = {
    Configuration = HasNotStartedYet
    ErrorMsg = None
}
    
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
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCHost = r })
        { model with Configuration = newConf }, Cmd.none
    | RPCPortInput r, _ ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCPort = r })
        { model with Configuration = newConf }, Cmd.none
    | RPCCookiefileInput r, _  ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCCookieFile = r })
        { model with Configuration = newConf }, Cmd.none
    | RPCPasswordInput s, _ ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCPassword = s })
        { model with Configuration = newConf }, Cmd.none
    | RPCUserInput s, _ ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCUser = s })
        { model with Configuration = newConf }, Cmd.none
    | TryApplyChanges, { Configuration = Resolved conf } ->
        let onSuccess _ = ApplyChanges
        let onError = InvalidInput
        model, Cmd.OfAsync.either (conf.ValidateAsync) () onSuccess onError
    | ApplyChanges, { Configuration = Resolved(conf) } ->
        let onError = InvalidInput
        { model with ErrorMsg = None },
        Cmd.OfAsync.attempt
            (service.Update)
            (conf)
            (onError)
    | ApplyChanges, _ -> model, Cmd.none
    | InvalidInput exn, _ ->
        { model with ErrorMsg = exn.ToString() |> Some }, Cmd.none
        
        
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
                cond <| (t.Validate(), model.ErrorMsg) <| function
                    | None, None ->
                        empty
                    | Some e, _ ->
                        div [attr.style "color: red"] [
                            textf "Invalid Config: %s" (e)
                        ]
                    | _, Some e ->
                        div [attr.style "color: red"] [
                            textf "Failed to validate config: %s" (e)
                        ]
                        
                comp<MatDivider> [] []
                comp<MatButton> [
                    "Disabled" => t.Validate().IsSome
                    attr.callback "OnClick" (fun (_e: MouseEventArgs) -> TryApplyChanges |> dispatch)
                ] [ text "Commit" ]
            ]
            
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        let service = this.Remote<ConfigurationService>()
        Program.mkProgram (fun _ -> init, Cmd.ofMsg (LoadConfig Started)) (update service) view
#if DEBUG
        |> Program.withHotReload
#endif
