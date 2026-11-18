using ProductosMvc.Dtos;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista del catálogo, y nada más.
/// El ViewModel es la fachada de la vista: si mañana el servicio devuelve la lista entera
/// para calcular un total, esta vista no se entera, porque aquí solo entra lo que ella pide.
/// </summary>
/// <param name="Productos">Productos a pintar en la rejilla.</param>
/// <param name="Resumen">Resumen que pinta el componente CatalogSummary.</param>
public record CatalogoViewModel(
    IReadOnlyList<ProductDto> Productos,
    CatalogSummaryDto Resumen);
