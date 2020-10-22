namespace NRustLightning.GUI.Client

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
        builder.RootComponents.Add<Main2.MyApp2>("#main")
        builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
        builder.Build().RunAsync() |> ignore
        0
