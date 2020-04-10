using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_chain_watch_interface", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _create_chain_watch_interface(
            ref InstallWatchTx fn1,
            ref InstallWatchOutPoint fn2,
            ref WatchAllTxn fn3,
            ref GetChainUtxo fn4,
            ref FilterBlock fn5,
            out ChainWatchInterfaceHandle handle);

        internal static FFIResult create_chain_watch_interface(
            ref InstallWatchTx fn1,
            ref InstallWatchOutPoint fn2,
            ref WatchAllTxn fn3,
            ref GetChainUtxo fn4,
            ref FilterBlock fn5,
            out ChainWatchInterfaceHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_create_chain_watch_interface(ref fn1, ref fn2, ref fn3, ref fn4, ref fn5, out handle), check);
        }

        [DllImport(RustLightning, EntryPoint = "release_chain_watch_interface", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _release_chain_watch_interface(IntPtr handle);

        internal static FFIResult release_chain_watch_interface(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_chain_watch_interface(handle), check);
        }
        
    }
}