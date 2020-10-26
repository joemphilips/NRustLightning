namespace NRustLightning.GUI.Client.Configuration

open System
open System.IO
open System.Net
open NBitcoin
open NBitcoin.RPC
open System.Net.Http

module private Defaults =
    let homePath = 
        (if Environment.OSVersion.Platform = PlatformID.Unix || Environment.OSVersion.Platform = PlatformID.MacOSX then
             Environment.GetEnvironmentVariable("HOME")
         else
             Environment.GetEnvironmentVariable("%HOMEDRIVE%%HOMEPATH%")) |> Option.ofObj |> Option.defaultWith(fun _ -> failwith "Failed to define Home directory Path");
    let homeDirectoryName = ".walletbiwa"
    let homeDirectoryPath = Path.Join(homePath.AsSpan(), homeDirectoryName.AsSpan())
    let dataDirectoryPath = Path.Join(homeDirectoryPath.AsSpan(), "data".AsSpan())

[<CLIMutable>]
type WalletBiwaConfiguration = {
    mutable RPCHost: string
    mutable RPCPort: int
    mutable RPCPassword: string
    mutable RPCUser: string
    mutable RPCCookieFile: string
    mutable DBPath: string
}
    with
    static member Default = {
        RPCHost = "http://localhost"
        RPCPort = 18334
        RPCPassword = null
        RPCUser = null
        RPCCookieFile = null
        DBPath = Defaults.dataDirectoryPath
    }
    member this.Url = Uri(this.RPCHost)
    member this.GetRPCClient(network: Network, http: HttpClient) =
        let cred = NetworkCredential(this.RPCUser, this.RPCPassword)
        let rpc = RPCClient(cred, this.Url, network)
        rpc.HttpClient <- http
        rpc
        
    member this.Validate() =
        if (this.RPCHost |> fun h -> Uri.IsWellFormedUriString(h, UriKind.RelativeOrAbsolute)) then Some ("Invalid URL for RPCHost") else
        if (this.RPCPort = 0) then Some("Port must not be 0") else
        None
