using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Services.Storage;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Descarga de un fichero de wwwroot.
/// Es una página con un solo handler GET: no pinta nada, entrega el fichero con la cabecera
/// Content-Disposition que abre el diálogo de guardado del navegador.
/// </summary>
/// <param name="almacen">Almacenamiento de los ficheros subidos.</param>
public class DescargarModel(IStorageService almacen) : PageModel
{
    /// <summary>
    /// Entrega el fichero. El nombre que recibe el navegador lo decide el servidor con
    /// <c>fileDownloadName</c>; la ruta que llega por la query se resuelve dentro de wwwroot
    /// y, si escapa de él, no devuelve nada.
    /// </summary>
    /// <param name="ruta">Ruta pública del fichero, por ejemplo /uploads/productos/foto.jpg.</param>
    /// <returns>El fichero como descarga o NotFound si no está en wwwroot.</returns>
    public IActionResult OnGet([FromQuery] string ruta)
    {
        if (!almacen.FileExists(ruta))
        {
            return NotFound();
        }

        var rutaFisica = almacen.GetPhysicalPath(ruta)!;
        return PhysicalFile(rutaFisica, "application/octet-stream", Path.GetFileName(ruta));
    }
}
