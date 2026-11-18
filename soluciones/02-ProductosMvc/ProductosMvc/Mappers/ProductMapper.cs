using ProductosMvc.Dtos;
using ProductosMvc.Models;

namespace ProductosMvc.Mappers;

/// <summary>
/// Del modelo al DTO, con métodos de extensión.
/// Es el único sitio que sabe cómo se traduce un Product a un ProductDto:
/// ni el servicio repite el mapeo, ni la vista conoce la entidad.
/// </summary>
public static class ProductMapper
{
    /// <summary>
    /// Convierte un producto en su DTO para la vista.
    /// </summary>
    /// <param name="product">Producto a convertir; null devuelve null.</param>
    /// <returns>El DTO, o null si no había producto.</returns>
    public static ProductDto? ToDto(this Product? product) =>
        product is null
            ? null
            : new ProductDto(
                product.Id,
                product.Nombre,
                product.Descripcion,
                product.Precio,
                product.Stock,
                product.Categoria,
                product.ImagenOrDefault,
                product.Disponible);

    /// <summary>
    /// Convierte una colección de productos en su lista de DTOs, sin alterar el orden.
    /// </summary>
    /// <param name="products">Productos a convertir.</param>
    /// <returns>Lista de DTOs lista para el PageModel.</returns>
    public static IReadOnlyList<ProductDto> ToDtoList(this IEnumerable<Product> products) =>
        products
            .Select(p => p.ToDto())
            .OfType<ProductDto>()
            .ToList();
}
