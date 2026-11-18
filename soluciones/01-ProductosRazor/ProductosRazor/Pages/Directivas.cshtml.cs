using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Demostración de las directivas de Razor: @@inject, @@functions y @@section.
/// </summary>
/// <param name="service">Servicio del catálogo, para demostrar @@inject.</param>
public class DirectivasModel(IProductService service) : PageModel
{
    /// <summary>Resumen del catálogo, por si la vista quiere leerlo del PageModel.</summary>
    public CatalogSummaryDto? Resumen { get; private set; }

    /// <summary>
    /// Handler del GET.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Tarea asíncrona.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
        => Resumen = await service.GetCatalogSummaryAsync(cancellationToken);
}
