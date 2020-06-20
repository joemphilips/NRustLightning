using System;
using NBitcoin;

namespace NRustLightning.Adaptors
{
    public enum Network : int
    {
        MainNet = 0,
        TestNet = 1,
        RegTest = 2
    }
    
    public static class Extensions
    {
        public static Network ToFFINetwork(this NBitcoin.Network n)
        {
            if (n.NetworkType == NetworkType.Mainnet)
            {
                return Network.MainNet;
            }

            if (n.NetworkType == NetworkType.Testnet)
            {
                return Network.TestNet;
            }

            if (n.NetworkType == NetworkType.Regtest)
            {
                return Network.RegTest;
            }
            
            throw new Exception($"Unknown network type {n.NetworkType}");
        }
    }
}