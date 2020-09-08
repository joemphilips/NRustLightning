/*
 * You can think these are just a type alias for Span<T>
 */

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using DotNetLightning.Serialization;
using NBitcoin;
using NBitcoin.Protocol;
using NRustLightning.Adaptors;
namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIScript
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public Script ToScript()
        {
            using var m = new MemoryStream(this.AsArray());
            var stream = new BitcoinStream(m, false);
            var s = Script.Empty;
            stream.ReadWrite(ref s);
            return s;
        }
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
        public Transaction AsTransaction(NBitcoin.Network n)
        {
            var t = Transaction.Create(n);
            t.FromBytes(this.AsArray());
            return t;
        }

    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIBlock
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
        public Block ToBlock(NBitcoin.Network n) => Block.Load(this.AsArray(), n);
    }
    
    #region internal members
   
    [StructLayout(LayoutKind.Sequential)]
    internal readonly ref struct FFIRoute
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFIRoute(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIBytes
    {
        internal readonly IntPtr ptr;
        public readonly UIntPtr len;
        public FFIBytes(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
        
    }
    
    # endregion
}
