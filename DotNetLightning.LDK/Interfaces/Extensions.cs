using System;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK.Interfaces
{
    internal static class Extensions
    {
        internal static FFIManyChannelMonitor ToFFI(this IManyChannelMonitor manyChannelMonitor)
        {
            throw new NotImplementedException();
        }
        
        internal static NullFFILogger ToFFI(this ILogger logger)
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