using System;
using System.Text;

namespace NRustLightning.Adaptors
{
    public class LastResult
    {
        public static (FFIResult, string) GetLastResult()
            => FillLastResult(new Span<byte>(new byte[1024]));

        private static unsafe (FFIResult, string) FillLastResult(Span<byte> buffer)
        {
            fixed (byte* messageBufPtr = buffer)
            {
                var result = Interop.ffi_last_result(
                    (IntPtr) messageBufPtr,
                    (UIntPtr) buffer.Length,
                    out var actualMessageLen,
                    out var lastResult);

                if (result.IsBufferTooSmall) return FillLastResult(new Span<byte>(new byte[(int) actualMessageLen]));

                return (lastResult, Encoding.UTF8.GetString(messageBufPtr, (int) actualMessageLen));
            }
        }
    }
}