module Generators.RustLightningTypes

open DotNetLightning.Utils
open FsCheck
open NRustLightning.RustLightningTypes
open Generators.Generator
open Generators.Msgs

let private directionalChannelInfoGen = gen {
    let! lastUpdate = Arb.generate<uint32>
    let! enabled = Arb.generate<bool>
    let! cltvExpiryDelta = Arb.generate<uint16> |> Gen.map(BlockHeightOffset16)
    let! htlcMinimum = lnMoneyGen
    let! fees = Arb.generate<RoutingFees>
    let! m = channelUpdateGen |> optionGen
    return  {
        DirectionalChannelInfo.LastUpdate = lastUpdate
        Enabled = enabled
        CltvExpiryDelta = cltvExpiryDelta
        HTLCMinimumMSat = htlcMinimum
        Fees = fees
        LastUpdateMessage = m
    }
}

let channelInfoGen = gen {
    let! f = featuresGen
    let! nOne = pubKeyGen |> Gen.map(NodeId)
    let! oneToTwo = directionalChannelInfoGen |> optionGen
    let! nTwo = pubKeyGen |> Gen.map(NodeId)
    let! twoToOne = directionalChannelInfoGen |> optionGen
    let! m = channelAnnouncementGen |> optionGen
    return {
        ChannelInfo.Features = f
        NodeOne = nOne
        OneToTwo = oneToTwo
        NodeTwo = nTwo
        TwoToOne = twoToOne
        AnnouncementMsg = m
    }
}

let nodeAnnouncementInfoGen = gen {
    let! f = featuresGen
    let! l = Arb.generate<uint32>
    let! rgb = (fun r g b -> {Red = r; Green = g; Blue = b})
                <!> Arb.generate<uint8>
                <*> Arb.generate<uint8>
                <*> Arb.generate<uint8>
    let! a = uint256Gen
    let! addrs = netAddressesGen |> Gen.map (Array.choose id)
    let! m = nodeAnnouncementGen |> optionGen
    return {
        Features = f
        LastUpdate = l
        RGB = rgb
        Alias = a
        Addresses = addrs
        AnnouncementMsg = m
    }
}

let nodeInfoGen = gen {
    let! c =  shortChannelIdsGen |> Gen.listOf
    let! f = Arb.generate<RoutingFees> |> optionGen
    let! a = nodeAnnouncementInfoGen |> optionGen
    return {
        NodeInfo.Channels = c
        LowestInboundChannelFees = f
        AnnouncementInfo = a
    }
}
let networkGraphGen = gen {
    let! c = (shortChannelIdsGen, channelInfoGen) ||> Gen.map2(fun s c -> (s, c)) |> Gen.listOf |> Gen.map Map.ofSeq
    let! n = (pubKeyGen |> Gen.map(ComparablePubKey), nodeInfoGen) ||> Gen.map2(fun s c -> (s, c)) |> Gen.listOf |> Gen.map Map.ofSeq
    return {
        _Channels = c
        _Nodes = n
    }
}
    
type RustLightningTypeGenerators =
    static member public NetworkGraph =
        Arb.fromGen(networkGraphGen)

    static member NodeInfo =
        Arb.fromGen(nodeInfoGen)
        
    static member NodeAnnouncementInfo =
        Arb.fromGen(nodeAnnouncementInfoGen)
    static member ChannelInfo =
        Arb.fromGen(channelInfoGen)
        
    static member DirectionalChannelInfo = Arb.fromGen(directionalChannelInfoGen)

