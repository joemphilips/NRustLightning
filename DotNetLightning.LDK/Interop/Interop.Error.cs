using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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

        #region simple_struct_alloc_dealloc
        public static FFIResult ffi_test_ok(bool check = true)
        {
            return MaybeCheck(_ffi_test_ok(), check);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void FFITestFn();

        [StructLayout(LayoutKind.Sequential)]
        internal struct FFITestInputStruct
        {
            public FFITestFn fn;
        }

        internal class FFITestOutputStructHandle : SafeHandle
        {
            private FFITestOutputStructHandle() : base(IntPtr.Zero, true)
            {}

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                if (handle == IntPtr.Zero) return true;

                var h = handle;
                handle = IntPtr.Zero;

                ffi_test_release(h, false);
                return true;
            }
        }
        
        [DllImport(RustLightning, EntryPoint = "ffi_test_simple_struct_with_function_pointer",
            ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _ffi_test_simple_struct_with_function_pointer(IntPtr input, out FFITestOutputStructHandle handle);

        internal static FFIResult ffi_test_simple_struct_with_function_pointer(
            IntPtr inputStruct, out FFITestOutputStructHandle handle)
        {
             return MaybeCheck(_ffi_test_simple_struct_with_function_pointer(inputStruct, out handle), true);   
        }
        
        [DllImport(RustLightning, EntryPoint = "ffi_test_release",
            ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _ffi_test_release(IntPtr handle);

        internal static FFIResult ffi_test_release(IntPtr handle, bool check = true)
            => MaybeCheck(_ffi_test_release(handle), check);

        internal class FFITestInputInterface
        {
            internal static void Action1() => Console.WriteLine("Function has been called!");
            internal FFITestInputInterface() {}

            internal FFITestInputStruct ToFFI()
            {
                var t = new FFITestInputStruct();
                t.fn = Action1;
                return t;
            }
        }

        internal class TestOutputStruct : IDisposable
        {
            private readonly FFITestOutputStructHandle _handle;
            internal TestOutputStruct(FFITestOutputStructHandle handle)
            {
                _handle = handle ?? throw new ArgumentNullException(nameof(handle));
            }

            internal static unsafe TestOutputStruct Create(
                FFITestInputStruct input
            )
            {
                var i = (IntPtr)Unsafe.AsPointer(ref input);
                ffi_test_simple_struct_with_function_pointer(i, out var handle);

                return new TestOutputStruct(handle);
            }

            public void Dispose()
            {
                _handle.Dispose();
            }
        }
        
        [DllImport(RustLightning, EntryPoint = "ffi_test_function_ptr",
            ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _ffi_test_function_ptr(in FFITestInputStruct struct_arg_pointer, in FFITestFn2 fn);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void FFITestFn2();
        internal static FFIResult ffi_test_function_ptr()
        {
            FFITestFn2 fn = () => Console.WriteLine("");
            var ffiTestInput = new FFITestInputStruct();
            return MaybeCheck(_ffi_test_function_ptr(in ffiTestInput, fn), true);
        }
        
        #endregion
#endif
    }
}