using DotNetLightning.Payment;

namespace NRustLightning.Infrastructure.Entities
{
    public enum InvoiceStatus
    {
        New = 0,
        PartialPayed,
        Expired,
        Complete,
        ExpiredAfterPartiallyPayed,
        CompleteWithOverpay
    }
    public class Invoice
    {
        public PaymentRequest Bolt11Invoice { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}