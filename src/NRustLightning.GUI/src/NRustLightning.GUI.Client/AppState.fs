module NRustLightning.GUI.Client.AppState
open Humanizer
open NRustLightning.GUI.Client.Themes

type AppState () =
    member val AppName: string = "AppName".ToString().Humanize(LetterCasing.Title) with get, set
    member val AppShortName: string = "AppShortName".ToString().Humanize(LetterCasing.Title) with get, set
    member val BreadCrumbHome: string = "BreadCrumbHome".ToString().ToUpper() with get, set

    member val currentTheme = theme1 with get, set