using System;
using System.IO;
using System.Runtime.InteropServices;
using DotNetLightning.Serialization;
using Msgs = DotNetLightning.Serialization.Msgs;

namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
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
        public Msgs.NodeAnnouncementMsg ParseArray()
        {
            using var ms = new MemoryStream(this.AsArray());
            using var ls = new LightningReaderStream(ms);
            var r = new Msgs.NodeAnnouncementMsg();
            ((Msgs.ILightningSerializable<Msgs.NodeAnnouncementMsg>)r).Deserialize(ls);
            return r;
        }
    }

}