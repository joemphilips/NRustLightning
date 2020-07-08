using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using NBitcoin;
using NRustLightning.Adaptors;
using RustLightningTypes;

namespace NRustLightning.Interfaces
{
    public interface IKeysInterface
    {
        public Key GetNodeSecret();
        public Script GetDestinationScript();
        public PubKey GetShutdownKey();
        public ChannelKeys GetChannelKeys(bool inbound, ulong channelValueSatoshis);
        public Tuple<Key, uint256> GetOnionRand();
        public uint256 GetChannelId();
    }
    
    internal class KeysInterfaceDelegatesHolder
    {
        private readonly IKeysInterface _keysInterface;
        private readonly GetNodeSecret _getNodeSecret;
        private readonly GetDestinationScript _getDestinationScript;
        private readonly GetShutdownKey _getShutdownKey;
        private readonly GetChannelKeys _getChannelKeys;
        private readonly GetOnionRand _getOnionRand;
        private readonly GetChannelId _getChannelId;

        public KeysInterfaceDelegatesHolder(IKeysInterface keysInterface)
        {
            _keysInterface = keysInterface ?? throw new ArgumentNullException(nameof(keysInterface));
            _getNodeSecret = (ref byte nodeSecretPtr) =>
            {
                var b = _keysInterface.GetNodeSecret().ToBytes();
                Debug.Assert(b.Length == 32);
                Unsafe.CopyBlock(ref nodeSecretPtr, ref b[0], 32);
            };
            _getDestinationScript = (ref byte scriptPtr, ref UIntPtr scriptLen) =>
            {
                var b = _keysInterface.GetDestinationScript().ToBytes();
                if (b.Length > 512)
                {
                    throw new InvalidDataException($"Currently, binary length of the script must not be longer than 512. It was {b.Length}");
                }
                unsafe
                {
                    Unsafe.Write(Unsafe.AsPointer(ref scriptLen), (UIntPtr) b.Length);
                }
                Unsafe.CopyBlock(ref scriptPtr, ref b[0], (uint) b.Length);
            };
            _getShutdownKey = (ref byte ptr) =>
            {
                var pk = _keysInterface.GetShutdownKey();
                if (!pk.IsCompressed)
                {
                    pk = pk.Compress();
                }

                var b = pk.ToBytes();
                Unsafe.CopyBlock(ref ptr, ref b[0], 33);
            };

            _getChannelKeys = (byte inbound, ulong satoshis, ref byte channelKeysPtr) =>
            {
                var channelKeysBytes = _keysInterface.GetChannelKeys(inbound == 1, satoshis).ToBytes();
                Debug.Assert(channelKeysBytes.Length == 216);
                Unsafe.CopyBlock(ref channelKeysPtr, ref channelKeysBytes[0], 216);
            };

            _getOnionRand = (ref byte sec, ref byte seed) =>
            {
                var t = _keysInterface.GetOnionRand();
                var (secretBytes, seedBytes) = (t.Item1.ToBytes(), t.Item2.ToBytes());
                Unsafe.CopyBlock(ref sec, ref secretBytes[0], 32);
                Unsafe.CopyBlock(ref seed, ref seedBytes[0], 32);
            };
            _getChannelId = (ref byte id) =>
            {
                var b = _keysInterface.GetChannelId().ToBytes(false);
                Unsafe.CopyBlock(ref id, ref b[0], 32);
            };
        }

        public GetNodeSecret GetNodeSecret => _getNodeSecret;

        public GetDestinationScript GetDestinationScript => _getDestinationScript;

        public GetShutdownKey GetShutdownKey => _getShutdownKey;

        public GetChannelKeys GetChannelKeys => _getChannelKeys;

        public GetOnionRand GetOnionRand => _getOnionRand;

        public GetChannelId GetChannelId => _getChannelId;
    }
}