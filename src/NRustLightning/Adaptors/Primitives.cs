using System.Runtime.InteropServices;
using NBitcoin;

namespace NRustLightning.Adaptors
{

    public struct RouteHop
    {
        private readonly byte[] PubKey;
        private readonly byte[] NodeFeatures;
        private readonly ulong ShortChannelId;
        private readonly byte[] ChannelFeatures;
        private readonly ulong FeeMsat;
        private readonly uint cltvExpiryDelta;

        public void Foo()
        {
        }
    }
}
