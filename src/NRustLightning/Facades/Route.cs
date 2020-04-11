using System;
using System.Collections.Generic;
using NBitcoin;

namespace NRustLightning.Facades
{

    public struct RouteHop
    {
        private readonly PubKey PubKey;
        private readonly byte[] NodeFeatures;
        private readonly ulong ShortChannelId;
        private readonly byte[] ChannelFeatures;
        private readonly ulong FeeMsat;
        private readonly uint cltvExpiryDelta;
    }
    public class Route
    {
        public List<RouteHop> Hops;
        public Span<byte> AsSpan()
        {
            throw new NotImplementedException();
        }
    }
}