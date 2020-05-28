using System;
using System.Buffers;
using NRustLightning.Adaptors;

namespace NRustLightning.Utils
{
    public static class Utils
    {
        public const int BUFFER_SIZE_UNIT = 1024;
        public const int MAX_BUFFER_SIZE = 65536;
        
        /// <summary>
        /// Use this function when ffi function returns variable length return value
        /// It will rent `BUFFER_SIZE_UNIT` bytes from MemoryPool and tries to write a return value
        /// in to it. If the length was not enough, then it will try again with longer buffer.
        /// This process continues until the buffer reaches MAX_BUFFER_SIZE, and it will throws FFIException
        /// When that is even not enough.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="func"> actual ffi function, first argument is a pointer to a allocated buffer, second is the length of it, and third is a parent SafeHandle class</param>
        /// <param name="handle"></param>
        /// <param name="postProcess"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THandle"></typeparam>
        /// <returns></returns>
        /// <exception cref="FFIException"></exception>
        public static unsafe byte[] WithVariableLengthReturnBuffer<THandle>(
            MemoryPool<byte> pool,
            Func<IntPtr, UIntPtr, THandle, (FFIResult, UIntPtr)> func,
            THandle handle)
        {
            var currentBufferSize = BUFFER_SIZE_UNIT;

            while (true)
            {
                using var memoryOwner = pool.Rent(currentBufferSize);
                var span = memoryOwner.Memory.Span;
                fixed (byte* ptr = span)
                {
                    var (result, actualNodeIdsLength) = func.Invoke((IntPtr)ptr, (UIntPtr)span.Length, handle);
                    if ((int)actualNodeIdsLength > MAX_BUFFER_SIZE)
                    {
                        throw new FFIException(
                            $"Tried to return too long buffer form rust {currentBufferSize}. This should never happen.",
                            result);
                    }

                    if (result.IsSuccess)
                    {
                        return span.Slice(0, (int)actualNodeIdsLength).ToArray();
                    }

                    if (result.IsBufferTooSmall)
                    {
                        currentBufferSize = (int)actualNodeIdsLength;
                        continue;
                    }

                    result.Check();
                }
            }
        }
    }
}