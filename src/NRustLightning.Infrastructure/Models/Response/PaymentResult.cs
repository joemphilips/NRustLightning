namespace NRustLightning.Infrastructure.Models.Response
{
    public class PaymentResult
    {
        private enum Kind
        {
            Ok = 0,
            ParameterError,
            PathParameterError,
            AllFailedRetrySafe,
            PartialFailure,
        }

        private readonly Kind _kind;
    }
}