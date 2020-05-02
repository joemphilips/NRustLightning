using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FFIResult
    {
        private enum Kind : uint
        {
            Ok,
            EmptyPointerProvided,
            InvalidDataLength,
            DeserializationFailure,
            BufferTooSmall,
            InternalError,
        }

        private readonly Kind _kind;
        private readonly uint _id;
        public static (FFIResult, string) GetLastResult()
        {
            return LastResult.GetLastResult();
        }

        public bool IsSuccess => _kind == Kind.Ok;
        public bool IsBufferTooSmall => _kind == Kind.BufferTooSmall;

        internal FFIResult Check()
        {
            if (IsSuccess) return this;
            
            var (lastResult, msg) = GetLastResult();

            // Check whether the last result kind and id are the same
            // We need to use both because successful results won't
            // bother setting the id (it avoids some synchronization)
            if (lastResult._kind == _kind && lastResult._id == _id)
                throw new Exception($"FFI against rust-lightning failed ({_kind}), {msg?.TrimEnd()}");

            throw new Exception($"FFI against rust-lightning failed with {_kind}");
        }
    }
}