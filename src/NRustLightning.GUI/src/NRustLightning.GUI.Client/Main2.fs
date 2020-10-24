module NRustLightning.GUI.Client.Main2

open System
open System.Linq
open Bolero
open Bolero.Html
open Elmish
open MatBlazor
open MatBlazor
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Web
open NRustLightning.GUI.Client.AppState
open Bolero.Templating.Client

type Page =
    | Startup
    | Wallet
    | CoinView
    | Configuration


type Model = {
    bbDrawerClass: string
    NavMenuOpened: bool
    NavMinified: bool
}
    with
    static member Default = {
        bbDrawerClass = String.Empty
        NavMenuOpened = false
        NavMinified = true
    }

type Msg =
    | NavToggle
    | NavMinify

type Args = internal {
    AppState: AppState
}

let update msg model =
    match msg with
    | NavToggle ->
        let opened =  not model.NavMenuOpened
        let bbDrawerClass = if opened then "full" else "closed"
        { model with NavMenuOpened = opened; bbDrawerClass = bbDrawerClass }
    | NavMinify ->
        let minified = if (model.NavMenuOpened |> not) then true  else not model.NavMinified
        let bbDrawerClass =
            if (minified) then "mini" else
            if (model.NavMenuOpened) then "full" else
            model.bbDrawerClass
        { model with bbDrawerClass = bbDrawerClass; NavMinified = minified }
        
[<AutoOpen>]
module private ButtonWithTooltip =
            
    let private button icon onClick forwardRef =
        comp<MatIconButton> ["Class" => "navToggle"
                             "Icon" => icon
                             "ToggleIcon" => icon
                             "RefBack" => forwardRef
                             attr.callback "OnClick" (onClick)
                             ] []
    let buttonWithTooltip icon tooltip onClick =
        let buttonComp = button icon onClick
        let tooltip = comp<MatTooltip> ["Tooltip" => tooltip
                                        attr.fragmentWith "ChildContent" (fun (f: ForwardRef) -> buttonComp f)] []
        tooltip

let view ({ AppState = appState; })
         (model: Model)
         (dispatch) =
    comp<MatDrawerContainer> [
        "Style" => "width: 100vw; height: 100vh;"
        "Class" => model.bbDrawerClass
    ] [
        comp<MatDrawer> [
            "Opened" => model.NavMenuOpened
        ] [
            header [attr.classes["drawer-header"]] [
                div [attr.classes["drawer-logo"]] [
                    img [attr.alt appState.AppName; attr.classes["logo-img"]; attr.src "/images/bitcoin-svglogo.svg"; attr.title appState.AppName]
                    a [attr.classes ["miniHover"]; attr.href "/"] [text appState.AppName]
                ]
            ]
            comp<MatNavMenu> [ "Multi" => true; "Class" => "app-sidebar" ] [
                comp<NavMenu.App> [] []
            ]
            footer [attr.classes ["drawer-footer"]] []
        ]
        comp<MatDrawerContent> [] [
            comp<MatAppBarContainer> ["Style" => "display: flex; flex-direction: column; min-height: 100vh;"] [
                comp<MatAppBar> ["Fixed" => true] [
                    comp<MatAppBarRow> [] [
                        comp<MatAppBarSection> [] [
                            comp<MatAppBarTitle> [] [
                                div [attr.classes ["hidden-mdc-down"]] [
                                    buttonWithTooltip "menu" "AppHoverNavToggle" (fun (_e: MouseEventArgs) -> dispatch NavToggle)
                                    buttonWithTooltip "format_indent_decrease" "AppHoverNavMinimize" (fun (_e: MouseEventArgs) -> dispatch NavMinify )
                                ]
                            ]
                        ]
                        comp<MatAppBarSection> ["Align" => MatAppBarSectionAlign.End] [
                            img [attr.alt appState.AppName; attr.classes["logo-img"]; attr.src "/images/bitcoin-svglogo.svg"; attr.title appState.AppName]
                        ]
                    ]
                ]
                comp<MatAppBarContent> ["Style" => "flex: 1; display: flex; flex-direction: column;"] [
                    comp<Breadcrumbs.App> [] []
                    section [ attr.classes["container-fluid"]; attr.style "flex: 1" ] [
                        text "this is child fuga"
                    ]
                    footer [] []
                ]
            ]
        ]
    ]

type MyApp2() =
    inherit ProgramComponent<Model, Msg>()
    
    [<Inject>]
    member val AppState: AppState = Unchecked.defaultof<AppState> with get, set
    
    override this.Program =
        assert (this.AppState |> box |> isNull |> not)
        let args = { AppState = this.AppState; }
        Program.mkSimple(fun _ -> Model.Default) update (view args)
    #if DEBUG
            |> Program.withHotReload
    #endif
