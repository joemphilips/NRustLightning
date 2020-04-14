using System;
using System.Collections.Generic;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;

namespace NRustLightning.Facades
{

    public class RouteHopWithFeature
    {
        public readonly Primitives.NodeId NodeId;
        public readonly FeatureBit NodeFeatures;
        public readonly ulong ShortChannelId;
        public readonly FeatureBit ChannelFeatures;
        public readonly ulong FeeMsat;
        public readonly uint CltvExpiryDelta;
        public RouteHopWithFeature(
            Primitives.NodeId nodeId,
            FeatureBit nodeFeatures,
            ulong shortChannelId,
            FeatureBit channelFeatures,
            ulong feeMsat,
            uint cltvExpiryDelta)
        {
            NodeId = nodeId ?? throw new ArgumentNullException(nameof(nodeId));
            NodeFeatures = nodeFeatures ?? throw new ArgumentNullException(nameof(nodeFeatures));
            ShortChannelId = shortChannelId;
            ChannelFeatures = channelFeatures ?? throw new ArgumentNullException(nameof(channelFeatures));
            FeeMsat = feeMsat;
            CltvExpiryDelta = cltvExpiryDelta;
        }
    }
    
    public class RouteWithFeature
    {
        public readonly RouteHopWithFeature[] Hops;

        public RouteWithFeature(params RouteHopWithFeature[] hops)
        {
            Hops = hops;
        }
        public byte[] AsArray()
        {
            var result = new List<byte>();
            var length = (byte)Hops.Length;
            result.Add(length);
            foreach (var hop in Hops)
            {
                result.AddRange(hop.NodeId.Value.ToBytes());
                result.AddRange(hop.NodeFeatures.ToByteArrayWithLength());
                result.AddRange(hop.ShortChannelId.AsArray());
                result.AddRange(hop.ChannelFeatures.ToByteArrayWithLength());
                result.AddRange(hop.FeeMsat.AsArray());
                result.AddRange(hop.CltvExpiryDelta.AsArray());
            }
            return result.ToArray();
        }
    }
}