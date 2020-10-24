[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.NavMenu

open Bolero
open Elmish
open Bolero.Html
open MatBlazor
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open NRustLightning.GUI.Client.Utils

type private Args = {
    NavigationManager: NavigationManager
}
type Model = {
    Order: int
    WalletNames: Deferred<string []>
}

type Msg =
    | NoOp
    | LoadWalletNames of AsyncOperationStatus<string[]>

let update msg model =
    match msg with
    | NoOp -> model
    | LoadWalletNames(Started) ->
        { model with WalletNames = InProgress }
    | LoadWalletNames(Finished names) ->
        { model with WalletNames = Resolved names }
    
let init = {
    Order = 1
    WalletNames = HasNotStartedYet
}

let private view { NavigationManager = navigationManager } model _dispatch =
    comp<MatNavMenu> ["Multi" => true; "Class" => "app-sidebar"] [
        text "This is NavMenu"
        comp<MatNavItem> ["Href" => (navigationManager.ToAbsoluteUri" ").AbsoluteUri
                          "NavLinkMatch" => NavLinkMatch.All
                          ] [
            comp<MatIcon>[] [text MatIconNames.Home]
        ]
        
        comp<MatNavSubMenu> [] [
            comp<MatNavSubMenuHeader> [] [
                comp<MatNavItem> ["AllowSelection" => false] [
                    comp<MatIcon> [] [text MatIconNames.Account_balance_wallet]
                    span [] [text "List Of Wallets"]
                ]
            ]
            comp<MatNavSubMenuList> [] [
                cond model.WalletNames <|
                    function
                    | HasNotStartedYet -> empty
                    | InProgress -> Components.spinner
                    | Resolved names ->
                        comp<MatNavItem> ["Href" => navigationManager.ToAbsoluteUri("/wallet")] [
                            forEach names <| fun n -> textf "Wallet of name: %s" n
                        ]
                        
                comp<MatNavItem> [] [
                    comp<MatFAB> ["Icon" => MatIconNames.Add; "Label" => "Create New Wallet"] []
                ]
            ]
        ]
    ]
    
    
type App() =
    inherit ProgramComponent<Model,Msg>()
    
    [<Inject>]
    member val NavigationManager = Unchecked.defaultof<NavigationManager> with get,set
    override this.Program =
        let args = { NavigationManager = this.NavigationManager }
        Program.mkSimple (fun _ -> init) update (view args)