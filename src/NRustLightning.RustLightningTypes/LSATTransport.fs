namespace RustLightningTypes.LSATTransport

open System.Diagnostics
open System.Net.Http.Headers
open DotNetLightning.Payment
open Microsoft.Extensions.Logging

module LSATHeaders = 
    [<Literal>]
    let HeaderAuthorization = "Authorization"
    [<Literal>]
    let HeaderMacaroonMD = "Grpc-Metadata-Macaroon"
    
    [<Literal>]
    let HeaderMacaroon = "Macaroon"
    
module LSATExtensions =
    type LSAT with
        // --- methods which uses transport layer information ---
        static member FromHeader(headers: HttpHeaders) =
            match headers.TryGetValues(LSATHeaders.HeaderAuthorization) with
            | true, h ->
                Debug.WriteLine(sprintf "Trying to authorize with header value [%s]" )
                failwith ""
            | false, _ ->
                failwith ""
        
