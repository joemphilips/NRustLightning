using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Handles
{
    public class BroadcasterHandle : SafeHandle
    {
        private BroadcasterHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_broadcaster(h, false);
            return true;
        }
        
    }
}