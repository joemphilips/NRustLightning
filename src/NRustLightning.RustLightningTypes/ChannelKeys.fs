namespace RustLightningTypes

open System.IO
open DotNetLightning.Serialization
open NBitcoin


type ChannelKeys = {
    /// Private key of funding tx
    FundingKey: Key
    /// Local secret key for blinded revocation pubkey
    RevocationBaseKey: Key
    /// Local secret key used for our balance in remote-broadcasted commitment txs.
    PaymentKey: Key
    /// Local secret key used in HTLC tx
    DelayedPaymentBaseKey: Key
    /// Local htlc secret key used in commitment tx htlc outputs
    HTLCBaseKey: Key
    CommitmentSeed: uint256
    ChannelValueSatoshis: uint64
    KeyDerivationParams1: uint64
    KeyDerivationParams2: uint64
}
    with
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write (this.FundingKey.ToBytes())
        ls.Write (this.RevocationBaseKey.ToBytes())
        ls.Write (this.PaymentKey.ToBytes())
        ls.Write (this.DelayedPaymentBaseKey.ToBytes())
        ls.Write (this.HTLCBaseKey.ToBytes())
        
        ls.Write (this.CommitmentSeed.ToBytes(false))
        ls.Write (this.ChannelValueSatoshis, false)
        ls.Write (this.KeyDerivationParams1, false)
        ls.Write (this.KeyDerivationParams2, false)
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize ls
        ms.ToArray()
