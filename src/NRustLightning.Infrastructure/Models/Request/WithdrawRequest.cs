namespace NRustLightning.Infrastructure.Models.Request
{
    public class WithdrawRequest
    {
        public ulong AmountSatoshi { get; set; }
        public string DestinationAddress { get; set; }
    }
}