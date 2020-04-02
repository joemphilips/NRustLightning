using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Tests.Utils;
using DotNetLightning.LDK.Utils;

namespace DotNetLightning.LDK.Tests.Utils
{
    /// <summary>
    /// Object for testing behavior of broadcaster when it is held in
    /// Another SafeHandle.
    /// In real circumstances this will be `ChannelManger` or `ChannelMonitor`,
    /// but those are too big. We want this to ensure that nested SafeHandle works.
    /// </summary>
    public class BroadcasterWrapper : IDisposable
    {
        private readonly BroadcasterWrapperHandle _handle;
        private readonly BroadcasterHandle _innerHandle;

        private BroadcasterWrapper(BroadcasterWrapperHandle handle, BroadcasterHandle innerHandle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
            _innerHandle = innerHandle ?? throw new ArgumentNullException(nameof(innerHandle));
        }

        private static FFIBroadcastTransaction broadcast_ptr = (ref FFITransaction tx) =>
        {
            Console.WriteLine($"going to broadcast tx {Hex.Encode(tx.AsSpan())} from wrapper.");
        };


        public static BroadcasterWrapper Create()
        {
            Interop.create_broadcaster(ref broadcast_ptr, out var innerHandle);
            Interop.create_broadcaster_wrapper(innerHandle, out var handle);
            return new BroadcasterWrapper(handle, innerHandle);
        }

        public void Broadcast()
        {
            Interop.test_broadcaster_wrapper(_handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
            _innerHandle.Dispose();
        }
    }
    
}
