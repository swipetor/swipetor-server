using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebLibServer.SharedLogic.Fx;

namespace SwipetorApp.System.Binders;

/// <summary>
///     Binds Currency object to ASP.NET Core MVC models
/// </summary>
public class CurrencyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None)
        {
            var currencyValue = new Currency(valueProviderResult.FirstValue);
            bindingContext.Result = ModelBindingResult.Success(currencyValue);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        return valueProviderResult != ValueProviderResult.None ? new Currency(valueProviderResult.FirstValue) : null;
    }
}