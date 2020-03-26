using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Handles
{
    class ChannelManagerHandle : SafeHandle
    {
        private ChannelManagerHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_ffi_channel_manager(h, false);
            return true;
        }
    }
}
