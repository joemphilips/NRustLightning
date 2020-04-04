using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFIScript
    {
        internal IntPtr ptr;
        internal UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct SecretKey
    {
        internal IntPtr ptr;
        internal UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct PublicKey
    {
        internal IntPtr ptr;
        internal UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFISha256dHash
    {
        internal IntPtr ptr;
        internal UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFIOutPoint
    {
        internal FFIScript script_pub_key;
        internal ushort index;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFITxOut
    {
        internal ulong value;
        internal FFIScript script_pub_key;
    }

    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFITransaction
    {
        public IntPtr ptr;
        public UIntPtr len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFIBlock
    {
        internal IntPtr ptr;
        internal UIntPtr len;
    }
    
}