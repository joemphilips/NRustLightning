using System;
using System.Runtime.InteropServices;
using NRustLightning.Utils;

namespace NRustLightning.Adaptors
{
    public static class ArrayLikePrimitiveExtensions
    {
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFIScript ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }

        public static byte[] AsArray(this FFIScript ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFISecretKey ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        public static byte[] AsArray(this FFISecretKey ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        
        
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFIPublicKey ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        public static byte[] AsArray(this FFIPublicKey ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFISha256dHash ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        
        public static byte[] AsArray(this FFISha256dHash ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFITransaction ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        public static byte[] AsArray(this FFITransaction ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        
        /// <summary>
        /// Be super careful that this function only works in the delegate.
        /// Because rust side will free the memory when finished calling it.
        /// If you want to use the data from out side of the delegate, use `ToArray()`
        /// instead, which will make a copy of the data.
        /// </summary>
        /// <param name="ffi"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this FFIBlock ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        public static byte[] AsArray(this FFIBlock ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }

        public static Span<byte> AsSpan(this FFIBytes ffi)
        {
            var size = (int) ffi.len;
            unsafe
            {
                return new Span<byte>(ffi.ptr.ToPointer(), size);
            }
        }
        public static byte[] AsArray(this FFIBytes ffi)
        {
            var arr = new byte[(int)ffi.len];
            var span = ffi.AsSpan();
            span.CopyTo(arr);
            return arr;
        }
    }
}