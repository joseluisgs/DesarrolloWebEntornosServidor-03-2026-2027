using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductosWeb.Shared.Extensions;

/// <summary>
/// Puente entre el resultado de FluentValidation y el ModelState de ASP.NET Core.
/// El paquete FluentValidation.AspNetCore está sin soporte oficial, así que esta pieza
/// (tres líneas) es la que hace de pegamento, y se ve entera.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Copia cada fallo del resultado al ModelState, con la clave que la vista espera
    /// (el nombre de la propiedad o la clave anidada, como "Etiquetas[0]").
    /// </summary>
    /// <param name="result">Resultado de Validate o ValidateAsync.</param>
    /// <param name="modelState">Diccionario de estado del modelo de la petición.</param>
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
