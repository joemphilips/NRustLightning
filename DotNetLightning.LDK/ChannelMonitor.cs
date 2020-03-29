using System;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    public sealed class ChannelMonitor : IDisposable
    {
        private readonly ChannelMonitorHandle _handle;
        internal ChannelMonitor(ChannelMonitorHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        // when passing delegates from C# to rust, we don't have to pin it but
        // we must
        // 1. declare as a static field.
        // 2. make a copy of it
        // so that we can make sure that GC won't erase the pointer.
        // we e.g. https://stackoverflow.com/questions/5465060/do-i-need-to-pin-an-anonymous-delegate/5465074#5465074
        // and https://stackoverflow.com/questions/29300465/passing-function-pointer-in-c-sharp
        private static FFIBroadcastTransaction broadcast_ptr; 
        public static ChannelMonitor Create()
        {
            // we must make a copy of the delegate here.
            var broadcast = broadcast_ptr;
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}