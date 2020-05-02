using System.Runtime.InteropServices;
using Bool = System.Byte;

namespace NRustLightning.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Bool HandleNodeAnnouncement(ref FFINodeAnnoucement nodeAnnouncement, ref FFILightningError error);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Bool HandleChannelAnnouncement(ref FFIBytes channelAnnouncement, ref FFILightningError error);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Bool HandleChannelUpdate(ref FFIBytes channelUpdate, ref FFILightningError error);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HandleHTLCFailChannelUpdate(ref FFIBytes msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetNextChannelAnnouncements(ulong startingPoint, byte batchAmount, out FFIBytes channelAnnouncement);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetNextNodeAnnouncements(out FFIPublicKey maybeStartingPoint, byte batchAmount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Bool ShouldRequestFullSync(out FFIPublicKey nodeId);
}
