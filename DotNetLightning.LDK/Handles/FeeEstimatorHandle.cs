using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Handles
{
    public class FeeEstimatorHandle : SafeHandle
    {
        private FeeEstimatorHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_fee_estimator(h, false);
            return true;
        }
    }
}