using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Tests.Utils;
using NRustLightning.Utils;
using NRustLightning.Handles;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
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
        private IBroadcaster _broadcaster;

        private BroadcasterWrapper(BroadcasterWrapperHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public static BroadcasterWrapper Create(IBroadcaster broadcaster)
        {
            Interop.create_broadcaster_wrapper(ref broadcaster.BroadcastTransaction, out var handle);
            return new BroadcasterWrapper(handle);
        }

        public void Broadcast()
        {
            Interop.test_broadcaster_wrapper(_handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
    
}
