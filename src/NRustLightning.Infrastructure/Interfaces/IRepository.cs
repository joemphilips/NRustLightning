using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Infrastructure.Interfaces
{
    public interface IRepository
    {
        
        
        Task<Primitives.PaymentPreimage?> GetPreimage(Primitives.PaymentHash hash, CancellationToken ct = default);
        Task SetPreimage(Primitives.PaymentPreimage paymentPreimage, CancellationToken ct = default);

        Task<PaymentRequest?> GetInvoice(Primitives.PaymentHash hash, CancellationToken ct = default);

        Task SetInvoice(PaymentRequest paymentRequest, CancellationToken ct = default);

        Task SetChannelManager(ChannelManager channelManager, CancellationToken ct = default);

        Task<ChannelManager?> GetChannelManager(ChannelManagerReadArgs readArgs, CancellationToken ct = default);
    }
}