using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Services;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página del catálogo.
/// En Razor Pages la lógica de la vista vive en el PageModel, en el mismo fichero .cs,
/// y no en un controlador separado: cada página es una unidad autónoma.
/// Aquí conviven el listado y el handler de borrado, que es el que dispara el formulario.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
public class IndexModel(IProductService servicio) : PageModel
{
    /// <summary>Lista de productos que la vista pinta en la tabla.</summary>
    public IReadOnlyList<ProductDto> Productos { get; private set; } = [];

    /// <summary>
    /// Carga el catálogo completo. TempData ya lo prepara el middleware para esta petición.
    /// </summary>
    /// <returns>Tarea asíncrona.</returns>
    public async Task OnGetAsync()
    {
        Productos = await servicio.GetAllAsync();
    }

    /// <summary>
    /// Elimina un producto. El nombre del método es el valor de asp-page-handler en la vista:
    /// el form Tag Helper arma la URL /Productos/Index?handler=Eliminar&amp;id=3.
    /// </summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Redirect al listado con el mensaje correspondiente.</returns>
    public async Task<IActionResult> OnPostEliminarAsync(long id)
    {
        var eliminado = await servicio.DeleteAsync(id);

        TempData[eliminado ? "Exito" : "Error"] = eliminado
            ? "Producto eliminado correctamente"
            : "El producto que intentabas eliminar ya no existe";

        return RedirectToPage();
    }
}
