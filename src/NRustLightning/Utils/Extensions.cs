using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NRustLightning.Utils
{
    internal static class Hex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Encode4Bits(
            int src)
        {
            unchecked
            {
                // upper case
                int diff = 48;
                diff += ((9 - src) >> 31) & 7;
                return src + diff;

                // lower case
                ////int diff = 48;
                ////diff += ((9 - src) >> 31) & 39;
                ////return src + diff;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EncodeByte(
            byte b0,
            out int r0,
            out int r1)
        {
            unchecked
            {
                r0 = Encode4Bits(b0 >> 4);
                r1 = Encode4Bits(b0 & 15);
            }
        }
        
        public static void Encode(
            ReadOnlySpan<byte> bytes,
            Span<char> base16)
        {
            if (base16.Length != bytes.Length * 2)
            {
                throw new Exception(nameof(base16));
            }

            int di = 0;
            int si = 0;
            int b0, b1;

            while (bytes.Length - si >= 1)
            {
                EncodeByte(bytes[si++], out b0, out b1);
                base16[di++] = (char)b0;
                base16[di++] = (char)b1;
            }

            Debug.Assert(si == bytes.Length);
            Debug.Assert(di == base16.Length);
        }
        public static string Encode(
            ReadOnlySpan<byte> bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            Encode(bytes, chars);
            return new string(chars);
        }
    }
}