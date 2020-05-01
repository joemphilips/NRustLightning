using System;
using System.Buffers;
using NRustLightning.Adaptors;

namespace NRustLightning.Utils
{
    /// <summary>
    /// A MemoryManager over a raw pointer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T: unmanaged
    {
        private readonly T* _pointer;
        private readonly int _length;

        /// <summary>
        /// This assumes that provided span is already unmanaged or externally pinned.
        /// Otherwise, it will probably cause undefined behavior.
        /// </summary>
        /// <param name="span"></param>
        public UnmanagedMemoryManager(Span<T> span)
        {
            fixed (T* ptr = span)
            {
                _pointer = ptr;
                _length = span.Length;
            }
        }

        [CLSCompliant(false)]
        public UnmanagedMemoryManager(T* pointer, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            _pointer = pointer;
            _length = length;
        }

        public UnmanagedMemoryManager(IntPtr ptr, int length) : this((T*)ptr.ToPointer(), length) {}
        public UnmanagedMemoryManager(ref FFIBytes b) : this(b.ptr, (int)b.len) {}
        
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public override Span<T> GetSpan() => new Span<T>(_pointer, _length);

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            throw new NotImplementedException();
        }

        public override void Unpin()
        {
            throw new NotImplementedException();
        }
    }
}