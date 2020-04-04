using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Utils;

namespace DotNetLightning.LDK.Adaptors
{
    public static class PrimitiveExtensions
    {
        public static Span<byte> AsSpan(this FFIScript ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static Span<byte> AsSpan(this SecretKey ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static Span<byte> AsSpan(this PublicKey ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static Span<byte> AsSpan(this FFISha256dHash ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static Span<byte> AsSpan(this FFITransaction ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static Span<byte> AsSpan(this FFIBlock ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
    }
}