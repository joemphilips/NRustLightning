using System;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using Extensions = NRustLightning.Adaptors.Extensions;

namespace NRustLightning
{
    public class BlockNotifier : IDisposable
    {
        private readonly BlockNotifierHandle _handle;
        private ManyChannelMonitor? _manyChannelMonitor;
        private ChannelManager? _channelManager;
        private readonly object[] _deps;
        private bool _disposed = false;
        
        internal BlockNotifier(
            BlockNotifierHandle handle,
            object[] deps
            )
        {
            _handle = handle;
            _deps = deps;
        }
        public static BlockNotifier Create(
            IChainWatchInterface chainWatchInterface)
        {
            if (chainWatchInterface == null) throw new ArgumentNullException(nameof(chainWatchInterface));
            var chainWatchInterfaceDelegatesHolder = new ChainWatchInterfaceConverter(chainWatchInterface);
            return Create(chainWatchInterfaceDelegatesHolder);
        }

        internal static BlockNotifier Create(IChainWatchInterfaceDelegatesHolder chainWatchInterfaceDelegatesHolder)
        {
            Interop.create_block_notifier(chainWatchInterfaceDelegatesHolder.InstallWatchTx, chainWatchInterfaceDelegatesHolder.InstallWatchOutPoint, chainWatchInterfaceDelegatesHolder.WatchAllTxn, chainWatchInterfaceDelegatesHolder.GetChainUtxo, chainWatchInterfaceDelegatesHolder.FilterBlock ,chainWatchInterfaceDelegatesHolder.ReEntered, out var handle);
            return new BlockNotifier(handle, new object[]{ chainWatchInterfaceDelegatesHolder });
        }

        public void RegisterChannelManager(ChannelManager channelManager)
        {
            if (channelManager == null) throw new ArgumentNullException(nameof(channelManager));
            var h = channelManager.Handle;
            Interop.register_channel_manager(h, _handle);
            _channelManager = channelManager;
        }

        public void UnregisterChannelManager(ChannelManager channelManager)
        {
            // This may cause crash. abandon for now.
            throw new NotImplementedException();
            if (channelManager == null) throw new ArgumentNullException(nameof(channelManager));
            var h = channelManager.Handle;
            Interop.unregister_channel_manager(h, _handle);
            _channelManager = null;
        }

        public void RegisterManyChannelMonitor(ManyChannelMonitor manyChannelMonitor)
        {
            // This may cause crash. abandon for now.
            throw new NotImplementedException();
            if (manyChannelMonitor == null) throw new ArgumentNullException(nameof(manyChannelMonitor));
            var h = manyChannelMonitor.Handle;
            Interop.register_many_channel_monitor(h, _handle);
            _manyChannelMonitor = manyChannelMonitor;
        }

        public void UnregisterManyChannelMonitor(ManyChannelMonitor manyChannelMonitor)
        {
            if (manyChannelMonitor == null) throw new ArgumentNullException(nameof(manyChannelMonitor));
            var h = manyChannelMonitor.Handle;
            Interop.unregister_many_channel_monitor(h, _handle);
            _manyChannelMonitor = null;
        }

        public unsafe void BlockConnected(Block block, uint height)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            var blockBytes = block.ToBytes();
            fixed (byte* b = blockBytes)
            {
                Interop.block_connected(b, (UIntPtr)blockBytes.Length, height, _handle);
            }
        }

        public unsafe void BlockDisconnected(BlockHeader blockHeader, uint height)
        {
            if (blockHeader == null) throw new ArgumentNullException(nameof(blockHeader));
            var blockHeaderBytes = blockHeader.ToBytes();
            fixed (byte* b = blockHeaderBytes)
            {
                Interop.block_disconnected(b, (UIntPtr)blockHeaderBytes.Length, height, _handle);
            }
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _channelManager?.Dispose();
                _manyChannelMonitor?.Dispose();
                _handle.Dispose();
            }
        }
    }
}