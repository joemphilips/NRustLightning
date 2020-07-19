using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Net.Sockets
{
    internal static class BufferExtensions
    {
        public static ArraySegment<byte> GetArray(this Memory<byte> memory)
        {
            return ((ReadOnlyMemory<byte>)memory).GetArray();
        }

        public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("buffer backed by array was expected");
            }
            return result;
        }
    }
}