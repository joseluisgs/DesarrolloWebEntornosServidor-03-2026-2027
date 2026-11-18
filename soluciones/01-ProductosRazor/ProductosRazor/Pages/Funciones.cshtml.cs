using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Demostración de funciones y delegados dentro de una vista.
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
public class FuncionesModel(IProductService service) : PageModel
{
    /// <summary>Productos del catálogo.</summary>
    public IReadOnlyList<ProductDto> Productos { get; private set; } = Array.Empty<ProductDto>();

    /// <summary>Precio más alto del catálogo, para demostrar los delegados.</summary>
    public decimal PrecioMaximo { get; private set; }

    /// <summary>
    /// Handler del GET.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Tarea asíncrona.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        Productos = await service.GetAllAsync(cancellationToken);
        PrecioMaximo = Productos.Count == 0 ? 0 : Productos.Max(p => p.Precio);
    }
}
