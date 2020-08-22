namespace NRustLightning.RustLightningTypes

open System
open System.Diagnostics
open System.IO
open System.Security.Cryptography.X509Certificates
open ResultUtils
open DotNetLightning.Utils.Primitives
open DotNetLightning.Core.Utils.Extensions
open DotNetLightning.Crypto
open DotNetLightning.Serialize
open DotNetLightning.Utils
open NBitcoin

type FundingGenerationReadyData = {
    /// The random channel_id we picked which you'll need to pass into
    /// ChannelManager.FundingTransactionGenerated.
    TemporaryChannelId: ChannelId
    /// The value, in satoshis, that the output should have.
    ChannelValueSatoshis: uint64
    /// The script which should be used in the transaction output.
    OutputScript: Script
    /// The value passed in to ChannelManager::CreateChannel
    UserChannelId: uint64
}

type FundingBroadcastSafeData = {
    /// The output, which was passed to ChannelManager.FundingTransactionGenerated, which is now safe to broadcast.
    OutPoint: LNOutPoint
    /// The value passed in to ChannelManager.CreateChannel .
    UserChannelId: uint64
}

type PaymentReceivedData = {
    PaymentHash: PaymentHash
    PaymentSecret: uint256 option
    Amount: LNMoney
}
type PaymentFailedData = {
    PaymentHash: PaymentHash
    RejectedByDest: bool
}

type Duration = {
    MilliSec: uint64
}
with
     member this.TimeToWait(random: Random) =
         let factor = float (random.Next(1, 5))
         TimeSpan.FromMilliseconds(float this.MilliSec) * factor

type StaticOutputData = {
    Outpoint: LNOutPoint
    Output: TxOut
}
type DynamicOutputP2WSHData = {
    Outpoint: LNOutPoint
    PerCommitmentPoint: CommitmentPubKey
    ToSelfDelay: uint16
    Output: TxOut
    KeyDerivationParams: uint64 * uint64
    RemoteRevocationPubKey: PubKey
}

type StaticOutputRemotePaymentData = {
    Outpoint: LNOutPoint
    Output: TxOut
    KeyDerivationParams: uint64 * uint64
}

/// When on-chain outputs are created by rust-lightning (which our counterparty is not able to claim at any point in
/// the future) and event is generated which you must track and be able to spend on-chain.
/// The information needed to do this is provided in this enum, including the outpoint describing which txid and
/// output index is available, the full output which exists at that txid/index, and any keys or other information
/// required to sign.
type SpendableOutputDescriptor =
    /// An output to a script which was provided via KeysInterface, thus you should already know
    /// how to spend it. No keys are provided as rust-lightning was never given any keys - only the script_pubkey
    /// as it appears in the output.
    /// These may include outputs from a transaction pushing our counterparty or claiming an HTLC on-chain using
    /// the payment preimage or after it has timed out.
    | StaticOutput of StaticOutputData
    /// An output to a P2WSH script which can be spent with a single signature after a CSV delay.
    /// The private key which should be used to sign the transaction is provided, as well as the
    /// full witness in the spending input should be:
    /// <BIP 143 signature generated with the given key> <empty vector> (MINIMALIF standard rule)
    /// <witness_script as provided>
    /// Note that the nSequence field in the input must be set to_self_delay (which corresponds to
    /// the transaction not being broadcastable until at least tl_self_delay blocks after the input
    /// confirms).
    /// These are generally the result of "revocable" output to us, spendable only by us unless
    /// it is an output from us having broadcast an old state (which should never happen).
    | DynamicOutputP2WSH of DynamicOutputP2WSHData
    /// An output to a P2WPKH, spendable exclusively by our payment key (ie the private key which
    /// corresponds to the public key in ChannelKeys::pubkeys().payment_point).
    /// The witness in the spending input, is, simply:
    /// <BIP 143 signature> <payment key>
    ///
    /// These are generally the result of our counterparty having broadcast the current state,
    /// allowing us to claim the non-HTLC-encumbered outputs immediately.
    | StaticOutputRemotePayment of StaticOutputRemotePaymentData
    with
    member this.Output =
        match this with
        | StaticOutput({ Output = o }) -> o
        | DynamicOutputP2WSH({Output = o}) -> o
        | StaticOutputRemotePayment({Output = o}) -> o
    member this.OutPoint =
        match this with
        | StaticOutput({ Outpoint = o }) -> o
        | DynamicOutputP2WSH({ Outpoint = o }) -> o
        | StaticOutputRemotePayment({ Outpoint = o }) -> o
    static member Deserialize(ls: LightningReaderStream): SpendableOutputDescriptor =
        match ls.ReadByte() with
        | 0uy ->
            StaticOutput({
                Outpoint = ls.ReadOutpoint()
                Output = ls.ReadTxOut()
            })
        | 1uy ->
            DynamicOutputP2WSH({
                DynamicOutputP2WSHData.Outpoint = ls.ReadOutpoint()
                PerCommitmentPoint = ls.ReadCommitmentPubKey()
                ToSelfDelay = ls.ReadUInt16 false
                Output = ls.ReadTxOut()
                KeyDerivationParams = (ls.ReadUInt64(false), ls.ReadUInt64(false))
                RemoteRevocationPubKey = ls.ReadPubKey()
            })
        | 2uy ->
            StaticOutputRemotePayment({
                Outpoint = ls.ReadOutpoint()
                Output = ls.ReadTxOut()
                KeyDerivationParams = (ls.ReadUInt64(false), ls.ReadUInt64(false))
            })
        | x ->
            failwithf "Unknown SpendableOutputDescriptor type %A ! this should never happen"  x
    static member ParseUnsafe(b: byte[]) =
        use ms = new MemoryStream(b)
        use ls = new LightningReaderStream(ms)
        SpendableOutputDescriptor.Deserialize ls
        
    static member Parse(bytes: byte[]): Result<SpendableOutputDescriptor, _> =
        try
            SpendableOutputDescriptor.ParseUnsafe bytes |> Ok
        with
        | ex -> Error ex
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use s = new LightningWriterStream(ms)
        this.Serialize s
        ms.ToArray()
        
            
    member this.Serialize(ls: LightningWriterStream) =
        match this with
        | StaticOutput d ->
            ls.Write(0uy)
            ls.Write(d.Outpoint.Value.ToBytes())
            ls.Write(d.Output.ToBytes())
        | DynamicOutputP2WSH d ->
            ls.Write(1uy)
            ls.Write(d.Outpoint.Value.ToBytes())
            ls.Write(d.PerCommitmentPoint.ToByteArray())
            ls.Write(d.ToSelfDelay, false)
            ls.Write(d.Output.ToBytes())
            let (a, b) = d.KeyDerivationParams
            ls.Write(a.GetBytesBigEndian())
            ls.Write(b.GetBytesBigEndian())
            ls.Write(d.RemoteRevocationPubKey)
        | StaticOutputRemotePayment d ->
            ls.Write(2uy)
            ls.Write(d.Outpoint.Value.ToBytes())
            ls.Write(d.Output.ToBytes())
            let (a, b) = d.KeyDerivationParams
            ls.Write(a.GetBytesBigEndian())
            ls.Write(b.GetBytesBigEndian())

/// An Event which you should probably take some action in response to.
type Event =
    /// Used to indicate that the client should generate a funding transaction with the given
    /// parameters and then call ChannelManager::FundingTransactionGenerated .
    /// Generated in ChannelManager message handling.
    /// Note that *all inputs* in the funding transaction must spend SegWit outputs or your counterparty
    /// can steal your funds!
    | FundingGenerationReady of FundingGenerationReadyData
    /// Used to indicate that the client may now broadcast the funding tx it created for a channel.
    /// Broadcasting such a tx prior to this event may lead to our counterparty
    /// trivially stealing all funds in the funding tx
    | FundingBroadcastSafe of FundingBroadcastSafeData
    /// Indicates we've received money! Just gotta dig out that payment preimage and feed it to
    /// ChannelManager.ClaimFunds to get it...
    /// Note that if the preimage is not known or hte amount paid is incorrect, you should cal
    /// ChannelManager.FailHTLCBackwards to free up resources for this HTLC and avoid network congestion.
    /// The amount paid should be considered 'incorrect' when it is less than or more than twice the amount expected.
    /// If you fail to call either ChannelManager.ClaimFunds or ChannelManager.FailHTLCBackwards within the HTLC's
    /// timeout, the HTLC will be automatically failed.
    | PaymentReceived of PaymentReceivedData
    
    /// Indicates an outbound payment we made succeeded (ie it made it all the way to its target
    /// and we got back the payment preimage for it).
    /// Note that duplicative PaymentSent Events may be generated - it is your responsibility to
    /// deduplicate them by payment_preimage (which MUST be unique)!
    | PaymentSent of PaymentPreimage
    /// Indicates an outbound payment we made succeeded (ie it made it all the way to its target
    /// and we got back the payment preimage for it).
    /// Note that duplicative PaymentSent Events may be generated - it is your responsibility to
    /// deduplicate them by payment_preimage (which MUST be unique)!
    | PaymentFailed of PaymentFailedData
    /// Used to indicate that ChannelManager::process_pending_htlc_forwards should be called at a
    /// time in the future.
    | PendingHTLCsForwardable of Duration
    /// Used to indicate that ChannelManager::process_pending_htlc_forwards should be called at a
    /// time in the future.
    | SpendableOutputs of SpendableOutputDescriptor[]
    with
    static member Deserialize(ls: LightningReaderStream) =
        let t = ls.ReadByte()
        match t with
        | 0uy ->
            FundingGenerationReady({
                FundingGenerationReadyData.TemporaryChannelId = ChannelId(ls.ReadUInt256(false))
                ChannelValueSatoshis = ls.ReadUInt64(false)
                OutputScript = ls.ReadWithLen16() |> Script.FromBytesUnsafe
                UserChannelId = ls.ReadUInt64 false
            })
        | 1uy ->
            FundingBroadcastSafe({
                FundingBroadcastSafeData.OutPoint =
                     // confusingly, the outpoint used here is not consensus-encoded, instead it is rl-specific way.
                    (ls.ReadUInt256(true), ls.ReadUInt16(false) |> uint32)
                    |> OutPoint
                    |> LNOutPoint
                UserChannelId = ls.ReadUInt64 false
            })
        | 2uy ->
            PaymentReceived({
                PaymentReceivedData.PaymentHash = ls.ReadUInt256 false |> PaymentHash
                PaymentSecret = ls.ReadOption() |> Option.map (fun x ->  Debug.Assert(x.Length = 32, (sprintf "x.Length %d" x.Length)); uint256(x, false))
                Amount = ls.ReadUInt64 false |> LNMoney.MilliSatoshis
            })
        | 3uy ->
            PaymentSent(ls.ReadBytes 32 |> PaymentPreimage.Create)
        | 4uy ->
            PaymentFailed({
                PaymentFailedData.PaymentHash = ls.ReadUInt256 false |> PaymentHash
                RejectedByDest = ls.ReadByte() = 1uy
            })
        | 5uy ->
            PendingHTLCsForwardable({
                Duration.MilliSec = ls.ReadUInt64 false
            })
        | 6uy ->
            let length = int (ls.ReadUInt64 false)
            let result = Array.zeroCreate length
            for i in 0..(length - 1) do
                result.[i] <- SpendableOutputDescriptor.Deserialize ls
            SpendableOutputs(result)
        | x -> failwithf "Unknown event type %A!: this should never happen" x
    
    static member Parse(s:byte[]): Result<Event, _> =
        try
            Event.Parse s
        with
            | ex -> Error ex
        
    static member ParseUnsafe(s: byte[]): Event =
        use ms = new MemoryStream(s)
        use ls = new LightningReaderStream(ms)
        Event.Deserialize ls
        
    static member ParseManyUnsafe(s: byte[]): Event[] =
        if s.Length = 0 then [||] else
        let len = int(UInt16.FromBytesBigEndian(s.[0..1]))
        if len = 0 then [||] else
        let res = Array.zeroCreate(len)
        use ms = new MemoryStream(s.[2..])
        use ls = new LightningReaderStream(ms)
        for i in 0..(len - 1) do
            res.[i] <- Event.Deserialize ls
        res
        
    static member ParseMany(s: byte[]): Result<Event[], _> =
        try
            Event.ParseManyUnsafe s |> Ok
        with
        | ex -> Error (ex)
            
