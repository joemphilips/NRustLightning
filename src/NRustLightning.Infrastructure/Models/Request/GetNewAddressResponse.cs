using NBitcoin;

namespace NRustLightning.Infrastructure.Models.Request
{
    public class GetNewAddressResponse
    {
        public BitcoinAddress Address { get; set; }
    }
}