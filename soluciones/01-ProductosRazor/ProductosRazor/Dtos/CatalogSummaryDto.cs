namespace ProductosRazor.Dtos;

/// <summary>
/// Resumen del catálogo que pinta el componente de vista CatalogSummary.
/// </summary>
/// <param name="TotalProductos">Número de productos del catálogo.</param>
/// <param name="ValorInventario">Valor total del almacén (precio por stock, sumado).</param>
/// <param name="PorCategoria">Reparto de productos por categoría.</param>
public record CatalogSummaryDto(
    int TotalProductos,
    decimal ValorInventario,
    IReadOnlyList<CategoryCountDto> PorCategoria);
