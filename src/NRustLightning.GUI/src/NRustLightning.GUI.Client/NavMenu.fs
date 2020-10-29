[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.NavMenu

open Bolero
open Elmish
open Bolero.Html
open MatBlazor
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open NRustLightning.GUI.Client.Utils
open NRustLightning.GUI.Client.Wallet

type WalletComponentArgs = {
    WalletName: string
    HRef: string
}
type Model =
    | Home
    | Wallet of WalletNames: Map<WalletId, WalletComponentArgs> * importWalletHRef: string * generateWalletHRef: string
    | Config

type Msg =
    | NoOp

type Args = {
    Href: string
}
let private view {Href = href} model dispatch =
    cond model <| function
        | Model.Home ->
            // Home
            comp<MatNavItem> ["NavLinkMatch" => NavLinkMatch.All
                              "Href" => href
                              ] [
                comp<MatIcon>[ on.click(fun _ -> dispatch(NoOp)) ] [text MatIconNames.Home]
            ]
        | Model.Wallet (infos, importHRef, generateHRef) ->
            // Wallet
            comp<MatNavSubMenu> [] [
                comp<MatNavSubMenuHeader> [] [
                    comp<MatNavItem> ["AllowSelection" => false] [
                        comp<MatIcon> [] [text MatIconNames.Account_balance_wallet]
                        span [attr.classes ["miniHover"]] [text "List Of Wallets"]
                    ]
                ]
                comp<MatNavSubMenuList> [] [
                    forEach infos <| fun kv ->
                        let name = kv.Value.WalletName
                        comp<MatNavItem> [ "Href" => kv.Value.HRef  ] [
                            textf "Wallet of name: %s" name
                        ]
                            
                    comp<MatNavItem> ["Href" => importHRef] [
                        comp<MatFAB> ["Icon" => MatIconNames.Import_export; "Label" => "Import Wallet"
                                      ] [
                        ]
                    ]
                    comp<MatNavItem> ["Href" => generateHRef] [
                        comp<MatFAB> ["Icon" => MatIconNames.Add; "Label" => "Create Wallet"
                                      ] [
                        ]
                    ]
                ]
            ]
        | Model.Config ->
            comp<MatNavItem> [ "Href" => href] [
                comp<MatIcon> [on.click(fun _ -> dispatch(NoOp))] [
                    text MatIconNames.Settings
                ]
                span [attr.classes ["miniHover"]] [text "Config"]
            ]
    
type EApp() =
    inherit ElmishComponent<Model, Msg>()
    
    [<Parameter>]
    member val Href = Unchecked.defaultof<string> with get, set
    
    override this.View model dispatch =
        let args = { Href = this.Href }
        view args model dispatch
