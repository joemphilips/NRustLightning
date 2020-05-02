using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    public enum FFIErrorActionType : byte
    {
        DisconnectPeer = 0,
        IgnoreError,
        SendErrorMessage,
    }

    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FFIErrorMsg
    {
        public fixed byte ChannelId[32];
        private IntPtr data;
        
        public string Data => Marshal.PtrToStringUTF8(data);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFIErrorAction
    {
        public readonly FFIErrorActionType Type;
        private readonly IntPtr msg;
        public FFIErrorMsg? Msg => msg == IntPtr.Zero ? (FFIErrorMsg?) null : Marshal.PtrToStructure<FFIErrorMsg>(msg);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct FFILightningError
    {
        private readonly IntPtr errMsg;
        public readonly FFIErrorAction Action;
        public string ErrorMsg => Marshal.PtrToStringUTF8(errMsg);
    }
}