using Microsoft.AspNetCore.Mvc;
using ProductosRazor.Services;

namespace ProductosRazor.ViewComponents;

/// <summary>
/// Componente de vista: el resumen del catálogo.
/// Vive en ViewComponents/ y su vista en Pages/Shared/Components/CatalogSummary/Default.cshtml.
/// Se invoca desde cualquier página con <c>&lt;vc:catalog-summary /&gt;</c>,
/// que es la versión declarativa de llamarlo a mano con @await Component.InvokeAsync(...).
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
public class CatalogSummaryViewComponent(IProductService service) : ViewComponent
{
    /// <summary>
    /// Monta el resumen y devuelve su vista por defecto.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Vista del componente con el resumen ya calculado.</returns>
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken = default)
    {
        var resumen = await service.GetCatalogSummaryAsync(cancellationToken);
        return View(resumen);
    }
}
