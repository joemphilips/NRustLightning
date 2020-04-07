using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Facades;

namespace DotNetLightning.LDK.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFIRouteHop
    {
        public readonly IntPtr pubkey_ptr;
        public readonly UIntPtr pubkey_len;
        public readonly IntPtr node_features_ptr;
        public readonly UIntPtr node_features_len;
        public readonly ulong ShortChannelId;
        public readonly IntPtr channel_features_ptr;
        public readonly UIntPtr channel_features_len;
        public readonly ulong FeeMsat;
        public readonly uint CLTVExpiryDelta;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FFIRouteHops
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFIRouteHops(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FFIRoute
    {
        public readonly FFIRouteHops Hops;
    }
}