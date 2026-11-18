using ProductosMvc.Dtos;
using ProductosMvc.Models.Enums;

namespace ProductosMvc.Services;

/// <summary>
/// Lógica de negocio del catálogo.
/// Los PageModels piden datos aquí, no al contexto: la vista nunca toca la base de datos.
/// Todos los métodos devuelven DTOs, no entidades.
/// </summary>
public interface IProductService
{
    /// <summary>El catálogo completo, ordenado alfabéticamente.</summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Lista de DTOs, vacía si no hay productos.</returns>
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Un producto por su identificador.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>El DTO, o null si no existe.</returns>
    Task<ProductDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>Los productos de una categoría, ordenados alfabéticamente.</summary>
    /// <param name="categoria">Categoría a filtrar.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Lista de DTOs de esa categoría.</returns>
    Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(
        ProductCategory categoria,
        CancellationToken cancellationToken = default);

    /// <summary>El resumen del catálogo que pinta el componente CatalogSummary.</summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Conteos y valor de inventario, calculados en una sola consulta.</returns>
    Task<CatalogSummaryDto> GetCatalogSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Productos de la misma categoría que el dado, para la sección de sugerencias de la ficha.
    /// </summary>
    /// <param name="id">Identificador del producto de referencia.</param>
    /// <param name="maximo">Cuántos sugerir como mucho.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Lista de DTOs; vacía si el producto no existe o no tiene hermanos.</returns>
    Task<IReadOnlyList<ProductDto>> GetRelatedAsync(
        long id,
        int maximo = 3,
        CancellationToken cancellationToken = default);
}
