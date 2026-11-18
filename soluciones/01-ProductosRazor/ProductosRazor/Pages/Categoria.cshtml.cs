using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Models.Enums;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Los productos de una categoría.
/// El enum llega de la query string (?categoria=Audio) y el Model Binding lo convierte
/// solo: en el handler no hay ningún Parse ni ningún Switch sobre cadenas.
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
public class CategoriaModel(IProductService service) : PageModel
{
    /// <summary>Categoría pedida, para el título y la miga de pan.</summary>
    public ProductCategory Categoria { get; private set; }

    /// <summary>Productos de esa categoría.</summary>
    public IReadOnlyList<ProductDto> Productos { get; private set; } = Array.Empty<ProductDto>();

    /// <summary>
    /// Handler del GET. El parámetro se llama igual que el de la query string.
    /// </summary>
    /// <param name="categoria">Categoría pedida en la query string.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La página con los productos filtrados.</returns>
    public async Task<IActionResult> OnGetAsync(
        string? categoria,
        CancellationToken cancellationToken = default)
    {
        if(categoria is null || !Enum.TryParse<ProductCategory>(categoria, true, out var cat))
        {
            return NotFound();
        }
        Categoria = cat;
        Productos = await service.GetByCategoryAsync(cat, cancellationToken);

        return Page();
    }
}
