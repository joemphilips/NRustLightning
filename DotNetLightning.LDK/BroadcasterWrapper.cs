using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK
{
    #if DEBUG
    /// <summary>
    /// Object for testing behavior of broadcaster when it is held in
    /// Another SafeHandle.
    /// In real circumstances this will be `ChannelManger` or `ChannelMonitor`,
    /// but those are too big. We want this to ensure that nested SafeHandle works.
    /// </summary>
    public class BroadcasterWrapper
    {
        private readonly BroadcasterWrapperHandle _handle;

        private BroadcasterWrapper(BroadcasterWrapperHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public static BroadcasterWrapper Create()
        {
            //Interop.create_broadcaster_wrapper(out var handle;
            //return handle;
            return null;
        }
    }

    public class BroadcasterWrapperHandle : SafeHandle
    {
        
        private BroadcasterWrapperHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            Interop.release_broadcaster_wrapper(h, false);
            return true;
        }
    }
    #endif
}