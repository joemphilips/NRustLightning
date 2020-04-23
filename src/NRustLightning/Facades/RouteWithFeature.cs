using System;
using System.Collections.Generic;
using System.Linq;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using static DotNetLightning.Core.Utils.Extensions.PrimitiveExtensions;

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
    
    internal class RouteWithFeatureList
    {
        public readonly RouteHopWithFeature[] Hops;

        public RouteWithFeatureList(params RouteHopWithFeature[] hops)
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
                result.AddRange(hop.ShortChannelId.GetBytesBigEndian());
                result.AddRange(hop.ChannelFeatures.ToByteArrayWithLength());
                result.AddRange(hop.FeeMsat.GetBytesBigEndian());
                result.AddRange(hop.CltvExpiryDelta.GetBytesBigEndian());
            }
            return result.ToArray();
        }
    }

    public class RoutesWithFeature
    {
        private readonly RouteWithFeatureList[] Routes;
        public RoutesWithFeature(params RouteHopWithFeature[][] hopsList)
        {
            Routes = hopsList.Select(hops => new RouteWithFeatureList(hops)).ToArray();
        }

        public byte[] AsArray()
        {
            var result = new List<byte>();
            var length = (ulong)Routes.Length;
            result.AddRange(BitConverter.GetBytes(length).Reverse());
            foreach (var route in Routes)
            {
                result.AddRange(route.AsArray());
            }

            return result.ToArray();
        }
    }
}