using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
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
        GetEstSatPer1000Weight getEstSatPer1000Weight { get; }
    }

    public abstract class FeeEstimator : IFeeEstimator
    {

        protected abstract ulong GetEstSatPer1000WeightImpl(FFIConfirmationTarget confirmationTarget);

        public GetEstSatPer1000Weight getEstSatPer1000Weight => GetEstSatPer1000WeightImpl;
    }
}