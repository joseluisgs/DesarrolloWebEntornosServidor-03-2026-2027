using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Ficha de un producto.
/// El id llega del enrutado: el parámetro del handler se llama igual que el segmento
/// de la ruta y el framework lo enlaza solo (Model Binding, tema 14).
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
public class DetalleModel(IProductService service) : PageModel
{
    /// <summary>El producto pedido, o null si no existe.</summary>
    public ProductDto? Producto { get; private set; }

    /// <summary>
    /// Otros productos de la misma categoría. Son de presentación, no del modelo de negocio:
    /// por eso no están en el DTO del producto, sino aquí, en el PageModel.
    /// </summary>
    public IReadOnlyList<ProductDto> Sugerencias { get; private set; } = Array.Empty<ProductDto>();

    /// <summary>
    /// Handler del GET. Si el producto no existe, responde 404 y ni siquiera se pinta la vista.
    /// </summary>
    /// <param name="id">Identificador que viene en la ruta.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La página, o NotFound si no hay producto.</returns>
    public async Task<IActionResult> OnGetAsync(long id, CancellationToken cancellationToken = default)
    {
        Producto = await service.GetByIdAsync(id, cancellationToken);

        if (Producto is null)
        {
            return NotFound();
        }

        Sugerencias = await service.GetRelatedAsync(id, 3, cancellationToken);

        return Page();
    }
}
