using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebAppShared.SharedLogic.Fx;

namespace SwipetorApp.System.Binders;

/// <summary>
///     Provides CurrencyModelBinder for registering in Startup.cs
/// </summary>
public class CurrencyModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (context.Metadata.ModelType == typeof(Currency))
            return new CurrencyModelBinder();

        return null;
    }
}