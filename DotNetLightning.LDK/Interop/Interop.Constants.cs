using System;
namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        internal const string RustLightning = "bindings.dylib";
        internal static readonly UIntPtr BLOCK_SIZE = new UIntPtr(64);

    }
}