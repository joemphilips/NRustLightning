namespace RustLightningTypes

open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices
open DotNetLightning.Core.Utils.Extensions
open DotNetLightning.Payment
open DotNetLightning.Serialize
open DotNetLightning.Utils


type RouteHint = {
    ExtraHop: ExtraHop
    HTLCMinimum: LNMoney
}
    with
    static member Deserialize(ls: LightningReaderStream) =
        let extraHop = {
            ExtraHop.NodeId = ls.ReadPubKey() |> NodeId
            ShortChannelId = ls.ReadUInt64(false) |> ShortChannelId.FromUInt64
            FeeBase = ls.ReadUInt32(false) |> LNMoney.MilliSatoshis
            FeeProportionalMillionths = ls.ReadUInt32(false)
            CLTVExpiryDelta = ls.ReadUInt16(false) |> BlockHeightOffset16
        }
        {
            RouteHint.ExtraHop = extraHop
            HTLCMinimum = ls.ReadUInt64(false) |> LNMoney.MilliSatoshis
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(this.ExtraHop.NodeId.Value)
        ls.Write(this.ExtraHop.ShortChannelId)
        ls.Write(this.ExtraHop.FeeBase.MilliSatoshi |> uint32, false)
        ls.Write(this.ExtraHop.FeeProportionalMillionths, false)
        ls.Write(this.ExtraHop.CLTVExpiryDelta.Value, false)
        ls.Write((uint64)this.HTLCMinimum.MilliSatoshi, false)
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
    static member Size = 33 + 8 + 4*2 + 2 + 8
    
[<Extension;AbstractClass;Sealed>]
type RouteHintExtension =
    [<Extension>]
    static member ToBytes(routeHints: IList<RouteHint>) =
        let len = uint64 routeHints.Count
        let result = Array.zeroCreate(8 + RouteHint.Size * routeHints.Count)
        Array.blit (len.GetBytesBigEndian()) 0 result 0 8
        for i in 0..(routeHints.Count - 1) do
            let r = routeHints.[i]
            Array.blit (r.ToBytes()) 0 result (8 + (i * RouteHint.Size)) RouteHint.Size
        result
