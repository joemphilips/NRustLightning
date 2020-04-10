using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Handles
{
    #if DEBUG
    public class BroadcasterWrapperHandle : SafeHandle
    {
        
        private BroadcasterWrapperHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_broadcaster_wrapper(h, false);
            return true;
        }
    #endif
    }
}