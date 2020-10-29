namespace NRustLightning.GUI.Client.Configuration

open System
open System.IO
open System.Net
open System.Text.Json.Serialization
open NBitcoin
open NBitcoin.RPC
open System.Net.Http
open NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes

exception ConfigException of string

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
    mutable _Network: string
}
    with
    member this.Network = Network.GetNetwork (this._Network)
    static member Default = {
        RPCHost = "http://localhost"
        RPCPort = 18334
        RPCPassword = null
        RPCUser = null
        RPCCookieFile = null
        DBPath = Defaults.dataDirectoryPath
        _Network = Network.RegTest.ToString()
    }
    member this.Url = Uri(this.RPCHost)
    member this.GetRPCClient(http: HttpClient) =
        let cred = NetworkCredential(this.RPCUser, this.RPCPassword)
        let rpc = RPCClient(cred, this.Url, this.Network)
        rpc.HttpClient <- http
        rpc
        
    member this.Validate() =
        if (this.RPCHost |> fun h -> Uri.IsWellFormedUriString(h, UriKind.RelativeOrAbsolute)) |> not then Some ("Invalid URL for RPCHost") else
        if (this.RPCPort = 0) then Some("Port must not be 0") else
        if ((this.RPCPassword |> String.IsNullOrWhiteSpace || this.RPCUser |> String.IsNullOrWhiteSpace) && this.RPCCookieFile |> String.IsNullOrWhiteSpace) then Some("You must specify either RPC user/pass or cookie file") else
        if (this.RPCCookieFile |> String.IsNullOrWhiteSpace |> not && (this.RPCCookieFile) |> File.Exists |> not) then Some ("The RPCCookieFile does not exist") else
        None
