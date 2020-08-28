using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NRustLightning.Infrastructure.Models.Request
{
    public class WithdrawRequest
    {
        [DefaultValue(0)]
        public ulong AmountSatoshi { get; set; }
        [Required]
        public string DestinationAddress { get; set; }
    }
}