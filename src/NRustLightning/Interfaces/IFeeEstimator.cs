using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK.Interfaces
{
    /// <summary>
    /// User defined interface for estimating fee.
    /// Important: It has to hold the delegate as a static field.
    /// Otherwise the delegate passed to the rust side becomes null pointer and crashes.
    ///
    /// keep in mind that the static delegate defined in this class may call the
    /// delegate from multiple threads at the same time, so it must make thread safe,
    /// If you want to hold mutable state in it.
    /// </summary>
    public interface IFeeEstimator
    {
        ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight { get; }
    }
}