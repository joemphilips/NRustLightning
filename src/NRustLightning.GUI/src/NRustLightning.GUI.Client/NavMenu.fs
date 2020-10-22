[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.NavMenu

open Bolero.Html
open MatBlazor


let view _model _dispatch =
    comp<MatNavMenu> ["Multi" => true; "Class" => "app-sidebar"] [
        text "This is NavMenu"
    ]
    