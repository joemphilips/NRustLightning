namespace NRustLightning.GUI.Client

open MatBlazor
open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Bolero.Remoting.Client
open Microsoft.Extensions.DependencyInjection
open NRustLightning.GUI.Client.AppState

module Program =

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault(args)
        builder.Services
            .AddSingleton(AppState()) |> ignore
        builder.RootComponents.Add<Main.MyApp>("#main")
        builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
        builder.Services.AddMatToaster(fun config ->
            config.Position <- MatToastPosition.TopRight
            config.PreventDuplicates <- true
            config.NewestOnTop <- true
            config.ShowCloseButton <- true
            config.VisibleStateDuration <- 30000
            ()) |> ignore
        builder.Build().RunAsync() |> ignore
        0
