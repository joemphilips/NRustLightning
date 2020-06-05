using System.Threading.Tasks;
using DotNetLightning.Payment;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Server.Authentication
{
    public interface ILSATInvoiceProvider
    {
        Task<PaymentRequest> GetNewInvoiceAsync(InvoiceCreationOption option);
    }
}