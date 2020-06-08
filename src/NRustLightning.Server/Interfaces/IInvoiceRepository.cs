using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Server.Authentication;
using NRustLightning.Server.Entities;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IInvoiceRepository
    {
        Task PaymentStarted(PaymentRequest bolt11);
        Task PaymentFinished(Primitives.PaymentPreimage paymentPreimage);
        
        PaymentRequest GetNewInvoice(
            NRustLightningNetwork network,
            Primitives.PaymentPreimage paymentPreimage,
            InvoiceCreationOption option);
        
        /// <summary>
        /// </summary>
        /// <param name="paymentHash"></param>
        /// <param name="amount"></param>
        /// <param name="secret"></param>
        /// <returns>
        /// Tuple of
        /// 1. How we consider this payment (is it valid? if it is not, why?)
        /// 2. amount we specified in invoice (If we didn't, then just return the amount received)
        /// </returns>
        (PaymentReceivedType, LNMoney) PaymentReceived(Primitives.PaymentHash paymentHash,
            LNMoney amount,
            uint256? secret = null);

        Primitives.PaymentPreimage GetPreimage(Primitives.PaymentHash hash);
    }
}