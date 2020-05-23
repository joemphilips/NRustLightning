namespace NRustLightning.NRustLightningTypes

open System
open System.Diagnostics
open System.IO
open DotNetLightning.Core.Utils.Extensions
open DotNetLightning.Serialize
open NBitcoin
open ResultUtils

type ChannelDetails = {
    ChannelId: uint256
    ShortChannelId: Option<uint64>
    RemoteNetworkId: PubKey
    CounterPartyFeatures: FeatureBit
    ChannelValueSatoshis: uint64
    OutboundCapacityMSat: uint64
    InboundCapacityMSat: uint64
    IsLive: bool
}
    with
    static member Deserialize(ls: LightningReaderStream) =
        let channelId = ls.ReadUInt256(false)
        let shortChannelId =
            let b = ls.ReadByte()
            match b with
            | 0uy -> None
            | _ ->
                let _length = ls.ReadBigSize()
                Debug.Assert(_length.Equals 9) |> ignore
                let d = ls.ReadUInt64(false)
                Some(d)
        {
            ChannelId = channelId
            ShortChannelId = shortChannelId
            RemoteNetworkId = ls.ReadPubKey()
            CounterPartyFeatures = ls.ReadWithLen() |> FeatureBit.CreateUnsafe
            ChannelValueSatoshis = ls.ReadUInt64(false)
            OutboundCapacityMSat = ls.ReadUInt64(false)
            InboundCapacityMSat = ls.ReadUInt64(false)
            IsLive = ls.ReadByte() = 1uy
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(this.ChannelId, false)
        ls.Write(this.ShortChannelId |> Option.map(fun x -> x.GetBytesBigEndian()))
        ls.Write(this.RemoteNetworkId)
        failwith ""
        
    static member ParseUnsafe(b: byte[]): ChannelDetails =
        use mem = new MemoryStream(b)
        use ls = new LightningReaderStream(mem)
        ChannelDetails.Deserialize ls
        
    static member ParseManyUnsafe(b: byte[]): ChannelDetails[] =
        failwith "todo"
