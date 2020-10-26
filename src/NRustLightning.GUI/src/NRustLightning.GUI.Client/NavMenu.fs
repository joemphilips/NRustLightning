[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.NavMenu

open Bolero
open Elmish
open Bolero.Html
open MatBlazor
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open NRustLightning.GUI.Client.Utils
open NRustLightning.GUI.Client.Wallet.Utils

type Model =
    | Home
    | Wallet of WalletNames: Map<WalletId, string>
    | Config
    | CoinView

type Msg =
    | NoOp

let private view model dispatch =
    cond model <| function
        | Model.Home ->
            // Home
            comp<MatNavItem> ["NavLinkMatch" => NavLinkMatch.All
                              ] [
                comp<MatIcon>[ on.click(fun _ -> dispatch(NoOp)) ] [text MatIconNames.Home]
            ]
        | Model.Wallet names ->
            // Wallet
            comp<MatNavSubMenu> [] [
                comp<MatNavSubMenuHeader> [] [
                    comp<MatNavItem> ["AllowSelection" => false] [
                        comp<MatIcon> [] [text MatIconNames.Account_balance_wallet]
                        span [attr.classes ["miniHover"]] [text "List Of Wallets"]
                    ]
                ]
                comp<MatNavSubMenuList> [] [
                    forEach names <| fun kv ->
                        let name = kv.Value
                        comp<MatNavItem> [] [
                            textf "Wallet of name: %s" name
                        ]
                            
                    comp<MatNavItem> [] [
                        comp<MatFAB> ["Icon" => MatIconNames.Add; "Label" => "Create New Wallet"
                                      ] [
                        ]
                    ]
                ]
            ]
        | Model.CoinView ->
            text "TODO: show coinview"
        | Model.Config ->
            text "TODO: show config"
    
type EApp() =
    inherit ElmishComponent<Model, Msg>()
    
    override this.View model dispatch =
        view model dispatch
