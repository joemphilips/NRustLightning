using System;
using System.Buffers;
using System.IO;
using System.Linq;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Tests.Utils;
using NRustLightning.Utils;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace NRustLightning.Tests
{
    public class BlockNotifierTests
    {
        private MemoryPool<byte> _pool;
        public BlockNotifierTests()
        {
            _pool = MemoryPool<byte>.Shared;
        }
        
        [Fact]
        public void RegistrationTests()
        {
            var keySeed = new byte[]{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
            var keysInterface = new KeysManager(keySeed, DateTime.UnixEpoch);
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var n = NBitcoin.Network.TestNet;
            var chainWatchInterface = new ChainWatchInterfaceUtil(n);

            using var blockNotifier = BlockNotifier.Create(chainWatchInterface);
            using var manyChannelMonitor =
                ManyChannelMonitor.Create(n, chainWatchInterface, broadcaster, logger, feeEstiamtor);
            
            blockNotifier.RegisterManyChannelMonitor(manyChannelMonitor);

            using var channelManager = ChannelManager.Create(n, UserConfig.GetDefault(), chainWatchInterface, keysInterface, logger, broadcaster, feeEstiamtor, 0, manyChannelMonitor);
            blockNotifier.RegisterChannelManager(channelManager);
            
            // second block in testnet3
            var block = (Block.Parse("0100000006128e87be8b1b4dea47a7247d5528d2702c96826c7a648497e773b800000000e241352e3bec0a95a6217e10c3abb54adfa05abb12c126695595580fb92e222032e7494dffff001d00d235340101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff0e0432e7494d010e062f503253482fffffffff0100f2052a010000002321038a7f6ef1c8ca0c588aa53fa860128077c9e6c11e6830f4d7ee4e763a56b7718fac00000000", n));
            blockNotifier.BlockConnected(block, 1);

            var b = manyChannelMonitor.Serialize(_pool);
            var (manyChannelMonitor2, keyToHeaderHash) = ManyChannelMonitor.Deserialize(n, chainWatchInterface, broadcaster, logger, feeEstiamtor, b.AsMemory(), _pool);
            using (manyChannelMonitor2)
            {
                Assert.True(NBitcoin.Utils.ArrayEqual(b, manyChannelMonitor2.Serialize(_pool)));
                // without creating any channel, it will result to empty.
                Assert.Empty(keyToHeaderHash);
            }

            blockNotifier.UnregisterManyChannelMonitor(manyChannelMonitor);
            blockNotifier.UnregisterChannelManager(channelManager);
        }
    }
}