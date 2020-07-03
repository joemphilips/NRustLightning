using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NBitcoin;

namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe ref struct FFIPublicKey
    {
        public fixed byte Data[33];
        public byte[] AsArray()
        {
            var result = new byte[33];
            for (int i = 0; i < 33; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe ref struct FFISecretKey
    {
        public fixed byte Data[32];

        public byte[] AsArray()
        {
            var result = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }

        public uint256 ToUInt256() => new uint256(this.AsArray());
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe ref struct FFISecret
    {
        public fixed byte Data[32];

        public byte[] AsArray()
        {
            var result = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }

        public uint256 ToUInt256() => new uint256(this.AsArray());
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe ref struct FFISha256dHash
    {
        public fixed byte Data[32];

        public byte[] AsArray()
        {
            var result = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }

        public Span<byte> AsSpan()
        {
           return new Span<byte>(Unsafe.AsPointer(ref this.Data[0]), 32);
        }

        public uint256 ToUInt256() => new uint256(this.AsArray(), false);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ActOne
    {
        public fixed byte Data[50];

        public byte[] AsArray()
        {
            var result = new byte[50];
            for (int i = 0; i < 50; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Array32
    {
        public fixed byte Data[32];

        public byte[] AsArray()
        {
            var result = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = Data[i];
            }

            return result;
        }
    }
    
    /// <summary>
    /// This is not consensus-compatible representation of OutPoint.
    /// Instead, the hash is encoded in big endian.
    /// This is because rl (or LN in general) serializes txid in big-endian
    /// </summary>
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

            return new uint256(bytes, true);
        }

        public FFIOutPoint(OutPoint outpoint) : this(outpoint.Hash, (ushort)outpoint.N, true)
        {
        }

        public FFIOutPoint(uint256 txId, ushort index, bool lendian = false)
        {
            var bytes = txId.ToBytes(lendian);
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
}