using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Handles
{
    public class ManyChannelMonitorHandle : SafeHandle
    {
        
        public ManyChannelMonitorHandle() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_many_channel_monitor(h, false);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
}