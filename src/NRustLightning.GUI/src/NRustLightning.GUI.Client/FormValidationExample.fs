module NRustLightning.GUI.Client.FormValidationExample

open System.Net.Http
open System.Threading.Tasks
open Bolero
open Elmish

// based on: https://github.com/lucamug/elm-form-examples/blob/master/src/Example_18.elm
let exampleVersion = "18"

type FormState = private Editing | Fetching
type FormFieldType =
    | Email
    | Password
type Model = {
    Errors: string list
    Email: string
    Password: string
    Fruits: Map<string, bool>
    Response: string option
    Focus: FormFieldType option
    ShowErrors: bool
    ShowPassword: bool
    FormState: FormState
}
    with
    member this.GetField(f: FormFieldType) =
        match f with
        | Email -> this.Email
        | Password -> this.Password
    member this.SetField (f: FormFieldType) v =
        match f with
        | Email ->  { this with Email = v }
        | Password -> { this with Password = v }

let init =
    {
        Errors = []
        Email = ""
        Password = ""
        Fruits =
            Map.empty
            |> Map.add ("Apple") false
            |> Map.add ("Banana") false
            |> Map.add "Orange" false
            |> Map.add "Pear" false
            |> Map.add "Strawberry" false
            |> Map.add "Cherry" false
            |> Map.add "Grapes" false
            |> Map.add "Watermelon" false
            |> Map.add "Pineapple" false
        Response = None
        Focus = None
        ShowErrors = false
        ShowPassword = false
        FormState = Editing
    }
type Error = FormFieldType * string
type Fruit = string

type Msg =
    | NoOp
    | SubmitForm
    | SetField of FormFieldType * string
    | Response of Result<string, HttpRequestException>
    | OnFocus of FormFieldType
    | OnBlur of FormFieldType
    | ToggleShowPassword
    | ToggleFruit of Fruit
    
[<AutoOpen>]
module private Helpers =
    open System
    open ResultUtils
    let validate (model: Model) =
        let validateEmail (x: string) =
            if (x |> String.IsNullOrWhiteSpace) then Error "Email cannot be blank" else Ok()
        let validatePassword (x: string) =
            if (x |> String.IsNullOrWhiteSpace) then Error "Password cannot be blank" else Ok()
        Ok()
        *^> (validateEmail model.Email)
        *^> (validatePassword model.Password)
   
   
    // tasks
    let postRequest (model: Model): Task<Result<string, HttpRequestException>> =
         failwith ""
        
    let setField (f: FormFieldType) v model: Model =
        match f with
        | Email _ ->  { model with Email = v }
        | Password _ -> { model with Password = v }
        | _ -> model
           
    let setErrors model: Model =
        match validate model with
        | Ok _ -> model
        | Error e -> { model with Errors = e }
       
    let toggle (fruit: Fruit) (fruits: Map<string, bool>) =
        fruits |> Map.map(fun k oldValue -> if k <> fruit then oldValue else oldValue |> not)
           
let update msg model =
    match msg with
    | NoOp -> model, Cmd.none
    | SubmitForm ->
        match validate model with
        | Ok _ ->
            { model with Errors = []; Response = None; FormState = Fetching },
            Cmd.OfTask.perform postRequest model (Response)
        | Error e ->
            { model with Errors = e; ShowErrors = true }, Cmd.none
    | SetField (f, v) ->
        model |> setField f v |> setErrors
        , Cmd.none
    | Response(Ok resp) ->
        { model with Response = Some resp; FormState = Editing }, Cmd.none
    | Response(Error e) ->
        { model with Response = Some(e.ToString()); FormState = Editing }, Cmd.none
    | OnFocus formField ->
        { model with Focus = Some (formField) }, Cmd.none
    | OnBlur formField ->
        { model with Focus = None }, Cmd.none
    | ToggleShowPassword ->
        { model with ShowPassword = not model.ShowPassword }, Cmd.none
    | ToggleFruit fruit ->
        { model with Fruits = toggle fruit model.Fruits }, Cmd.none


open Bolero.Html
open Bolero.Html

type Main = Template<"wwwroot/main.html">

let private viewInput model formField inputType inputName dispatch =
    let hasFocus =
        match model.Focus with
        | Some (x) when x = formField -> true
        | _ -> false
    let content =
        match formField with
        | FormFieldType.Email -> model.Email
        | FormFieldType.Password -> model.Password
        
    label
        []
        [ div
            [ attr.``class`` "inputFieldContainer" ]
            [ input
                [
                    if formField = FormFieldType.Password && model.ShowPassword then attr.``type`` "text" else attr.``type`` inputType
                    attr.classes [ "focus" ]
                    on.input (fun e -> SetField (formField, unbox e.Value) |> dispatch)
                    on.focus (fun _ -> OnFocus formField |> dispatch)
                    on.blur (fun _ -> OnBlur formField |> dispatch)
                    bind.input.string (model.GetField(formField)) (fun n -> (formField, n) |> SetField |> dispatch)
                ]
              div [] []
            ]
        ]
let private viewForm model dispatch =
    div
        [ on.keyup(fun k -> if k.Key = "enter" then SubmitForm |> dispatch else ()) ]
        [
            style [] [ text "" ]
            viewInput model (FormFieldType.Email) "text" "Email" dispatch
            viewInput model (FormFieldType.Password) "text" "Password" dispatch
        ]
let view (model: Model) (dispatch: Dispatch<Msg>): Node = Utils.viewUtils model model.Response exampleVersion (fun x -> viewForm x dispatch)

type FormValidationComponent() =
    inherit ElmishComponent<Model, Msg>()
    
    override this.View model dispatch = view model dispatch