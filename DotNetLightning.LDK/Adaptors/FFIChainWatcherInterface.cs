using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallWatchTx(ref FFISha256dHash txid, ref FFIScript spk);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallWatchOutPoint(ref FFIOutPoint spk, ref FFIScript outScript);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WatchAllTxn();
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetChainUtxo(ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError error, ref FFITxOut txout);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FilterBlock(ref FFIBlock block);
    
}