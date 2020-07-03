using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    internal interface IFeeEstimatorDelegatesHolder
    {
        GetEstSatPer1000Weight getEstSatPer1000Weight { get; }
    }

    /// <summary>
    /// User defined interface for estimating fee.
    /// </summary>
    public interface IFeeEstimator
    {
        uint GetEstSatPer1000Weight(FFIConfirmationTarget confirmationTarget);
    }

    internal class FeeEstimatorDelegatesHolder : IFeeEstimatorDelegatesHolder
    {
        private readonly IFeeEstimator _feeEstimator;
        private readonly GetEstSatPer1000Weight _getEstSatPer1000Weight;

        public FeeEstimatorDelegatesHolder(IFeeEstimator feeEstimator)
        {
            _feeEstimator = feeEstimator ?? throw new ArgumentNullException(nameof(feeEstimator));
            _getEstSatPer1000Weight = confirmationTarget => _feeEstimator.GetEstSatPer1000Weight(confirmationTarget);
        }

        public GetEstSatPer1000Weight getEstSatPer1000Weight => _getEstSatPer1000Weight;
    }
}