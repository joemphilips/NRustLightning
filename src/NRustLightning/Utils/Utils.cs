using System;
using System.Buffers;
using System.Diagnostics;
using NRustLightning.Adaptors;

namespace NRustLightning.Utils
{
    public static class Utils
    {
        public const int BUFFER_SIZE_UNIT = 1024;
        public const int MAX_BUFFER_SIZE = 65536;


        /// <summary>
        /// Call FFI function which writes return value into the buffer. If the buffer is too short, it will return
        /// FFIResult with a kind `BufferTooSmall`. In that case <see cref="WithVariableLengthReturnBuffer(x,x)"/>
        /// will call this function again with extended buffer.
        /// </summary>
        /// <param name="butPtr">pointer to the first element of return buffer</param>
        /// <param name="bufLen">length of the buffer</param>
        /// <returns>tuple of 1. result of the operation. 2. the actual length of the buffer that rl wants to return.</returns>
        internal delegate (FFIResult, UIntPtr) FFIOperationWithVariableLengthReturnBuffer(IntPtr butPtr, UIntPtr bufLen);
        
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
        internal static byte[] WithVariableLengthReturnBuffer(
            MemoryPool<byte> pool, FFIOperationWithVariableLengthReturnBuffer func) 
        {
            var initialBufLength = BUFFER_SIZE_UNIT;

            var (resultBuf, result, actualBufferLength) = InvokeWithLength(pool, initialBufLength, func);
            if (actualBufferLength > MAX_BUFFER_SIZE)
            {
                throw new FFIException(
                    $"Tried to return too long buffer form rust ({actualBufferLength}). This should never happen.",
                    result);
            }

            if (result.IsSuccess)
            {
                return resultBuf[..actualBufferLength];
            }

            if (result.IsBufferTooSmall)
            {
                var (resultBuf2, result2, actualBufferLength2) = InvokeWithLength(pool, actualBufferLength, func);
                Debug.Assert(actualBufferLength == actualBufferLength2);
                result2.Check();
                return resultBuf2[..actualBufferLength];
            }
            throw new FFIException($"Unexpected error in FFI Call {result}", result);
        }

        private static unsafe (byte[], FFIResult, int) InvokeWithLength(MemoryPool<byte> pool, int length, FFIOperationWithVariableLengthReturnBuffer func)
        {
            using var memoryOwner = pool.Rent(length);
            var span = memoryOwner.Memory.Span;
            fixed (byte* ptr = span)
            {
                var (result, actualBufferLength) = func.Invoke((IntPtr) ptr, (UIntPtr) span.Length);
                return (span.ToArray(), result, (int) actualBufferLength);
            }
        }
    }
}