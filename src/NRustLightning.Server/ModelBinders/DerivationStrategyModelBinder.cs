using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBXplorer.DerivationStrategy;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Server.ModelBinders
{
    public class DerivationStrategyModelBinder: IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(DerivationStrategyBase).GetTypeInfo().IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult? val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string key = val?.FirstValue;
            if (key == null)
            {
                return Task.CompletedTask;
            }

            var networkProvider =
                (NRustLightningNetworkProvider) bindingContext.HttpContext.RequestServices.GetService(
                    typeof(NRustLightningNetworkProvider));

            string? cryptoCode = bindingContext.ValueProvider.GetValue("cryptoCode").FirstValue;
            cryptoCode ??= bindingContext.ValueProvider.GetValue("network").FirstValue;
            var network = networkProvider.GetByCryptoCode(cryptoCode ?? "BTC");
            try
            {
                var data = network.NbXplorerNetwork.DerivationStrategyFactory.Parse(key);
                if (!bindingContext.ModelType.IsInstanceOfType(data))
                {
                    throw new FormatException("Invalid destination type");
                }

                bindingContext.Result = ModelBindingResult.Success(data);
            }
            catch
            {
                throw new FormatException("Invalid derivation scheme");
            }

            return Task.CompletedTask;
        }
    }
}