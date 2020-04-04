using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK.Interfaces
{
    public interface IFeeEstimator
    {
        ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight { get; }
    }
}