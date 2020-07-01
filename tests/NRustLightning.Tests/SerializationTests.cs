using System;
using System.Buffers;
using System.Linq;
using System.Reflection.Metadata;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using Microsoft.FSharp.Core;
using NBitcoin;
using Newtonsoft.Json.Serialization;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.RustLightningTypes;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace NRustLightning.Tests
{
    public class SerializationTests
    {
        private MemoryPool<byte> _pool;
        public SerializationTests()
        {
            _pool = MemoryPool<byte>.Shared;
        }
        [Fact]
        public void EventSerialization()
        {
            var fundingGenerationReady =
                "0001009090dd9b9ad53747b935f96e27e2e4323e94cb629a0b44a92f90e728ec36b00300000000000186a0002200209cd7c21244f9d6a8e0768218340a3ec92dd2fc8cba5577e966f4b0b368b62809d599d1ed70dc7524";
            var e = Event.ParseManyUnsafe(Hex.Decode(fundingGenerationReady));
            Assert.Single(e);
            Assert.True(e[0].IsFundingGenerationReady);
        }
        
        public Event[] GetAndClearPendingEvents(MemoryPool<byte> pool)
        {
            Func<IntPtr, UIntPtr, ChannelManagerHandle, (FFIResult, UIntPtr)> func =
                (bufOut, bufLength, _handle) =>
                {
                    var ffiResult = Interop.test_event_serialization(bufOut, bufLength, out var actualLength);
                    return (ffiResult, actualLength);
                };
            var arr = NRustLightning.Utils.Utils.WithVariableLengthReturnBuffer(pool, func, null);
            return Event.ParseManyUnsafe(arr);
        }

        [Fact]
        public void EventSerializationInterop()
        {
            var events = GetAndClearPendingEvents(_pool);
            
            Assert.True(events[0].IsFundingBroadcastSafe);
            var e0 = (Event.FundingBroadcastSafe) events[0];
            var txid = new uint256(Hex.Decode("4141414141414141414141414141414141414141414141414141414141414142"), true);
            Assert.Equal(1u, e0.Item.OutPoint.Item.N);
            Assert.Equal(txid, e0.Item.OutPoint.Item.Hash);
            Assert.Equal(1111U, e0.Item.UserChannelId);

            var e1 = (Event.PaymentReceived)events[1];
            var paymentHash = new uint256(Enumerable.Repeat((byte)2, 32).ToArray());
            var paymentSecret = new uint256(Enumerable.Repeat((byte)3, 32).ToArray());
            Assert.Equal(paymentHash, e1.Item.PaymentHash.Item);
            Assert.Equal(paymentSecret, e1.Item.PaymentSecret.Value);
            Assert.Equal(50000, e1.Item.Amount.MilliSatoshi);

            var e2 = (Event.PaymentSent) events[2];
            var paymentPreimage = Primitives.PaymentPreimage.Create(Enumerable.Repeat((byte)4, 32).ToArray());
            Assert.Equal(paymentPreimage, e2.Item);

            var e3 = (Event.PaymentFailed)events[3];
            paymentHash = new uint256(Enumerable.Repeat((byte)5, 32).ToArray());
            Assert.Equal(paymentHash, e3.Item.PaymentHash.Item);
            Assert.True(e3.Item.RejectedByDest);

            var e4 = (Event.PendingHTLCsForwardable) events[4];
            Assert.Equal(100u, e4.Item.MilliSec);

            var e5 = (Event.SpendableOutputs) events[5];
            Assert.Equal(2, e5.Item.Length);
            
            Assert.True(e5.Item[0].IsStaticOutput);
            var s0 = (SpendableOutputDescriptor.StaticOutput)e5.Item[0];
            txid = new uint256(Hex.Decode("1e8a6ed582813120a85e1dfed1249f1a32f530ba4b3fbabf4047cfbc1faea28c"));
            Assert.Equal(txid, s0.Item.Outpoint.Item.Hash);
            Assert.Equal(1u, s0.Item.Outpoint.Item.N);
            Assert.Equal(s0.Item.Output.Value.Satoshi, 255);
            Assert.Equal(s0.Item.Output.ScriptPubKey, Script.Empty);
            
            Assert.True(e5.Item[1].IsDynamicOutputP2WSH);
            var s1 = (SpendableOutputDescriptor.DynamicOutputP2WSH) e5.Item[1];
            
            Assert.Equal(txid, s1.Item.Outpoint.Item.Hash);
            Assert.Equal(1u, s1.Item.Outpoint.Item.N);

            var perCommitmentPoint = new Primitives.CommitmentPubKey(new PubKey("02aca35d6de21baefaf65db590611fabd42ed4d52683c36caff58761d309314f65"));
            Assert.Equal(perCommitmentPoint, s1.Item.PerCommitmentPoint);
            Assert.Equal((3u, 4u), s1.Item.KeyDerivationParams.ToValueTuple());
            
            var remoteRevocationPubKey = new PubKey("02812cb18bf5c19374b34419095a09aa0b0d5559a24ce0ef558845230b0a096161");
            Assert.Equal(remoteRevocationPubKey, s1.Item.RemoteRevocationPubKey);
            
            Assert.Equal(144, s1.Item.ToSelfDelay);
            Assert.Equal(s1.Item.Output.Value.Satoshi, 255);
            Assert.Equal(s1.Item.Output.ScriptPubKey, Script.Empty);
        }

        private ChannelDetails[] ChannelDetailsInterop()
        {
            Func<IntPtr, UIntPtr, ChannelManagerHandle, (FFIResult, UIntPtr)> func =
                (bufOut, bufLength, _handle) =>
                {
                    var ffiResult = Interop.test_channel_details_serialization(bufOut, bufLength, out var actualLen);
                    return (ffiResult, actualLen);
                };
            var arr = NRustLightning.Utils.Utils.WithVariableLengthReturnBuffer(_pool, func, null);
            return ChannelDetails.ParseManyUnsafe(arr);
        }

        [Fact]
        public void ChannelDetailsSerializationTest()
        {
            var c = ChannelDetailsInterop();
            Assert.Single(c);
            var channelId = new uint256(Hex.Decode("4141414141414141414141414141414141414141414141414141414141414142"), false);
            Assert.Equal( channelId, c[0].ChannelId);
            Assert.Equal(FSharpOption<ulong>.Some(3), c[0].ShortChannelId);
            Assert.Equal(new PubKey("02aca35d6de21baefaf65db590611fabd42ed4d52683c36caff58761d309314f65"), c[0].RemoteNetworkId);
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.OptionDataLossProtect, FSharpOption<FeaturesSupport>.None));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.InitialRoutingSync, FSharpOption<FeaturesSupport>.Some(FeaturesSupport.Optional)));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.OptionUpfrontShutdownScript, FSharpOption<FeaturesSupport>.None));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.VariableLengthOnion, FSharpOption<FeaturesSupport>.None));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.OptionStaticRemoteKey, FSharpOption<FeaturesSupport>.None));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.PaymentSecret, FSharpOption<FeaturesSupport>.None));
            Assert.True(c[0].CounterPartyFeatures.HasFeature(Feature.BasicMultiPartPayment, FSharpOption<FeaturesSupport>.None));

            Assert.Equal(5454U, c[0].ChannelValueSatoshis);
            Assert.Equal(2223U, c[0].UserId);
            Assert.Equal(2828U, c[0].OutboundCapacityMSat);
            Assert.Equal(9292U, c[0].InboundCapacityMSat);
            Assert.False(c[0].IsLive);
        }
    }
}