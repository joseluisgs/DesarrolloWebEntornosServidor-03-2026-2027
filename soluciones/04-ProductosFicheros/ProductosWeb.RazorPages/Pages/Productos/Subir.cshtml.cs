using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Services.Storage;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página de subida de un fichero suelto, sin producto detrás.
/// OnPostFileAsync es el equivalente del POST Subir del controlador MVC: el fichero llega
/// como IFormFile y la decisión la toma el servicio de almacenamiento.
/// </summary>
/// <param name="almacen">Almacenamiento de los ficheros subidos.</param>
public class SubirModel(IStorageService almacen) : PageModel
{
    /// <summary>
    /// Formulario de subida. El campo de fichero no va en un modelo: se recibe en el handler.
    /// </summary>
    public void OnGet()
    {
        // Nada que preparar: la página solo pinta el input type=file.
    }

    /// <summary>
    /// Recibe el fichero y lo delega en el almacenamiento.
    /// Si el servicio lo rechaza, se devuelve la página con el motivo en el resumen;
    /// si lo guarda, se redirige con la ruta pública en TempData.
    /// </summary>
    /// <param name="fichero">Fichero elegido en el formulario; null si no se eligió ninguno.</param>
    /// <returns>Redirect al catálogo si se guardó; la propia página con el error si no.</returns>
    public async Task<IActionResult> OnPostAsync(IFormFile? fichero)
    {
        var resultado = await almacen.SaveFileAsync(fichero, "productos");

        if (!resultado.Ok)
        {
            // Clave vacía: el resumen de la página pinta los errores de esa clave.
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return Page();
        }

        TempData["Exito"] = $"Fichero guardado en {resultado.Ruta}";
        return RedirectToPage("/Productos/Index");
    }
}
