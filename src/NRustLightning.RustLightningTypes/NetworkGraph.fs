namespace NRustLightning.RustLightningTypes

open System.IO

open NBitcoin
open ResultUtils

open DotNetLightning.Utils
open DotNetLightning.Serialization
open DotNetLightning.Serialization.Msgs
open DotNetLightning.Utils.Aether

type RoutingFees = private {
    BaseMSat: uint32
    ProportionalMillionths: uint32
}
    with
    static member FromChannelUpdate(msg: UnsignedChannelUpdateMsg) = {
        BaseMSat = (uint32)msg.FeeBaseMSat.MilliSatoshi
        ProportionalMillionths = msg.FeeProportionalMillionths
    }
    static member Deserialize(ls: LightningReaderStream) =
        {
            BaseMSat = ls.ReadUInt32(false)
            ProportionalMillionths = ls.ReadUInt32(false)
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(this.BaseMSat, false)
        ls.Write(this.ProportionalMillionths, false)
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        RoutingFees.Deserialize(ls)

type DirectionalChannelInfo = {
    LastUpdate: uint32
    Enabled: bool
    CltvExpiryDelta: BlockHeightOffset16
    HTLCMinimumMSat: LNMoney
    Fees: RoutingFees
    LastUpdateMessage: ChannelUpdateMsg option
}
    with
    static member internal Enabled_: Lens<_, _> =
        (fun this -> this.Enabled),
        (fun e this -> { this with Enabled = e })
    static member Deserialize(ls: LightningReaderStream) =
        {
            LastUpdate = ls.ReadUInt32(false)
            Enabled = ls.ReadByte() = 1uy
            CltvExpiryDelta = ls.ReadUInt16(false) |> BlockHeightOffset16
            HTLCMinimumMSat = ls.ReadUInt64(false) |> LNMoney.MilliSatoshis
            Fees = RoutingFees.Deserialize(ls)
            LastUpdateMessage =
                let converter = ILightningSerializable.fromBytes
                ls.ReadOption() |> Option.map(converter)
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(this.LastUpdate, false)
        ls.Write(if this.Enabled then 1uy else 0uy)
        ls.Write(this.CltvExpiryDelta.Value, false)
        ls.Write(this.HTLCMinimumMSat.MilliSatoshi, false)
        this.Fees.Serialize(ls)
        ls.WriteOption(this.LastUpdateMessage |> Option.map(fun x -> x.ToBytes()))
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        DirectionalChannelInfo.Deserialize(ls)

type ChannelInfo = {
    Features: FeatureBits
    NodeOne: NodeId
    OneToTwo: DirectionalChannelInfo option
    NodeTwo: NodeId
    TwoToOne: DirectionalChannelInfo option
    AnnouncementMsg: ChannelAnnouncementMsg option
}
    with
    static member internal OneToTwo_: Lens<_, _> =
        (fun self -> self.OneToTwo),
        (fun v self -> { self with OneToTwo = v })
    static member internal TwoToOne_: Lens<_, _> =
        (fun self -> self.TwoToOne),
        (fun v self -> { self with TwoToOne = v })
    static member FromMsg(msg: UnsignedChannelAnnouncementMsg) =
        {
            Features = msg.Features
            NodeOne = msg.NodeId1
            OneToTwo = None
            NodeTwo = msg.NodeId2
            TwoToOne = None
            AnnouncementMsg = None
        }
    static member Deserialize(ls: LightningReaderStream) =
        {
            Features = ls.ReadWithLen() |> FeatureBits.CreateUnsafe
            NodeOne = ls.ReadPubKey() |> NodeId
            OneToTwo = ls.ReadOption() |> Option.map(DirectionalChannelInfo.FromBytes)
            NodeTwo = ls.ReadPubKey() |> NodeId
            TwoToOne = ls.ReadOption() |> Option.map(DirectionalChannelInfo.FromBytes)
            AnnouncementMsg =
                let converter = ILightningSerializable.fromBytes
                ls.ReadOption() |> Option.map(converter)
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.WriteWithLen(this.Features.ToByteArray())
        ls.Write(this.NodeOne.Value)
        this.OneToTwo |> Option.map(fun x -> x.ToBytes()) |> ls.WriteOption
        ls.Write(this.NodeTwo.Value)
        this.TwoToOne |> Option.map(fun x -> x.ToBytes()) |> ls.WriteOption
        this.AnnouncementMsg |> Option.map(fun x -> x.ToBytes()) |> ls.WriteOption
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
        
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        ChannelInfo.Deserialize(ls)

    
/// Information received in the latest node_announcement from this node.
type NodeAnnouncementInfo = {
    /// Protocol features the node announced support for
    Features: FeatureBits
    /// When the last known update to the node state was issued. Value is opaque, as set in the announcement.
    LastUpdate: uint32
    /// Color assigned to the node.
    RGB: RGB
    Alias: uint256
    Addresses: NetAddress array
    AnnouncementMsg: NodeAnnouncementMsg option
}
    with
    static member FromMsg(msg: UnsignedNodeAnnouncementMsg) =
        {
            Features = msg.Features
            LastUpdate = msg.Timestamp
            RGB = msg.RGB
            Alias = msg.Alias
            Addresses = msg.Addresses
            AnnouncementMsg = None
        }
    static member Deserialize(ls: LightningReaderStream) =
        {
            Features = ls.ReadWithLen() |> FeatureBits.CreateUnsafe
            LastUpdate = ls.ReadUInt32(false)
            RGB = ls.ReadRGB()
            Alias = ls.ReadUInt256 false
            Addresses =
                let addrLen = ls.ReadUInt64(false)
                let ret = ResizeArray()
                for i in 0..(int addrLen - 1) do
                    ret.Add(NetAddress.ReadFrom(ls) |> Result.deref)
                ret.ToArray()
            AnnouncementMsg =
                let converter =
                    ILightningSerializable.fromBytes<NodeAnnouncementMsg>
                ls.ReadOption() |> Option.map(converter)
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.WriteWithLen(this.Features.ToByteArray())
        ls.Write(this.LastUpdate, false)
        ls.Write(this.RGB)
        ls.Write(this.Alias, false)
        let addrLen = this.Addresses |> Seq.length |> uint64
        ls.Write(addrLen, false)
        this.Addresses |> Seq.iter(fun a -> a.WriteTo ls)
        this.AnnouncementMsg |> Option.map(fun m -> m.ToBytes()) |> ls.WriteOption
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
        
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        NodeAnnouncementInfo.Deserialize(ls)
    
type NodeInfo = {
    Channels: ShortChannelId list
    LowestInboundChannelFees: RoutingFees option
    AnnouncementInfo: NodeAnnouncementInfo option
}
    with
    static member Deserialize(ls: LightningReaderStream) =
        {
            Channels =
                let len = int(ls.ReadUInt64(false))
                let mutable ret = []
                for i in 0..(len-1) do
                    ret <- (ls.ReadUInt64(false) |> ShortChannelId.FromUInt64) :: (ret)
                ret
            LowestInboundChannelFees = ls.ReadOption() |> Option.map(RoutingFees.FromBytes)
            AnnouncementInfo = ls.ReadOption() |> Option.map(NodeAnnouncementInfo.FromBytes)
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write(uint64 this.Channels.Length, false)
        this.Channels |> Seq.rev |> Seq.iter(fun c -> ls.Write(c))
        this.LowestInboundChannelFees |> Option.map(fun f -> f.ToBytes()) |> ls.WriteOption
        this.AnnouncementInfo |> Option.map(fun a -> a.ToBytes()) |> ls.WriteOption
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        NodeInfo.Deserialize(ls)

type NetworkGraph = internal {
    _Channels: Map<ShortChannelId, ChannelInfo>
    _Nodes: Map<ComparablePubKey, NodeInfo>
}
    with
    member this.Channels = this._Channels
    member this.Nodes = this._Nodes
    static member private Channels_: Lens<_,_> =
        (fun state -> state._Channels),
        (fun c state -> { state with _Channels = c })
    static member private Nodes_: Lens<_,_> =
        (fun state -> state._Nodes),
        (fun n state -> { state with _Nodes = n })
    static member Zero =
        { _Channels = Map.empty
          _Nodes = Map.empty }
    static member Deserialize(ls: LightningReaderStream) =
        {
            _Channels =
                let len = int(ls.ReadUInt64(false))
                let ret = ResizeArray len
                for i in 0..(len - 1) do
                    ret.Add((ls.ReadUInt64(false) |> ShortChannelId.FromUInt64, ChannelInfo.Deserialize ls))
                ret |> Map.ofSeq
            _Nodes =
                let len = int(ls.ReadUInt64(false))
                let ret = ResizeArray len
                for i in 0..(len - 1) do
                    ret.Add((ls.ReadPubKey() |> ComparablePubKey, NodeInfo.Deserialize ls))
                ret |> Map.ofSeq
        }
        
    member this.Serialize(ls: LightningWriterStream) =
        ls.Write((uint64 this._Channels.Count), false)
        this._Channels |> Map.iter(fun k v -> ls.Write(k); ls.Write(v.ToBytes()))
        ls.Write(uint64 this._Nodes.Count, false)
        this._Nodes |> Map.iter(fun k v -> ls.Write(k.Value); ls.Write(v.ToBytes()))
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
        
    static member FromBytes(data: byte[]) =
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        NetworkGraph.Deserialize(ls)
    
    member this.GetAddress(pubkey) =
        match this.Nodes.TryGetValue(ComparablePubKey pubkey) with
        | true, node ->
            node.AnnouncementInfo |> Option.map(fun ann -> ann.Addresses)
        | false, _ ->
            None
            
