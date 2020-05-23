/*
 * You can think these are just a type alias for Span<T>
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NBitcoin;
using NRustLightning.Adaptors;
namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIScript
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public Script ToScript()
            => Script.FromBytesUnsafe(this.AsArray());
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFISecretKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFISecretKey(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIPublicKey
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;
        public FFIPublicKey(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFISha256dHash
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFISha256dHash(IntPtr ptr, UIntPtr len)
        {
            if (len != (UIntPtr) 32) throw new ArgumentException($"must be 32, it was {len}");
            this.ptr = ptr;
            this.len = len;
        }

        public uint256 ToUInt256() => new uint256(this.AsArray());
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFISecret
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFISecret(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIOutPoint
    {
        internal readonly uint256 txid;
        internal readonly ushort index;

        public FFIOutPoint(uint256 txid, ushort index)
        {
            this.txid = txid ?? throw new ArgumentNullException(nameof(txid));
            this.index = index;
        }

        public (uint256, ushort) ToTuple() => (txid, index);
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
