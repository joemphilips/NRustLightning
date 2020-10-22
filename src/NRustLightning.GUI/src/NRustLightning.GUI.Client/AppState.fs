module NRustLightning.GUI.Client.AppState
open System
open Humanizer
open Microsoft.Extensions.Localization
open NRustLightning.GUI.Client.Localization

type AppState () =
    member val AppName: string = "AppName".ToString().Humanize(LetterCasing.Title) with get, set
    member val AppShortName: string = "AppShortName".ToString().Humanize(LetterCasing.Title) with get, set
    member val BreadCrumbHome: string = "BreadCrumbHome".ToString().ToUpper() with get, set
