using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductosWeb.Shared.Binders;

/// <summary>
/// Proveedor del binder de decimales.
/// Se registra en la primera posición de la lista de providers para que gane al SimpleTypeModelBinder
/// de por defecto, que solo sabe leer el punto decimal.
/// </summary>
public class DecimalModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(decimal) ||
            context.Metadata.ModelType == typeof(decimal?))
        {
            return new DecimalModelBinder();
        }

        return null;
    }
}
