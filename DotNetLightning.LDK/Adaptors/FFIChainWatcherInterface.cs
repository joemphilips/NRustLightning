using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void InstallWatchTx(ref FFISha256dHash txid, ref FFIScript spk);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void InstallWatchOutPoint(ref FFIOutPoint spk, ref FFIScript outScript);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WatchAllTxn();
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void GetChainUtxo(ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError error, ref FFITxOut txout);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FilterBlock(ref FFIBlock block);
    
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct FFIChainWatchInterface
    {
        internal InstallWatchTx InstallWatchTx;
        internal InstallWatchOutPoint InstallWatchOutPoint;
        internal WatchAllTxn WatchAllTxn;
        internal GetChainUtxo GetChainUtxo;
        internal FilterBlock FilterBlock;
    }
}