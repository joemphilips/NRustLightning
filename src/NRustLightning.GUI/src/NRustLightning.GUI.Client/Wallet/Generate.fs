[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Wallet.WalletGenerateModule

open System.Diagnostics
open System.Text
open Bolero.Remoting.Client
open DotNetLightning.Crypto
open MatBlazor
open Elmish
open Bolero
open Bolero.Remoting
open Bolero.Html
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Web
open System
open NBitcoin

type Model = {
    WalletName: string
    Pass1: string
    Pass2: string
    ResultedMnemonic: Mnemonic option
    ErrorMsg: string option
}
    with
    member model.Validate() =
        if (String.IsNullOrWhiteSpace(model.Pass1) || String.IsNullOrWhiteSpace model.Pass2) then Some("Please enter password") else
        if (model.Pass1 <> model.Pass2) then Some("two passwords does not match!") else
        None

type Msg =
    | WalletNameInput of string
    | SetPass of int * string
    | Commit
    | Finished of mnemonic: Mnemonic
    | InvalidInput of string
    | ClearError

let init = { WalletName = String.Empty
             Pass1 = String.Empty
             Pass2 = String.Empty
             ResultedMnemonic = None
             ErrorMsg = None
             }

type Args = {
    Service: WalletService
}
let update { Service = service } msg model =
    match msg with
    | WalletNameInput str ->
        { model with WalletName = str }, Cmd.none
    | SetPass(i, str) ->
        if i = 1 then { model with Pass1 = str } else if (i = 2) then { model with Pass2 = str } else failwith "Unreachable!"
        , Cmd.none
    | Commit ->
        Debug.Assert(model.Pass1 = model.Pass2)
        let cipherSeed = CipherSeed.Create()
        let m = cipherSeed.ToMnemonic(Encoding.UTF8.GetBytes model.Pass1)
        let onSuccess = function
            | Ok _ ->
                Finished m
            | Error e ->
                InvalidInput (e)
        model, Cmd.OfAsync.perform service.TrackNewWallet (model.WalletName, cipherSeed) onSuccess
    | InvalidInput errorMsg ->
        { model with ErrorMsg = Some(errorMsg) }, Cmd.none
    | Finished mnemonic ->
        { model with ResultedMnemonic = Some (mnemonic) }, Cmd.ofMsg (ClearError)
    | ClearError -> { model with ErrorMsg = None }, Cmd.none
    
let view model dispatch =
    concat [
        h1 [] [text "Please Enter your Wallet Name and the password for encrypting the key"]
        comp<MatDivider> [] []
        comp<MatTextField<string>> ["Label" => "Your wallet name"
                                    "Value" => model.WalletName
                                    attr.callback "OnInput" (fun (e: ChangeEventArgs) -> e.Value |> unbox |> WalletNameInput |> dispatch)] []
        comp<MatDivider> [] []
        comp<MatTextField<string>> [bind.input.string model.Pass1 (fun s -> SetPass(1, s) |> dispatch)
                                    "Label" => "Enter your password"
                                    "Type"=> "Password"] []
        br []
        comp<MatTextField<string>> [bind.input.string model.Pass2 (fun s -> SetPass(2, s) |> dispatch)
                                    "Label" => "Enter the same password again"
                                    "Type" => "Password"] []
        comp<MatDialogActions> []  [
            comp<MatButton> [attr.callback "OnClick" (fun (_: MouseEventArgs) -> Commit |> dispatch)
                             "Disabled" => (String.IsNullOrWhiteSpace(model.Pass1) || String.IsNullOrWhiteSpace model.Pass2)] [text "Ok"]
        ]
    ]
    
type App() =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        let args = { Service = this.Remote<WalletService>() }
        Program.mkProgram (fun _ -> init, Cmd.none) (update args) view