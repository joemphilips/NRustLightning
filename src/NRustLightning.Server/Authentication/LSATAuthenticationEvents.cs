using System;
using System.Threading.Tasks;

namespace NRustLightning.Server.Authentication
{
    public class LSATAuthenticationEvents
    {
        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = ctx => Task.CompletedTask;
        
        
        /// <summary>
        /// Invoked when validation succeeds
        /// </summary>
        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = ctx => Task.CompletedTask;
        
        /// <summary>
        /// Invoked when we sent 402 message to the client.
        /// </summary>
        public Func<PaymentRequiredContext, Task> OnPaymentRequired { get; set; } = ctx => Task.CompletedTask;

        public virtual Task MessageReceived(MessageReceivedContext ctx) => OnMessageReceived(ctx);
        public virtual Task TokenValidated(TokenValidatedContext ctx) => OnTokenValidated(ctx);
        public virtual Task PaymentRequired(PaymentRequiredContext ctx) => OnPaymentRequired(ctx);
    }
}