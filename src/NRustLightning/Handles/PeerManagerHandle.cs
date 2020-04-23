using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Handles
{
    public class PeerManagerHandle : SafeHandle
    {
        public PeerManagerHandle() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_peer_manager(h, false);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
}