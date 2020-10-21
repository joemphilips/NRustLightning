namespace NRustLightning.GUI.Client.Configuration

open System
open System.Net
open NBitcoin
open NBitcoin.RPC
open System.Net.Http


[<CLIMutable>]
type WalletBiwaConfiguration = {
    RPCHost: string
    RPCPort: int
    RPCPassword: string
    RPCUser: string
    RPCCookieFile: string
}
    with
    static member Default = {
        RPCHost = "localhost"
        RPCPort = 18334
        RPCPassword = null
        RPCUser = null
        RPCCookieFile = null
    }
    member this.Url = Uri(this.RPCHost)
    member this.GetRPCClient(network: Network, http: HttpClient) =
        let cred = NetworkCredential(this.RPCUser, this.RPCPassword)
        let rpc = RPCClient(cred, this.Url, network)
        rpc.HttpClient <- http
        rpc
        