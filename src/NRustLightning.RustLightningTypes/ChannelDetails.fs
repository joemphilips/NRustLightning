namespace NRustLightning.RustLightningTypes

open System.IO

open DotNetLightning.Serialize
open DotNetLightning.Utils

open NBitcoin
open ResultUtils

type ChannelDetails = {
    ChannelId: uint256
    ShortChannelId: Option<ShortChannelId>
    RemoteNetworkId: PubKey
    CounterPartyFeatures: FeatureBit
    ChannelValueSatoshis: uint64
    UserId: uint64
    OutboundCapacityMSat: uint64
    InboundCapacityMSat: uint64
    IsLive: bool
}
    with
    static member Deserialize(ls: LightningReaderStream) =
        let channelId = ls.ReadUInt256(false)
        let shortChannelId = ls.ReadOption() |> Option.map(ShortChannelId.From8Bytes)
        {
            ChannelId = channelId
            ShortChannelId = shortChannelId
            RemoteNetworkId = ls.ReadPubKey()
            CounterPartyFeatures = ls.ReadWithLen16() |> FeatureBit.CreateUnsafe
            ChannelValueSatoshis = ls.ReadUInt64(false)
            UserId = ls.ReadUInt64(false)
            OutboundCapacityMSat = ls.ReadUInt64(false)
            InboundCapacityMSat = ls.ReadUInt64(false)
            IsLive = ls.ReadByte() = 1uy
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(this.ChannelId, false)
        ls.Write(this.ShortChannelId |> Option.map(fun x -> x.ToBytes()))
        ls.Write(this.RemoteNetworkId)
        ls.WriteWithLen16(this.CounterPartyFeatures.ToByteArray())
        ls.Write(this.ChannelValueSatoshis, false)
        ls.Write(this.UserId, false)
        ls.Write(this.OutboundCapacityMSat, false)
        ls.Write(this.InboundCapacityMSat, false)
        ls.Write(if this.IsLive then 1uy else 0uy)
        
    member this.ToBytes() =
        use m = new MemoryStream()
        use ls = new LightningWriterStream(m)
        this.Serialize ls
        m.ToArray()
        
    static member ParseUnsafe(b: byte[]): ChannelDetails =
        use mem = new MemoryStream(b)
        use ls = new LightningReaderStream(mem)
        ChannelDetails.Deserialize ls
        
    static member ParseManyUnsafe(b: byte[]): ChannelDetails[] =
        use mem = new MemoryStream(b)
        use ls = new LightningReaderStream(mem)
        let len = int (ls.ReadUInt16(false))
        if (len = 0) then [||] else
        let result = Array.zeroCreate (len)
        for i in 0..(len - 1) do
            result.[i] <- ChannelDetails.Deserialize ls
        result
