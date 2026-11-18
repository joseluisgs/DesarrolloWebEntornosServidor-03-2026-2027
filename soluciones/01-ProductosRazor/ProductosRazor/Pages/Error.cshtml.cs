using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProductosRazor.Pages;

/// <summary>
/// La página de error del sistema UseExceptionHandler.
/// Es el equivalente en Razor Pages a la acción Error del controlador del ejemplo 02:
/// la sirve el middleware cuando algo revienta fuera de Development.
/// </summary>
public class ErrorModel : PageModel
{
    /// <summary>URL que produjo el error, si la hubo.</summary>
    public string? Ruta { get; private set; }

    /// <summary>
    /// Mensaje de la excepción. En producción conviene no mostrarlo al usuario:
    /// aquí solo aparece porque la app corre en Development.
    /// </summary>
    public string? Mensaje { get; private set; }

    /// <summary>
    /// Handler del GET.
    /// </summary>
    public void OnGet()
    {
        var detalle = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        Ruta = detalle?.Path;
        Mensaje = detalle?.Error.Message;
    }
}
