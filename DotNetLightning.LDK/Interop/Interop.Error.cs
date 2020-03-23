using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int last_error_message(
            byte* buffer,
            int length
            );

        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        private static extern int last_error_length();
        
        internal static unsafe string last_error_message()
        {
            var error_length = last_error_length();
            if (error_length == 0)
                return String.Empty;
            Span<byte> error_bytes = stackalloc byte[error_length];

            int res;
            fixed(byte* b = error_bytes)
            {
                res = last_error_message(b, error_bytes.Length);
            }
            Debug.Assert(res != 0);

            return error_bytes.ToString();
        }
    }
}