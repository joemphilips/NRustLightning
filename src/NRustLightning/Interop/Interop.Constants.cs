using System;
namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        static readonly UIntPtr BLOCK_SIZE = new UIntPtr(64);
#if AOT
        const string RustLightning = "*";
#elif WINDOWS
        const string RustLightning = "Native/bindings.dll";
#elif MACOS
        const string RustLightning = "Native/libbindings.dylib";
#elif LINUX
        const string RustLightning = "Native/libbindings.so";
#elif UNIX
        const string RustLightning = "Native/libbindings.so";
#else // without this, editor might not notice the field.
        const string RustLightning = "Native/libbindings.so";
#endif
    }
}