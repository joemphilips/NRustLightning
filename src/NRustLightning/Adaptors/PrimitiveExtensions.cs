using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    public static class PrimitiveExtensions
    {
        
        public static Span<byte> AsSpan<T>(this T value) where T : unmanaged
        {
            Span<T> tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            Span<byte> span = MemoryMarshal.AsBytes(tSpan);
            return span;
        }
        
        public static byte[] AsArray<T>(this T value) where T: unmanaged
        {
            var arr = new byte[Marshal.SizeOf<T>()];
            var span = value.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
    }
}