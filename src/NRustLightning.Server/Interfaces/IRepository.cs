using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Server.Entities;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IRepository
    {
        
        Task<PaymentRequest> GetNewInvoice(
            NRustLightningNetwork network,
            InvoiceCreationOption option);
        
        Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default);
        Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default);

        Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default);

        Task SetInvoice(PaymentRequest paymentRequest, CancellationToken ct = default);

        Task SetChannelManager(ChannelManager channelManager, CancellationToken ct = default);

        Task<ChannelManager?> GetChannelManager(ChannelManagerReadArgs readArgs, CancellationToken ct = default);
    }
}