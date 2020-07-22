using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NRustLightning.Infrastructure.Models.Request;

namespace NRustLightning.Server.ModelBinders
{
    public class PeerConnectionStringModelBinders : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var key = val.FirstValue as string;
            if (key is null)
                return Task.CompletedTask;

            if (PeerConnectionString.TryCreate(key, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }

            return Task.CompletedTask;
        }
    }
}