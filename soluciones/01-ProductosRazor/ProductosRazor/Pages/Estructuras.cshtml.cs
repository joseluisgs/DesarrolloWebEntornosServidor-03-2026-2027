using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Demostración de las estructuras de control de C# dentro de una vista.
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
public class EstructurasModel(IProductService service) : PageModel
{
    /// <summary>Productos del catálogo, para recorrerlos desde la vista.</summary>
    public IReadOnlyList<ProductDto> Productos { get; private set; } = Array.Empty<ProductDto>();

    /// <summary>Suma de todas las unidades en almacén.</summary>
    public int StockTotal { get; private set; }

    /// <summary>
    /// Handler del GET.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Tarea asíncrona.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        Productos = await service.GetAllAsync(cancellationToken);
        StockTotal = Productos.Sum(p => p.Stock);
    }
}
