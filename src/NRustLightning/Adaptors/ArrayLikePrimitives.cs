using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIScript
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct SecretKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct PublicKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFISha256dHash
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIOutPoint
    {
        internal readonly FFIScript script_pub_key;
        internal readonly ushort index;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFITxOut
    {
        internal readonly ulong value;
        internal readonly FFIScript script_pub_key;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFITransaction
    {
        public readonly IntPtr ptr;
        public readonly UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIBlock
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
    }
    
}