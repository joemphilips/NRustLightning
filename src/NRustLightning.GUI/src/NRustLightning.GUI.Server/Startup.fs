namespace NRustLightning.GUI.Server

open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Bolero.Remoting.Server
open Bolero.Server.RazorHost
open Microsoft.Extensions.Logging
#if DEBUG
open NRustLightning.GUI.Server.HotReloadExtensions
#endif

[<AutoOpen>]
module private Helpers =
    let getStartupLoggerFactory() =
        LoggerFactory.Create(fun builder ->
                builder
                    .AddConsole()
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Debug) |> ignore
            )

type Startup(config: IConfiguration) =
    let logger = getStartupLoggerFactory().CreateLogger<Startup>();
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddMvc().AddRazorRuntimeCompilation() |> ignore
        services.AddServerSideBlazor() |> ignore
        services
            .AddHttpClient()
            .ConfigureNRustLightning(config)
            .AddNRustLightning()
            .AddAuthorization()
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .Services
            .AddRemoting<BookService>()
            .AddRemoting<ConfigurationService>()
            .AddRemoting<WalletService>()
            .AddBoleroHost()
#if DEBUG
            .AddHotReload(templateDir = __SOURCE_DIRECTORY__ + "/../NRustLightning.GUI.Client")
#endif
        |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        app
            .UseAuthentication()
            .UseRemoting()
            .UseStaticFiles()
            .UseRouting()
            .UseBlazorFrameworkFiles()
            .UseEndpoints(fun endpoints ->
#if DEBUG
                endpoints.UseHotReload()
#endif
                endpoints.MapBlazorHub() |> ignore
                endpoints.MapFallbackToPage("/_Host") |> ignore)
        |> ignore

module Program =
    
    let configureConfig (c: IConfigurationBuilder) =
        c
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional=false, reloadOnChange=true)
            .AddJsonFile("appsettings.Development.json", optional=false, reloadOnChange=true) |> ignore

    [<EntryPoint>]
    let main args =
        WebHost
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configureConfig)
            .UseStaticWebAssets()
            .UseStartup<Startup>()
            .Build()
            .Run()
        0
