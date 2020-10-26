[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.Breadcrumbs

open Elmish
open System
open System.Linq
open Bolero
open Bolero.Html
open Microsoft.AspNetCore.Blazor.Components
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open Microsoft.Extensions.Localization
open NRustLightning.GUI.Client.AppState
open NRustLightning.GUI.Client.Localization

type Model = internal {
    Paths: string[]
    BaseUrl: string
    LastUri: string
}
    with
    static member Default = {
        Paths = [||]
        BaseUrl = ""
        LastUri = ""
    }
    
type Msg =
    | LocationChange of string[]
    
type Args = internal {
    RootLinkTitle: string
}

let update msg model =
    match msg with
    | LocationChange paths ->
        { model with Paths = paths }

let view ({RootLinkTitle = rootLinkTitle }) {Paths = paths; BaseUrl = baseUrl} _dispatch =
    ul [attr.classes ["breadcrumb"]] [
         li [attr.href "/"] [ text rootLinkTitle ]
         forEach (paths |> Array.mapi(fun i p -> (p, i))) <| fun (value, i) ->
            if (paths |> Seq.length > 1) then
                if (i = 0)then
                    empty
                else if (i = paths.Count() - 1) then
                    li [] [
                        let refStr = "BreadCrumb" + String.Join("", paths).ToString().ToUpper()
                        a [attr.href refStr] []
                    ]
                else
                    li [] [
                        let refStr = baseUrl + String.Join("/", paths, 0, i + 1)
                        let contentStr = "BreadCrumb" + String.Join("", paths, 0, i + 1).ToString().ToUpper()
                        a [ attr.href refStr ] [ text contentStr ]
                    ]
            else
                li [] [text ("BreadCrumb" + (value |> string).ToString().ToUpper())]
    ]

type App() =
    inherit ProgramComponent<Model, Msg>()

    [<Inject>]
    member val internal AppState = Unchecked.defaultof<AppState> with get, set
    
    override this.Program =
        let subsc initial =
            let sub dispatch =
                this.NavigationManager.LocationChanged.Subscribe(fun _ ->
                    let uri = this.NavigationManager.Uri.Split('?').[0].Replace(initial.BaseUrl, "/").Trim()
                    let paths = if uri |> String.IsNullOrEmpty then [||] else uri.Split('/')
                    dispatch (LocationChange (paths))
                    ) |> ignore
            Cmd.ofSub sub
        Program.mkSimple (fun _ -> Model.Default) update (view { RootLinkTitle = this.AppState.BreadCrumbHome })
        |> Program.withSubscription subsc