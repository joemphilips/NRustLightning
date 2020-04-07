using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
namespace DotNetLightning.LDK.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFIScript
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SecretKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PublicKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFISha256dHash
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFIOutPoint
    {
        internal readonly FFIScript script_pub_key;
        internal readonly ushort index;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFITxOut
    {
        internal readonly ulong value;
        internal readonly FFIScript script_pub_key;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFITransaction
    {
        public readonly IntPtr ptr;
        public readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FFIBlock
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct NodeFeatures
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly  struct ChannelFeatures
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
}