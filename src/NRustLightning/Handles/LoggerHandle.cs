using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Handles
{
    public class LoggerHandle : SafeHandle
    {
        private LoggerHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_logger(h, false);
            return true;
        }
    }
}