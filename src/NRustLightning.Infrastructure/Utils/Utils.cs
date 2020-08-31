using System.Diagnostics;
using System.Runtime.CompilerServices;
using NRustLightning.Infrastructure.Models.Response;

namespace NRustLightning.Infrastructure.Utils
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static NRustLightningException Fail(string msg)
        {
            Debug.Fail(msg);
            throw new NRustLightningException("Assertion failed: " + msg);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Fail<T>(string msg) where T : class
        {
            throw Fail(msg);
        }
    }
}