using System;
using Msgs = DotNetLightning.Serialize.Msgs;

namespace NRustLightning.Adaptors
{
    public readonly ref struct FFINodeAnnoucement
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public Span<byte> AsSpan()
        {
            var size = (int) len;
            unsafe
            {
                return new Span<byte>(ptr.ToPointer(), size);
            }
        }
        public byte[] AsArray()
        {
            var arr = new byte[(int)len];
            var span = AsSpan();
            span.CopyTo(arr);
            return arr;
        }
        public Msgs.NodeAnnouncement ParseArray()
        {
            return Msgs.ILightningSerializable.fromBytes<Msgs.NodeAnnouncement>(this.AsArray());
        }
    }

}