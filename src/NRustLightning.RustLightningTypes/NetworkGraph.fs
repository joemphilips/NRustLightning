namespace RustLightningTypes

open System.Collections.Generic
open DotNetLightning.Utils

open System
open System.IO
open DotNetLightning.Serialize
open DotNetLightning.Serialize.Msgs
open DotNetLightning.Utils.Aether
open DotNetLightning.Utils.Aether.Operators
open NBitcoin
open NBitcoin.Crypto
open ResultUtils


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
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    

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
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    

type ChannelInfo = {
    Features: FeatureBit
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
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
/// Information received in the latest node_announcement from this node.
type NodeAnnouncementInfo = {
    /// Protocol features the node announced support for
    Features: FeatureBit
    /// When the last known update to the node state was issued. Value is opaque, as set in the announcement.
    LastUpdate: uint32
    /// Color assigned to the node.
    RGB: RGB
    Alias: uint256
    Addresses: NetAddress seq
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
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
type NodeInfo = {
    Channels: ShortChannelId list
    LowestInboundChannelFess: RoutingFees option
    AnnouncementInfo: NodeAnnouncementInfo option
}
    with
    static member Deserialize(ls: LightningReaderStream) =
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    

type NetworkGraph = private {
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
        failwith ""
        
    member this.Serialize(ls: LightningWriterStream) =
        failwith ""
        
    member this.ToBytes() =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        this.Serialize(ls)
        ms.ToArray()
    
    member this.GetAddress(pubkey) =
        match this.Nodes.TryGetValue(ComparablePubKey pubkey) with
        | true, node ->
            node.AnnouncementInfo |> Option.map(fun ann -> ann.Addresses)
        | false, _ ->
            None
            
    member private this.UpdateNodeFromAnnouncement(msg: NodeAnnouncementMsg, ?remoteNodeId: PubKey) =
        result {
            do!
                match remoteNodeId with
                | Some k ->
                    msg.Contents.ToBytes()
                    |> Hashes.SHA256
                    |> fun m -> k.VerifyMessage(m, msg.Signature.Value)
                    |> Result.requireTrue "Failed to verify signature"
                | None -> Ok()
            match this.Nodes.TryGetValue(ComparablePubKey msg.Contents.NodeId.Value) with
            | false, _ ->
                return! Error("No existing channels for node_announcement")
            | true, node ->
                do!
                    match node.AnnouncementInfo |> Option.bind(fun ann -> if ann.LastUpdate >= msg.Contents.Timestamp then Some() else None) with
                    | Some _ -> Error "Update older than last processed update"
                    | None _ -> Ok()
                    
                let shouldRelay = msg.Contents.ExcessData.Length = 0 && msg.Contents.ExcessAddressData.Length = 0
                return Ok(shouldRelay, { node with AnnouncementInfo = Some({ NodeAnnouncementInfo.FromMsg(msg.Contents) with AnnouncementMsg = if shouldRelay then Some(msg) else None }) })
        }
        
    member private this.UpdateChannelFromAnnouncement(msg: ChannelAnnouncementMsg, checkedUtxo: bool) =
        result {
            let msgHash = msg.Contents.ToBytes() |> Hashes.SHA256
            do! msg.Contents.NodeId1.Value.VerifyMessage(msgHash, msg.NodeSignature1.Value) |> Result.requireTrue "Failed to verify node signature1"
            do! msg.Contents.NodeId2.Value.VerifyMessage(msgHash, msg.NodeSignature2.Value) |> Result.requireTrue "Failed to verify node signature2"
            do! msg.Contents.BitcoinKey1.Value.VerifyMessage(msgHash, msg.BitcoinSignature1.Value) |> Result.requireTrue "Failed to verify bitcoin signature 1"
            do! msg.Contents.BitcoinKey2.Value.VerifyMessage(msgHash, msg.BitcoinSignature2.Value) |> Result.requireTrue "Failed to verify bitcoin signature 2"
            
            let shouldRelay= msg.Contents.ExcessData.Length = 0
            let channelInfo = { ChannelInfo.FromMsg(msg.Contents) with AnnouncementMsg = if shouldRelay then Some(msg) else None }
            
            let! channels =
                match this._Channels.TryGetValue(msg.Contents.ShortChannelId) with
                | true, _ ->
                    if checkedUtxo then
                        this._Channels |> Map.remove msg.Contents.ShortChannelId |> Map.add msg.Contents.ShortChannelId channelInfo
                        |> Ok
                    else
                        Error "Already have knowledge of channel"
                | false, _ ->
                    this.Channels |> Map.add(msg.Contents.ShortChannelId) (channelInfo) |> Ok
                
            let addChannelToNode (nodes: Map<_,_>) (NodeId nodeId) =
                let nodeId = ComparablePubKey(nodeId)
                match (nodes |> Map.tryFind nodeId) with
                | Some _ ->
                    nodes |> Map.map(fun _ nodeInfo -> { nodeInfo with Channels = (msg.Contents.ShortChannelId) :: nodeInfo.Channels })
                | None ->
                    nodes |> Map.add (nodeId) ({ NodeInfo.Channels = [msg.Contents.ShortChannelId]
                                                 LowestInboundChannelFess = None
                                                 AnnouncementInfo = None })
            let nodes = addChannelToNode this.Nodes msg.Contents.NodeId1
            let nodes = addChannelToNode nodes msg.Contents.NodeId2
            return (shouldRelay, { this with _Channels = channels; _Nodes = nodes})
        }

    member private this.RemoveChannelInNodes(m: ChannelInfo, shortChannelId: ShortChannelId) =
        failwith ""
    /// Close a channel if a corresponding HTLC fail was sent.
    /// If permanent, removes a channel from the local storage.
    /// May cause the removal of nodes too, if this was their last channel.
    /// If not permanent, makes channels unavailable for routing.
    member this.CloseChannelFromUpdate(shortChannelId: ShortChannelId, isPermanent: bool) =
        if isPermanent then
            match this.Channels |> Map.tryFind shortChannelId with
            | Some m -> this.RemoveChannelInNodes(m, shortChannelId)
            | None -> this
        else
            let partialPrism = NetworkGraph.Channels_ >-> Map.value_ shortChannelId >-> Option.value_
            let lens1 = partialPrism >?> ChannelInfo.OneToTwo_ >?> Option.value_ >?> DirectionalChannelInfo.Enabled_
            let lens2 = partialPrism >?> ChannelInfo.TwoToOne_ >?> Option.value_ >?> DirectionalChannelInfo.Enabled_
            Optic.set (lens1) false this
            |> Optic.set (lens2) false
            
    member private this.UpdateChannel(msg: ChannelUpdateMsg) =
        let chanEnabled = msg.Contents.ChannelFlags <> (1uy <<< 1)
        let mutable chanWasEnabled = false
        result {
            let! channel =
                match this.Channels |> Map.tryFind msg.Contents.ShortChannelId with
                | Some c -> Ok(c)
                | None -> Error("Couldn't find channel for update")
                
            let maybeUpdateChannelInfo (target: DirectionalChannelInfo option) (NodeId srcNode) =
                result {
                    do! 
                        match target with
                        | Some existingChanInfo ->
                            if (existingChanInfo.LastUpdate >= msg.Contents.Timestamp) then
                                Error("Update older than last processed update")
                            else
                                chanWasEnabled <- existingChanInfo.Enabled
                                Ok()
                        | None ->
                            chanWasEnabled <- false
                            Ok()
                            
                    return { DirectionalChannelInfo.Enabled = chanEnabled
                             LastUpdate = msg.Contents.Timestamp
                             CltvExpiryDelta = msg.Contents.CLTVExpiryDelta
                             HTLCMinimumMSat = msg.Contents.HTLCMinimumMSat
                             Fees = RoutingFees.FromChannelUpdate (msg.Contents)
                             LastUpdateMessage = Some(msg) }
                }
                
            let msgHash = msg.Contents.ToBytes() |> Hashes.Hash256
            
            let! (channel, destNodeId) =
                if (msg.Contents.MessageFlags &&& 1uy = 1uy) then
                    if (not <| channel.NodeTwo.Value.Verify(msgHash, msg.Signature.Value)) then
                        Error("Failed to verify signature")
                    else
                        maybeUpdateChannelInfo channel.TwoToOne channel.NodeTwo
                        |> Result.map(fun newDChannelInfo -> { channel with TwoToOne = Some newDChannelInfo })
                        |> Result.map(fun newChannelInfo -> (newChannelInfo, channel.NodeOne.Value))
                else
                    if (not <| channel.NodeOne.Value.Verify(msgHash, msg.Signature.Value)) then
                        Error("Failed to verify signature")
                    else
                        maybeUpdateChannelInfo channel.OneToTwo channel.NodeOne
                        |> Result.map(fun newDChannelInfo -> { channel with OneToTwo = Some newDChannelInfo })
                        |> Result.map(fun newChannelInfo -> (newChannelInfo, channel.NodeTwo.Value))
                    
            let node = this.Nodes |> Map.find (ComparablePubKey destNodeId)
            let node =
                if chanEnabled then
                    let mutable baseMSat = msg.Contents.FeeBaseMSat
                    let mutable proportionalMillionths = msg.Contents.FeeProportionalMillionths
                    do
                        match node.LowestInboundChannelFess with
                        | Some fees ->
                            baseMSat <- LNMoney.Min(baseMSat, fees.BaseMSat |> LNMoney.MilliSatoshis)
                            proportionalMillionths <- Math.Min(proportionalMillionths, fees.ProportionalMillionths)
                        | None -> ()
                    { node with
                          LowestInboundChannelFess =
                            Some({ RoutingFees.BaseMSat = (uint32)baseMSat.MilliSatoshi
                                   ProportionalMillionths = proportionalMillionths }) }
                else
                    let mutable lowestInboundChannelFees = None
                    node.Channels
                    |> List.iter(fun chanId ->
                        let chan = this._Channels |> Map.find chanId
                        if chan.NodeOne.Value = destNodeId then chan.TwoToOne else chan.OneToTwo
                        |> Option.iter (fun chanInfo ->
                            if chanInfo.Enabled then
                                failwith "Not implemented"
                            else
                                failwith "Not implemented"
                        )
                        ()
                    )
                    failwith "Not implemented"
            return ({ this with
                          _Nodes = this.Nodes |> Map.add(ComparablePubKey destNodeId) node
                          _Channels = this._Channels |> Map.add(msg.Contents.ShortChannelId) channel })
        }
