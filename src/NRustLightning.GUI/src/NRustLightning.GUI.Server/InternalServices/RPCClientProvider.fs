namespace NRustLightning.GUI.Server.InternalServices

open System.Net.Http
open Microsoft.Extensions.Options
open NRustLightning.GUI.Client.Configuration
open NRustLightning.Infrastructure.Configuration.SubConfiguration
open NRustLightning.Infrastructure.Interfaces

type RPCClientProvider(server: IOptionsMonitor<WalletBiwaConfiguration>, httpC: IHttpClientFactory) as this =
    do
        server.OnChange(this.OnOptionChange) |> ignore
        ()
    
    member private this.OnOptionChange(opt) =
        failwith "TODO"
    interface IRPCClientProvider with
        member this.GetRpcClient(n) =
            failwith ""

