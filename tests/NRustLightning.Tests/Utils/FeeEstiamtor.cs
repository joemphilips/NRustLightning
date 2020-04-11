using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;

namespace DotNetLightning.LDK.Tests.Utils
{
    internal class TestFeeEstimator : IFeeEstimator
    {
        private static FFIGetEstSatPer1000Weight _estimate = (ref FFITransaction tx) => 1000;
        public ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight => ref _estimate;
    }
}