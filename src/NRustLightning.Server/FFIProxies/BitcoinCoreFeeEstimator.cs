using NBitcoin.RPC;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Extensions;

namespace NRustLightning.Server.FFIProxies
{
    /// <summary>
    ///  TODO: use cache
    /// </summary>
    public class BitcoinCoreFeeEstimator : IFeeEstimator
    {
        private readonly RPCClient rpc;
        private GetEstSatPer1000Weight _getEstSatPer1000Weight;
        public BitcoinCoreFeeEstimator(RPCClient rpc)
        {
            this.rpc = rpc;
            _getEstSatPer1000Weight = (ref FFITransaction tx) =>
            {
                var feeRate = rpc.EstimateSmartFee(6, EstimateSmartFeeMode.Economical).FeeRate;
                var h = feeRate.GetFee(tx.AsTransaction(rpc.Network));
                return h;
            };
        }

        public ref GetEstSatPer1000Weight getEstSatPer1000Weight => ref _getEstSatPer1000Weight;
    }
}