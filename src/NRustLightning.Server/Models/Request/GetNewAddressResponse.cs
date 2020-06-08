using NBitcoin;

namespace NRustLightning.Server.Models.Request
{
    public class GetNewAddressResponse
    {
        public BitcoinAddress Address { get; set; }
    }
}