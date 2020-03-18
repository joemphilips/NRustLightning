using System;
using System.Data;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {

        
        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void process_ffi(ChaCha20* self, UIntPtr input, UIntPtr output);

        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void encrypt_ffi(
            ChaCha20Poly1305RFCState* self,
            byte* input_ptr,
            ulong input_len,
            byte* output_ptr,
            ulong output_len,
            byte* out_tag_ptr,
            ulong output_tag_len);
    }
}