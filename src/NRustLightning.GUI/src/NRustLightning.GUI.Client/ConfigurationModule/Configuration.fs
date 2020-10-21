[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Configuration.ConfigurationModule

open Elmish
open Bolero
open Bolero.Remoting
open Bolero.Html
open NRustLightning.GUI.Client.Utils
open NRustLightning.GUI.Client.Components

type ConfigurationService = {
    LoadConfig: unit -> Async<WalletBiwaConfiguration>
    Update: WalletBiwaConfiguration -> Async<unit>
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
    | ServiceFailed of exn
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
    | RPCPasswordInput s, _ ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCPassword = s })
        { model with Configuration = newConf }, Cmd.none
    | RPCUserInput s, _ ->
        let newConf = model.Configuration |> Deferred.map(fun x -> { x with RPCUser = s })
        { model with Configuration = newConf }, Cmd.none
    | ApplyChanges, { Configuration = Resolved(conf) } ->
        model,
        Cmd.OfAsync.attempt
            (service.Update)
            (conf)
            (ServiceFailed)
    | ApplyChanges, _ -> model, Cmd.none
    | ServiceFailed exn, _ ->
        { model with ErrorMsg = exn.ToString() |> Some }, Cmd.none
        
        
let view (model: Model) dispatch =
    cond model.Configuration <| function
        | HasNotStartedYet -> empty
        | InProgress -> spinner
        | Resolved t ->
            div [] [
                text "RPCHost"
                input [
                    bind.input.string t.RPCHost (RPCHostInput >> dispatch)
                ]
                br []
                text "RPCPort"
                input [
                    bind.input.int t.RPCPort (RPCPortInput >> dispatch)
                ]
                br []
                text "RPCUser"
                input [
                    bind.input.string t.RPCUser (RPCUserInput >> dispatch)
                ]
                br []
                text "RPCPassword"
                input [
                    bind.input.string t.RPCPassword (RPCPasswordInput >> dispatch)
                ]
            ]
            
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        let service = this.Remote<ConfigurationService>()
        Program.mkProgram (fun _ -> init, Cmd.ofMsg (LoadConfig Started)) (update service) view