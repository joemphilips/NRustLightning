using System;
using System.Linq;
using System.Diagnostics;

namespace NRustLightning.Utils
{
    internal static class BitConversionExtensions
    {
        public static ushort ToUInt16BE(this Span<byte> d)
        {
            Debug.Assert(d.Length == 2);
            if (BitConverter.IsLittleEndian)
                d.Reverse();
            return BitConverter.ToUInt16(d);
        }

        public static ushort ToUInt16BE(this byte[] a) => a.AsSpan().ToUInt16BE();
    }
}