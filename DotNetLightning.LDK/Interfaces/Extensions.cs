using System;
using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK.Interfaces
{
    internal static class Extensions
    {
        internal static FFILogger ToFFI(this ILogger logger)
        {
            throw new NotImplementedException();
        }
        internal static FFIFeeEstimator ToFFI(this IFeeEstimator logger)
        {
            throw new NotImplementedException();
        }
        internal static FFIBroadcaster ToFFI(this IBroadcaster logger)
        {
            throw new NotImplementedException();
        }
    }
}