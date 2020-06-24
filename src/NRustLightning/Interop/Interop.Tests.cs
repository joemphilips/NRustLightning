using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;

namespace NRustLightning
{
    internal static partial class Interop
    {
        
#if DEBUG

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "test_event_serialization",
            ExactSpelling = true)]
        static extern FFIResult _test_event_serialization(
            IntPtr bufOut,
            UIntPtr bufLen,
            out UIntPtr actualLen
            );

        internal static FFIResult test_event_serialization(
            IntPtr bufOut,
            UIntPtr bufLen,
            out UIntPtr actualLen,
            bool check = true
        ) => MaybeCheck(_test_event_serialization(bufOut, bufLen, out actualLen), check);

#endif
    }
}