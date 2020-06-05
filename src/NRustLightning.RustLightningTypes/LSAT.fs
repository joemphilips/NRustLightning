namespace DotNetLightning.Payment
open System
open System.Net
open System.Net.Http.Headers
open DotNetLightning.Utils.Primitives
open NBitcoin
open DotNetLightning.Core.Utils.Extensions
open DotNetLightning.Utils
open Macaroons
open ResultUtils

module private LSATHelpers =
    /// A utility for returning the expiration date of the LSAT's macaroon based on a optional caveat
    /// returns None when no expiration caveats found.
    let getExpirationFromMacaroon (m: Macaroon) =
        let mutable expirationCaveats = ResizeArray()
        m.Caveats
        |> Seq.iter( fun c ->
            let x = c.CId.ToString()
            if isNull x && x.StartsWith("expiration=") then
                let n = x.Substring("expiration=".Length) |> DateTimeOffset.Parse
                expirationCaveats.Add(n)
            )
        // return last expiration caveat
        expirationCaveats |> Seq.tryLast

    let hex = DataEncoders.HexEncoder()
    
    
type MacaroonIdentifierV0 = {
    PaymentHash: PaymentHash
    TokenId: uint256
}
    with
    static member Create(p: PaymentHash) =
        {
            PaymentHash = p
            TokenId = RandomUtils.GetUInt256()
        }

type MacaroonIdentifier =
    | V0 of MacaroonIdentifierV0
    | UnknownVersion of version: uint16 * contents: byte[]
    with
    static member TryCreateFromBytes(b: byte[]) =
        if (b.Length <> 2 + 32 + 32) then Error(sprintf "Invalid bytes for macaroon identifier %A" b) else
        match UInt16.FromBytesBigEndian(b.[0..1]) with
        | 0us ->
            {
                PaymentHash = PaymentHash.PaymentHash(uint256(b.[2..33], false))
                TokenId = uint256(b.[34..], false)
            }
            |> V0
            |> Ok
        | x ->
            UnknownVersion(x, b)
            |> Ok
            
    member this.ToBytes() =
        match this with
        | V0 i ->
            let r = Array.zeroCreate 66
            Array.blit (0us.GetBytesBigEndian()) 0 r 0 2
            Array.blit (i.PaymentHash.ToBytes()) 0 r 2 32
            Array.blit (i.TokenId.ToBytes(false)) 0 r 34 32
            r
        | UnknownVersion (v, bytes) ->
            Array.concat (seq { yield v.GetBytesBigEndian(); yield bytes } )
            
    static member TryParse(str: string) =
        str |> LSATHelpers.hex.DecodeData |> MacaroonIdentifier.TryCreateFromBytes
    member this.ToHex() =
        this.ToBytes() |> LSATHelpers.hex.EncodeData
        
    override this.ToString() = this.ToHex()
type LSATConstructorOption =
    {
        Id: MacaroonIdentifier
        BaseMacaroon: Macaroon
        PaymentHash: PaymentHash
        PaymentPreimage: PaymentPreimage option
        ValidUntil: DateTimeOffset option
        TimeCreated: DateTimeOffset option
        Invoice: PaymentRequest option
        AmountPaid: LNMoney option
        RoutingFeePaid: LNMoney option
    }
    
type LSAT = private {
    Id: MacaroonIdentifier
    BaseMacaroon: Macaroon
    PaymentHash: PaymentHash
    PaymentPreimage: PaymentPreimage option
    /// If None, it will never expires.
    ValidUntil: DateTimeOffset option
    TimeCreated: DateTimeOffset
    Invoice: PaymentRequest option
    AmountPaid: LNMoney option
    RoutingFeePaid: LNMoney option
}
    with
    member this.InvoiceAmount =
        this.Invoice |> Option.bind(fun i -> i.AmountValue)
    static member TryCreate(options: LSATConstructorOption) =
        result {
            let expiration = LSATHelpers.getExpirationFromMacaroon options.BaseMacaroon
            let maybeHash = options.Invoice |> Option.map(fun i -> i.PaymentHash)
            do!
                match maybeHash with
                | Some h -> if options.PaymentHash <> h then Error (sprintf "PaymentHash in option (%A) does not match with the one in invoice (%A)" options.PaymentHash h) else Ok()
                | _ -> Ok ()
            return {
                Id = options.Id
                BaseMacaroon = options.BaseMacaroon
                PaymentHash = options.PaymentHash
                PaymentPreimage = options.PaymentPreimage
                ValidUntil = expiration
                TimeCreated = DateTimeOffset.UtcNow
                Invoice = options.Invoice
                AmountPaid = options.AmountPaid
                RoutingFeePaid = options.RoutingFeePaid
            }
        }
        
    static member FromMacaroon(m: Macaroon, ?invoice: PaymentRequest) =
        result {
            let! idv0 =
                MacaroonIdentifier.TryParse(m.Identifier.ToString())
                |> Result.bind(function | V0 i -> Ok (i) | UnknownVersion (v, _) -> Error(sprintf "Unknown macaroon identifier version %d" v))
            let options = { LSATConstructorOption.Id = V0 idv0
                            BaseMacaroon = m
                            PaymentHash = idv0.PaymentHash
                            PaymentPreimage = None
                            ValidUntil = None
                            TimeCreated = None
                            Invoice = None
                            AmountPaid = None
                            RoutingFeePaid = None }
            let! lsat = LSAT.TryCreate options
            match invoice with
            | Some i ->
                return! lsat.AddInvoice i
            | None ->
                return lsat;
        }
        
    static member FromMacaroon(macaroon: string) =
        let m = Macaroon.Deserialize macaroon
        LSAT.FromMacaroon m
        
    static member FromToken(token: string, ?invoice: PaymentRequest) =
        let token = token.TrimStart()
        if not <| token.StartsWith("LSAT ") then Error("token must start with 'LSAT'") else
        let s = token.Substring("LSAT ".Length).Trim().Split(':')
        if (s.Length <> 2) then Error("token must contain exactly one ':'") else
        let (macaroon, preimage) = s.[0], s.[1]
        result {
            let m = Macaroon.Deserialize macaroon
            let! lsat = match invoice with | Some i -> LSAT.FromMacaroon(m, i) | None -> LSAT.FromMacaroon m
            let p = preimage |> LSATHelpers.hex.DecodeData |> PaymentPreimage.Create
            return! lsat.SetPreimage p
        }
        
    member this.IsExpired =
        match this.ValidUntil with
        | None -> false
        | Some x -> x < DateTimeOffset.UtcNow
        
    member this.IsPending =
        this.PaymentPreimage.IsNone
        
    member this.IsSatisfied =
        match this.PaymentPreimage with
        | None -> false
        | Some x ->
            if x.Hash <> this.PaymentHash then false else
            true
            
    member this.Macaroon = this.BaseMacaroon
    
    member this.SetPreimage(p: PaymentPreimage) =
        if p.Hash <> this.PaymentHash then Error(sprintf "PaymentPreimage mismatch! expected hash: (%A). given hash (%A)" this.PaymentHash p.Hash) else
        { this with PaymentPreimage = Some(p) }
        |> Ok

    member this.AddFirstPartyCaveat(caveat: string) =
        { this with BaseMacaroon = this.BaseMacaroon.AddFirstPartyCaveat caveat }
    member this.Caveats =
        this.BaseMacaroon.Caveats
        
    /// The value which you should put on HTTP Authorization header.
    /// returns None when PaymentPreimage is not set (which means there is no way to create valid token).
    member this.TokenString =
        this.PaymentPreimage
        |> Option.map(fun p ->
            sprintf "LSAT %s:%s" (this.BaseMacaroon.Serialize()) (p.ToByteArray().ToHex())
        )
        
    /// The value which you should put on HTTP WWW-Authenticate response header.
    /// Returns base64 encoded string with macaroon and invoice information prefixed with authentication type (in this case "LSAT")
    /// Returns None when invoice is not set (which means there is not way to create valid challenge)
    member this.ChallengeString =
        this.Invoice
        |> Option.map(fun i ->
            sprintf "LSAT macaroon=%s, invoice=%s" (this.BaseMacaroon.Serialize()) (i.ToString())
        )
        
    member this.AddInvoice(invoice: PaymentRequest): Result<LSAT, _> =
        if this.PaymentHash <> invoice.PaymentHash then Error(sprintf "payment hash mismatch! expected(%A). given (%A)" this.PaymentHash invoice.PaymentHash) else
        { this with Invoice = Some invoice } |> Ok
        
        
