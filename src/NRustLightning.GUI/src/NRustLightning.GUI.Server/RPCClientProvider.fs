namespace NRustLightning.GUI.Server

open System.Collections.Concurrent
open System.Net.Http
open Microsoft.Extensions.Options
open NBitcoin
open NBitcoin.RPC
open NRustLightning.GUI.Client.Configuration
open NRustLightning.Infrastructure.Configuration.SubConfiguration
open NRustLightning.Infrastructure.Interfaces
open NRustLightning.Infrastructure.Networks

type RPCClientProvider(opts: IOptionsMonitor<WalletBiwaConfiguration>, httpC: IHttpClientFactory,
                       networkProvider: NRustLightningNetworkProvider) as this =
    let clients = ConcurrentDictionary<string, RPCClient>()
    do
        for n in networkProvider.GetAll() do
            clients.TryAdd(n.CryptoCode, opts.CurrentValue.GetRPCClient(n.NBitcoinNetwork, httpC.CreateClient())) |> ignore
        opts.OnChange(this.OnOptionChange) |> ignore
    
    member private this.OnOptionChange(newOpts) =
        for n in networkProvider.GetAll() do
            let rpc = newOpts.GetRPCClient(n.NBitcoinNetwork, httpC.CreateClient())
            clients.AddOrReplace(n.CryptoCode, rpc)
        
    member this.GetRPCClient(cryptoCode: string) =
        match clients.TryGetValue(cryptoCode) with
        | true, v -> Some (v)
        | false, _ -> None
        
    member this.GetRPCClient(n: NRustLightningNetwork) =
        this.GetRPCClient(n.CryptoCode)

