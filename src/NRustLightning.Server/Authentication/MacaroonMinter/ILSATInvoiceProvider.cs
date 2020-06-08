using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;

namespace NRustLightning.Server.Authentication.MacaroonMinter
{
    public interface ILSATInvoiceProvider
    {
        Task<PaymentRequest> GetNewInvoiceAsync(LNMoney price);
    }
}