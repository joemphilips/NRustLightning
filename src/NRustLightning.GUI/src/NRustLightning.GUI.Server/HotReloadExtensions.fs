// Patch for https://github.com/fsbolero/Templating.HotReload/issues/10
// TODO: remove when it is fixed
#if DEBUG
module NRustLightning.GUI.Server.HotReloadExtensions

open System
open System.Collections.Concurrent
open System.IO
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.SignalR
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Bolero.Templating
open Bolero.TemplatingInternals

type WatcherConfig =
    {
        /// The directory containing the template files to reload on change.
        /// All files with extension .html in this directory and its subdirectories are watched.
        Directory: string
        /// The delay to wait when a change happens before triggering a reload.
        /// The default is 100 milliseconds; try increasing it if you experience issues
        /// such as file locks when saving template files.
        Delay: TimeSpan
    }

/// [omit]
[<AutoOpen>]
module Impl =

    let rec private asyncRetry (times: int) (job: Async<'T>) : Async<option<'T>> = async {
        try
            let! x = job
            return Some x
        with _ ->
            if times <= 1 then
                return None
            else
                do! Async.Sleep 1000
                return! asyncRetry (times - 1) job
    }

    let private delayed (delay: TimeSpan) (callback: 'K -> unit) =
        let cache = ConcurrentDictionary<'K, Timers.Timer>()
        fun (key: 'K) ->
            cache.AddOrUpdate(key,
                (fun _ ->
                    let t = new Timers.Timer(delay.TotalMilliseconds, AutoReset = false)
                    t.Elapsed.Add(fun _ ->
                        callback key
                        cache.TryRemove(key) |> ignore)
                    t.Start()
                    t),
                (fun _ t ->
                    t.Stop()
                    t.Start()
                    t))
            |> ignore

    type HotReloadHub(watcher: Watcher) =
        inherit Hub()

        member this.RequestFile(filename: string) : Task<string> =
            async {
                let fullPath = watcher.FullPathOf(filename)
                do! this.Groups.AddToGroupAsync(this.Context.ConnectionId, fullPath) |> Async.AwaitTask
                let! fileContent = watcher.GetFileContent fullPath
                return Option.toObj fileContent
            }
            |> Async.StartAsTask

    and Watcher(config: WatcherConfig, env: IHostEnvironment, log: ILogger<Watcher>, hub: IHubContext<HotReloadHub>) =
        let dir =
            Path.Combine(env.ContentRootPath, config.Directory)
            |> Path.Canonicalize

        let fullPathOf filename =
            Path.Combine(dir, filename)
            |> Path.Canonicalize

        let getFileContent fullPath =
            asyncRetry 3 <| async {
                use f = File.OpenText(fullPath)
                return! f.ReadToEndAsync() |> Async.AwaitTask
            }

        let changed = Event<string * string>()

        let onchange (fullPath: string) =
            async {
                let filename = Path.GetRelativePath dir fullPath
                match! getFileContent fullPath with
                | None ->
                    log.LogWarning("Bolero HotReload: failed to reload {0}", fullPath)
                | Some content ->
                    changed.Trigger((filename, content))
                    return! hub.Clients.Group(fullPath)
                        .SendAsync("FileChanged", filename, content)
                        |> Async.AwaitTask
            }

        let delayedOnchange = delayed config.Delay (Async.StartImmediate << onchange)

        let callback (args: PollingFileSystemEventArgs) =
            // filter later
            args.Changes |> Seq.iter (fun e -> delayedOnchange (Path.Combine(e.Directory, e.Name)))

        member _.Changed = changed.Publish

        member _.FullPathOf(filename) =
            fullPathOf filename

        member _.GetFileContent(fullPath) =
            getFileContent fullPath

        member this.Start() =
            let fsw = // leaks
                new PollingFileSystemWatcher(dir, "*.html",options=EnumerationOptions(RecurseSubdirectories=true))

            fsw.ChangedDetailed.Add callback
            fsw.Start()

            TemplateCache.client <-
                { new Client.ClientBase() with
                    member __.SetOnChange(_) = ()
                    member __.RequestFile(filename) =
                        onchange (fullPathOf filename)
                }

    /// Client used when running in Blazor server-side mode.
    and Client(watcher: Watcher) =

        let handlers = ResizeArray()

        interface IClient with

            member _.RequestTemplate(filename, subtemplate) =
                TemplateCache.client.RequestTemplate(filename, subtemplate)

            member _.SetOnChange(callback) =
                watcher.Changed.Subscribe(fun (filename, content) ->
                    TemplateCache.client.FileChanged(filename, content)
                    callback())
                |> handlers.Add

            member _.FileChanged(filename, content) =
                TemplateCache.client.FileChanged(filename, content)

        interface IDisposable with

            member _.Dispose() =
                for handler in handlers do
                    handler.Dispose()
open System
[<Extension;AbstractClass;Sealed>]
type HotReloadExtensions() =
    [<Extension>]
    static member AddHotReload(this: IServiceCollection, configure: WatcherConfig -> WatcherConfig) : IServiceCollection =
        this.AddSignalR().AddJsonProtocol() |> ignore
        let config = configure { Directory = "."; Delay = TimeSpan.FromMilliseconds 100. }
        this.AddSingleton(config)
            .AddSingleton<Watcher>()
            .AddTransient<IClient, Client>()

    [<Extension>]
    static member AddHotReload(this: IServiceCollection, ?templateDir: string, ?delay: TimeSpan) : IServiceCollection =
        HotReloadExtensions.AddHotReload(this, fun config ->
            {
                Directory = defaultArg templateDir config.Directory
                Delay = defaultArg delay config.Delay
            })


    [<Extension>]
    static member UseHotReload(this: IEndpointRouteBuilder, ?urlPath: string) : unit =
        this.ServiceProvider.GetService<Watcher>().Start()
        let urlPath = defaultArg urlPath HotReloadSettings.Default.Url
        this.MapHub<HotReloadHub>(urlPath) |> ignore
#endif

