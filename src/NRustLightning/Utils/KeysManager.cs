using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NRustLightning.Interfaces;
using NRustLightning.RustLightningTypes;
using RustLightningTypes;

namespace NRustLightning.Utils
{
    
    /// <summary>
    /// Equivalent to the class with the same name in rust-lightning.
    /// </summary>
    public class KeysManager : IKeysInterface
    {
        private readonly byte[] _seed;
        private readonly ulong _startingTimeSecs;
        private readonly ulong _startingTime100Nanos;
        private readonly Key _nodeSecret;
        private readonly Script _destinationScript;
        private readonly PubKey _shutdownPubKey;
        
        private readonly ExtKey _channelMasterKey;
        private int _channelChildIndex;
        
        private readonly ExtKey _sessionMasterKey;
        private int _sessionChildIndex;
        
        private readonly ExtKey _channelIdMasterKey;
        private int _channelIdChildIndex;
        private DataEncoder _ascii;

        public KeysManager(byte[] seed, DateTime startingTime)
        {
            _seed = seed ?? throw new ArgumentNullException(nameof(seed));
            _startingTime100Nanos = (ulong)startingTime.Ticks;
            _startingTimeSecs = (ulong)startingTime.Second;
            if (seed.Length != 32)
            {
                throw new ArgumentException($"seed must be length 32. it was {seed.Length}");
            }
            var master = new ExtKey(seed);
            _nodeSecret = master.Derive(0).PrivateKey;
            var destinationKey = master.Derive(1).PrivateKey;
            _destinationScript = destinationKey.PubKey.WitHash.ScriptPubKey;
            _shutdownPubKey = master.Derive(2).PrivateKey.PubKey;
            _channelMasterKey = master.Derive(3);
            _sessionMasterKey = master.Derive(4);
            _channelIdMasterKey = master.Derive(5);
            _ascii = new ASCIIEncoder();
        }


        public ChannelKeys DeriveChannelKeys(ulong channelValueSatoshis, ulong param1, ulong param2)
        {
            var channelId = (uint)((param1 & 0xFFFF_FFFF_0000_0000) >> 32);
            var childPrivKey = _channelMasterKey.Derive(channelId);
            var uniqueStart =
                param2.GetBytesBigEndian()
                    .Concat(((uint) param1).GetBytesBigEndian())
                    .Concat(_seed)
                    .Concat(childPrivKey.PrivateKey.ToBytes());
            var seed = Hashes.SHA256(uniqueStart.ToArray());
            var commitmentSeed = Hashes.SHA256(seed.Concat(_ascii.DecodeData("commitment seed")).ToArray());

            var fundingKey = Hashes.SHA256(commitmentSeed.Concat(_ascii.DecodeData("funding key")).ToArray());
            var revocationBaseKey = Hashes.SHA256(fundingKey.Concat(_ascii.DecodeData("revocation base key")).ToArray());
            var paymentKey = Hashes.SHA256(revocationBaseKey.Concat(_ascii.DecodeData("payment key")).ToArray());
            var delayedPaymentBaseKey = Hashes.SHA256(paymentKey.Concat(_ascii.DecodeData("delayed payment base key")).ToArray());
            var htlcBaseKey = Hashes.SHA256(delayedPaymentBaseKey.Concat(_ascii.DecodeData("HTLC base key")).ToArray());
            return new ChannelKeys(new Key(fundingKey), new Key(revocationBaseKey),new Key(paymentKey), new Key(delayedPaymentBaseKey), new Key(htlcBaseKey), new uint256(commitmentSeed), channelValueSatoshis, param1, param2);
        }

        private IEnumerable<byte> DeriveUniqueStart() => 
            _startingTimeSecs.GetBytesBigEndian()
            .Concat(_startingTime100Nanos.GetBytesBigEndian());
        public Key GetNodeSecret()
        {
            return this._nodeSecret;
        }

        public Script GetDestinationScript() => _destinationScript;

        public PubKey GetShutdownKey() => _shutdownPubKey;

        public ChannelKeys GetChannelKeys(bool inbound, ulong channelValueSatoshis)
        {
            var childIdx = Interlocked.Increment(ref _channelChildIndex);
            var index_and_nanos = ((ulong)childIdx) << 32 | _startingTime100Nanos;
            return DeriveChannelKeys(channelValueSatoshis, index_and_nanos, _startingTime100Nanos);
        }

        public Tuple<Key, uint256> GetOnionRand()
        {
            var salt = DeriveUniqueStart();
            var childIdx = Interlocked.Increment(ref _sessionChildIndex);
            var childPrivKey = _sessionMasterKey.Derive((uint)childIdx);

            var sha = salt.Concat(childPrivKey.PrivateKey.ToBytes()).Concat(_ascii.DecodeData("Session Key Salt"));
            return Tuple.Create(new Key(Hashes.SHA256(sha.ToArray())), new uint256(Hashes.SHA256(_ascii.DecodeData("RNG Seed Salt"))));
        }

        public uint256 GetChannelId()
        {
            var childIdx = Interlocked.Increment(ref _channelIdChildIndex);
            var childPrivKey = _channelIdMasterKey.Derive((uint)childIdx);
            return new uint256(Hashes.SHA256(childPrivKey.ToBytes()));
        }
    }
}