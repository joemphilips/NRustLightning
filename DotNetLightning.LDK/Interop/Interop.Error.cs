using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        
        [DllImport(RustLightning, EntryPoint = "ffi_last_result", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _ffi_last_result(
            IntPtr messageBuf,
            UIntPtr messageBufLen,
            out UIntPtr actualMessageLen,
            out FFIResult lastResult);

        public static FFIResult ffi_last_result(
            IntPtr messageBuf,
            UIntPtr messageBufLen,
            out UIntPtr actualMessageLen,
            out FFIResult lastResult,
            bool check = true)
            => MaybeCheck(_ffi_last_result(messageBuf, messageBufLen, out actualMessageLen, out lastResult), check);
#if DEBUG
        [DllImport(RustLightning, EntryPoint = "ffi_test_error", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _ffi_test_error();

        public static FFIResult ffi_test_error(bool check = true)
        {
            return MaybeCheck(_ffi_test_error(), check);
        }

        [DllImport(RustLightning, EntryPoint = "ffi_test_ok", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _ffi_test_ok();

        public static FFIResult ffi_test_ok(bool check = true)
        {
            return MaybeCheck(_ffi_test_ok(), check);
        }
#endif
    }
}