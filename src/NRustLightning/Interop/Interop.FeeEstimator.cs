using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_fee_estimator", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _create_fee_estimator(ref FFIGetEstSatPer1000Weight fn, out FeeEstimatorHandle handle);

        internal static FFIResult create_fee_estimator(
            ref FFIGetEstSatPer1000Weight fn,
            out FeeEstimatorHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_create_fee_estimator(ref fn, out handle), check);
        }

        [DllImport(RustLightning, EntryPoint = "release_fee_estimator", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _release_fee_estimator(IntPtr handle);

        internal static FFIResult release_fee_estimator(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_fee_estimator(handle), check);
        }
    }
}