[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletImportModule

open Elmish
open System
open System.Text
open Bolero
open Bolero.Html
open DotNetLightning.Crypto
open MatBlazor
open Microsoft.AspNetCore.Components.Web
open NBitcoin

type MnemonicInProgress =
    | InEdit of list<string>
    | WaitingPin of mnemonic: Mnemonic * pin1: string * pin2 : string
type Model = {
    Mnemonic: MnemonicInProgress
    DialogIsOpen: bool
    SnackbarIsOpen: bool
    ErrorMsg: string option
}

type Msg =
    | UpdateMnemonic of index: int * content: string
    | CommitMnemonic
    | SetPin of int * string
    | CancelPin
    | CommitPin
    | CipherSeedCreated of CipherSeed
    | CipherSeedImported
    | NoOp
    
let init = {
    Mnemonic = InEdit [ for _ in 0..23 -> "" ]
    DialogIsOpen = false
    SnackbarIsOpen = false
    ErrorMsg = None
}


type Args = {
    Toaster: IMatToaster
    Service: WalletService
}
let update { Toaster = toaster; Service = service } msg model =
    match msg with
    | UpdateMnemonic (i, content) ->
        let newM = model.Mnemonic |> function InEdit l -> l |> List.mapi(fun index m -> if index = i then content else m ) |> InEdit | el ->  el
        { model with Mnemonic = newM }, Cmd.none
    | CommitMnemonic ->
        match model.Mnemonic with
            | InEdit l ->
                let joined = String.Join(' ', l)
                try
                    let m = Mnemonic(joined, Wordlist.English)
                    toaster.Clear()
                    { model with Mnemonic = WaitingPin(m, "", ""); DialogIsOpen = true }, Cmd.none
                with
                | ex ->
                    let msg = sprintf "Invalid Mnemonic (%A)\n %s" (joined) (ex.Message)
                    toaster.Add(msg, MatToastType.Warning, "Invalid Mnemonic!") |> ignore
                    model, Cmd.none
            | WaitingPin _ -> { model with DialogIsOpen = true }, Cmd.none
    | SetPin(i, str) ->
        match model.Mnemonic with
        | WaitingPin (m, pin1, pin2) ->
            let newPin1 = if i = 1 then str else pin1
            let newPin2 = if i = 2 then str else pin2
            { model with Mnemonic = WaitingPin(m, newPin1, newPin2) }, Cmd.none
        | _ -> model, Cmd.none
    | CancelPin ->
        { model with DialogIsOpen = false }, Cmd.none
    | CommitPin ->
        match model.Mnemonic with
        | WaitingPin (m, pin1, pin2) ->
            if (pin1 <> pin2) then
                toaster.Add("Two Passwords are different", MatToastType.Warning) |> ignore
                model, Cmd.none
            else
                try
                    match m.ToCipherSeed(Encoding.UTF8.GetBytes pin1) with
                    | Ok cipherSeed ->
                        toaster.Clear()
                        { model with DialogIsOpen = false }, Cmd.ofMsg (CipherSeedCreated cipherSeed)
                    | Error (e) ->
                        toaster.Add((e.ToString()), MatToastType.Warning, "Invalid Password!") |> ignore
                        model, Cmd.none
                with
                | ex ->
                    toaster.Add((ex.ToString()), MatToastType.Warning, "Invalid Password!") |> ignore
                    model, Cmd.none
        | _ -> model, Cmd.none
    | CipherSeedCreated cipherSeed ->
        let onSuccess _ =
            toaster.Clear()
            CipherSeedImported
        let onError e =
            toaster.Add(e.ToString(), MatToastType.Warning, "Failed to import this cipher seed") |> ignore
            NoOp
        model, Cmd.OfAsync.either(service.TrackCipherSeed) cipherSeed onSuccess onError
    | CipherSeedImported ->
        { model with SnackbarIsOpen = true }, Cmd.none
    | NoOp -> model, Cmd.none

let private mnemonicWordList =
    Wordlist.English.GetWords()
let view model dispatch =
    concat [
        cond model.Mnemonic <|
        function
            | InEdit mnemonic -> 
                div [] [
                    h1 [] [text "Please Enter your aezeed mnemonic to recover your wallet"]
                    div [attr.style "display: flex; flex-direction: row; flex-wrap: wrap"] [
                        for i in 0..23 ->
                            div [attr.classes ["mat-elevation-z2"]; attr.style "width: 30vh; margin: 8px 5px"] [
                                comp<MatAutocompleteList<string>> ["Items" => mnemonicWordList
                                                                   "TItem" => string
                                                                   "Label" => sprintf "Enter %dth mnemonic" (i + 1)
                                                                   attr.callback "OnTextChanged" (fun (v: string) -> dispatch(UpdateMnemonic(i, v)))
                                                                   ] [
                                ]
                            ]
                    ]
                    comp<MatButton> ["Label" => "Ok"
                                     on.click(fun _ -> dispatch(CommitMnemonic)); "Raised" => true
                                     "Disabled" => (mnemonic |> Seq.exists(String.IsNullOrWhiteSpace))
                                     ] []
                ]
            | WaitingPin (_mnemonic, pin1, pin2) ->
                comp<MatDialog> ["IsOpen" => model.DialogIsOpen ] [
                    comp<MatDialogTitle> [] [
                        comp<MatDialog> [] [text "Enter your password"]
                        comp<MatDialogContent> [] [
                            comp<MatTextField<string>> [bind.input.string pin1 (fun s -> SetPin(1, s) |> dispatch); "Label" => "Enter your password"; "Type"=> "Password"] []
                            br []
                            comp<MatTextField<string>> [bind.input.string pin2 (fun s -> SetPin(2, s) |> dispatch); "Label" => "Enter the same password again"; "Type" => "Password"] []
                            comp<MatDialogActions> []  [
                                comp<MatButton> [attr.callback "OnClick" (fun (_: MouseEventArgs) -> CancelPin |> dispatch)] [text "Cancel"]
                                comp<MatButton> [attr.callback "OnClick" (fun (_: MouseEventArgs) -> CommitPin |> dispatch)
                                                 "Disabled" => (String.IsNullOrWhiteSpace(pin1) || String.IsNullOrWhiteSpace pin2)] [text "Ok"]
                            ]
                        ]
                    ]
                ]
        comp<MatSnackbar> ["IsOpen" => model.SnackbarIsOpen] [
            comp<MatSnackbarContent> [] [
                text "seed imported successfully! Now scanning..."
            ]
        ]
    ]
type EApp() =
    inherit ElmishComponent<Model, Msg>()
        
    override this.View model dispatch =
        view model dispatch