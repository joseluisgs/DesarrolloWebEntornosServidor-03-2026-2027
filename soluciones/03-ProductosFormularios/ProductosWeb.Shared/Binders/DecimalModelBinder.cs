using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductosWeb.Shared.Binders;

/// <summary>
/// Model binding propio para decimales.
/// En un navegador español el precio llega como "1.299,00" y en uno inglés como "1,299.00":
/// este binder acepta las dos formas y siempre guarda el número con InvariantCulture,
/// que es el que entiende SQLite. La TiendaWeb usa exactamente el mismo.
/// </summary>
public class DecimalModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        // Se guarda en el ModelState el valor tal y como llegó, para poder repintar el formulario.
        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.CompletedTask;
        }

        // Si el valor trae coma, se asume separador decimal español y se normaliza a punto.
        if (value.Contains(','))
        {
            value = value.Replace(',', '.');
        }

        if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var decimalValue))
        {
            bindingContext.Result = ModelBindingResult.Success(decimalValue);
        }
        else
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                $"'{valueProviderResult.FirstValue}' no es un número válido");
        }

        return Task.CompletedTask;
    }
}
