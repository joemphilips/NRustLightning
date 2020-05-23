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

    [StructLayout(LayoutKind.Sequential, Size=34)]
    public unsafe ref struct FFIOutPoint
    {
        internal fixed byte txid[32];
        internal readonly ushort index;

        public uint256 GetTxId()
        {
            var bytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                bytes[i] = this.txid[i];
            }

            return new uint256(bytes);
        }

        public FFIOutPoint(uint256 txId, ushort index)
        {
            var bytes = txId.ToBytes();
            for (int i = 0; i < 32; i++)
            {
                this.txid[i] = bytes[i];
            }
            this.index = index;
        }

        public (uint256, ushort) ToTuple()
        {
            return (this.GetTxId(), this.index);
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
