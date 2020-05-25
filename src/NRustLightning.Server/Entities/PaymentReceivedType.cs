namespace NRustLightning.Server.Entities
{
    public enum PaymentReceivedType
    {
        Ok = 0,
        UnknownPaymentHash,
        AmountTooLow,
        AmountTooHigh,
    }

}