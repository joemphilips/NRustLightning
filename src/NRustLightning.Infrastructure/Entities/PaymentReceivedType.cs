namespace NRustLightning.Infrastructure.Entities
{
    public enum PaymentReceivedType
    {
        Ok = 0,
        UnknownPaymentHash,
        AmountTooLow,
        AmountTooHigh,
    }

}