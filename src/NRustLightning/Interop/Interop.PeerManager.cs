using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_peer_manager", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe FFIResult _create_peer_manager(
            byte* seedPtr,
            UIntPtr seedLen,
            Network* n, 
            UserConfig* userConfig,
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            UIntPtr currentBlockHeight,
            
            ref HandleNodeAnnouncement handleNodeAnnouncement,
            ref HandleChannelAnnouncement handleChannelAnnouncement,
            ref HandleChannelUpdate handleChannelUpdate,
            ref HandleHTLCFailChannelUpdate handleHtlcFailChannelUpdate,
            ref GetNextChannelAnnouncements getNextChannelAnnouncements,
            ref GetNextNodeAnnouncements getNextNodeAnnouncements,
            ref ShouldRequestFullSync shouldRequestFullSync,
            
            ref FFISecretKey ourNodeSecret,
            out PeerManagerHandle handle
            );

        internal static unsafe FFIResult create_peer_manager(
            byte* seedPtr,
            UIntPtr seedLen,
            Network* n, 
            UserConfig* userConfig,
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            UIntPtr currentBlockHeight,
            
            ref HandleNodeAnnouncement handleNodeAnnouncement,
            ref HandleChannelAnnouncement handleChannelAnnouncement,
            ref HandleChannelUpdate handleChannelUpdate,
            ref HandleHTLCFailChannelUpdate handleHtlcFailChannelUpdate,
            ref GetNextChannelAnnouncements getNextChannelAnnouncements,
            ref GetNextNodeAnnouncements getNextNodeAnnouncements,
            ref ShouldRequestFullSync shouldRequestFullSync,
            
            ref FFISecretKey ourNodeSecret,
            out PeerManagerHandle handle,
            bool check = true
            ) =>
            MaybeCheck(_create_peer_manager(
                seedPtr,
                seedLen,
                n,
                userConfig,
                ref installWatchTx,
                ref installWatchOutPoint,
                ref watchAllTxn,
                ref getChainUtxo,
                ref broadcastTransaction,
                ref log,
                ref getEstSatPer1000Weight,
                currentBlockHeight,
                ref handleNodeAnnouncement,
                ref handleChannelAnnouncement,
                ref handleChannelUpdate,
                ref handleHtlcFailChannelUpdate,
                ref getNextChannelAnnouncements,
                ref getNextNodeAnnouncements,
                ref shouldRequestFullSync,
                ref ourNodeSecret,
                out handle),
                check);
        
        [DllImport(RustLightning, EntryPoint = "release_peer_manager", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _release_peer_manager(IntPtr handle);

        internal static FFIResult release_peer_manager(IntPtr handle, bool check = true) =>
            MaybeCheck(_release_peer_manager(handle), check);

        [DllImport(RustLightning, EntryPoint = "timer_tick_occured", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _timer_tick_occured(PeerManagerHandle handle);

        internal static FFIResult timer_tick_occured(PeerManagerHandle handle, bool check = true) => MaybeCheck(_timer_tick_occured(handle), check);
        
        [DllImport(RustLightning, EntryPoint = "new_inbound_connection", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _new_inbound_connection(
            UIntPtr index,
            ref SendData sendData,
            ref DisconnectSocket disconnectSocket,
            PeerManagerHandle handle
            );

        internal static FFIResult new_inbound_connection(
            UIntPtr index,
            ref SendData sendData,
            ref DisconnectSocket disconnectSocket,
            PeerManagerHandle handle,
            bool check = true
        ) => MaybeCheck(_new_inbound_connection(index, ref sendData, ref disconnectSocket, handle), check);
        
        [DllImport(RustLightning, EntryPoint = "new_outbound_connection", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _new_outbound_connection(
            UIntPtr index,
            ref SendData sendData,
            ref DisconnectSocket disconnectSocket,
            ref FFIPublicKey theirNodeId,
            PeerManagerHandle handle
            );

        internal static FFIResult new_outbound_connection(
            UIntPtr index,
            ref SendData sendData,
            ref DisconnectSocket disconnectSocket,
            ref FFIPublicKey theirNodeId,
            PeerManagerHandle handle,
            bool check = true
        ) => MaybeCheck(_new_outbound_connection(index, ref sendData, ref disconnectSocket, ref theirNodeId, handle), check);

        [DllImport(RustLightning, EntryPoint = "write_buffer_space_avail")]
        private static extern FFIResult _write_buffer_space_avail(UIntPtr index, ref SendData sendData, ref DisconnectSocket disconnectSocket, PeerManagerHandle handle);

        internal static FFIResult write_buffer_space_avail(UIntPtr index, ref SendData sendData,
            ref DisconnectSocket disconnectSocket, PeerManagerHandle handle, bool check = true)
            => MaybeCheck(_write_buffer_space_avail(index, ref sendData, ref disconnectSocket, handle), check);
        
        [DllImport(RustLightning, EntryPoint = "read_event")]
        private static extern FFIResult _read_event(UIntPtr index, ref SendData sendData, ref DisconnectSocket disconnectSocket,  ref FFIBytes data, out byte shouldPause, PeerManagerHandle handle);

        internal static FFIResult read_event(UIntPtr index, ref SendData sendData,
            ref DisconnectSocket disconnectSocket, ref FFIBytes data, out byte shouldPause, PeerManagerHandle handle,  bool check = true)
            => MaybeCheck(_read_event(index, ref sendData, ref disconnectSocket, ref data, out shouldPause, handle), check);
        
        [DllImport(RustLightning, EntryPoint = "process_events")]
        private static extern FFIResult _process_events(PeerManagerHandle handle);

        internal static FFIResult process_events(PeerManagerHandle handle, bool check = true)
            => MaybeCheck(_process_events(handle), check);
    }
}