using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Handles
{
    public class ChannelMonitorHandle : SafeHandle
    {
        private ChannelMonitorHandle() : base(IntPtr.Zero, true)
        {}
        public override bool IsInvalid => handle == IntPtr.Zero;
        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_ffi_channel_monitor(h, false);
            return true;
        }
    }
}