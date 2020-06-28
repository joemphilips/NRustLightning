using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
{
    internal static partial class Interop
    {

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_block_notifier",
            ExactSpelling = true)]
        static extern FFIResult _create_block_notifier(
            in Network network,
            ref Log log,
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref ReEntered reEntered,
            out BlockNotifierHandle handle
            );

        internal static FFIResult create_block_notifier(
            in Network network,
            Log log,
            InstallWatchTx installWatchTx,
            InstallWatchOutPoint installWatchOutPoint,
            WatchAllTxn watchAllTxn,
            GetChainUtxo getChainUtxo,
            FilterBlock filterBlock,
            ReEntered reEntered,
            out BlockNotifierHandle handle,
            bool check = true
        ) => MaybeCheck(_create_block_notifier(in network, ref log, ref installWatchTx, ref installWatchOutPoint, ref watchAllTxn, ref getChainUtxo, ref filterBlock, ref reEntered, out handle), check);

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "register_channel_manager",
            ExactSpelling = true)]
        static extern FFIResult _register_channel_manager(
            ChannelManagerHandle channelManagerHandle,
            BlockNotifierHandle handle
            );

        internal static FFIResult register_channel_manager(
            ChannelManagerHandle channelManagerHandle,
            BlockNotifierHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_register_channel_manager(channelManagerHandle, handle), check);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "unregister_channel_manager",
            ExactSpelling = true)]
        static extern FFIResult _unregister_channel_manager(
            ChannelManagerHandle channelManagerHandle,
            BlockNotifierHandle handle
            );
        
        internal static FFIResult unregister_channel_manager(
            ChannelManagerHandle channelManagerHandle,
            BlockNotifierHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_unregister_channel_manager(channelManagerHandle, handle), check);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "block_connected",
            ExactSpelling = true)]
        static extern unsafe FFIResult _block_connected(byte* blockPtr, UIntPtr blockLength, uint height, BlockNotifierHandle handle);

        internal static unsafe FFIResult block_connected(
            byte* blockPtr, UIntPtr blockLength, uint height, BlockNotifierHandle handle, bool check = true
        ) => MaybeCheck(_block_connected(blockPtr, blockLength, height, handle), check);
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "block_disconnected",
            ExactSpelling = true)]
        static extern unsafe FFIResult _block_disconnected(byte* blockHeaderPtr, UIntPtr blockHeaderLength, uint height, BlockNotifierHandle handle);

        internal static unsafe FFIResult block_disconnected(
            byte* blockHeaderPtr, UIntPtr blockHeaderLength, uint height, BlockNotifierHandle handle, bool check = true
        ) => MaybeCheck(_block_disconnected(blockHeaderPtr, blockHeaderLength, height, handle), check);

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_block_notifier",
            ExactSpelling = true)]
        static extern FFIResult _release_block_notifier(IntPtr handle);

        internal static FFIResult release_block_notifier(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_block_notifier(handle), check);
        }
    }
}