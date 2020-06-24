using NRustLightning.RustLightningTypes;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace NRustLightning.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void EventSerialization()
        {
            var fundingGenerationReady =
                "0001009090dd9b9ad53747b935f96e27e2e4323e94cb629a0b44a92f90e728ec36b00300000000000186a0002200209cd7c21244f9d6a8e0768218340a3ec92dd2fc8cba5577e966f4b0b368b62809d599d1ed70dc7524";
            var e = Event.ParseManyUnsafe(Hex.Decode(fundingGenerationReady));
            Assert.Single(e);
            Assert.True(e[0].IsFundingGenerationReady);
        }
    }
}