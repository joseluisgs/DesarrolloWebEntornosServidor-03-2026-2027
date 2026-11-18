using Microsoft.EntityFrameworkCore;
using ProductosMvc.Data;
using ProductosMvc.Dtos;
using ProductosMvc.Mappers;
using ProductosMvc.Models.Enums;

namespace ProductosMvc.Services;

/// <summary>
/// Implementación del catálogo sobre SQLite con EF Core.
/// Primary constructor: los parámetros ya son los campos, sin repetir el mapeo a mano.
/// </summary>
/// <param name="context">Contexto de la base de datos.</param>
/// <param name="logger">Logger del servicio.</param>
public class ProductService(ProductsDbContext context, ILogger<ProductService> logger) : IProductService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Leyendo el catálogo completo");

        var productos = await context.Products
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);

        return productos.ToDtoList();
    }

    /// <inheritdoc />
    public async Task<ProductDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Leyendo el producto {Id}", id);

        var producto = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return producto.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(
        ProductCategory categoria,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Leyendo los productos de la categoría {Categoria}", categoria);

        var productos = await context.Products
            .AsNoTracking()
            .Where(p => p.Categoria == categoria)
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);

        return productos.ToDtoList();
    }

    /// <inheritdoc />
    public async Task<CatalogSummaryDto> GetCatalogSummaryAsync(CancellationToken cancellationToken = default)
    {
        var productos = await context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var porCategoria = productos
            .GroupBy(p => p.Categoria)
            .OrderBy(g => g.Key)
            .Select(g => new CategoryCountDto(g.Key.ToString(), g.Count()))
            .ToList();

        return new CatalogSummaryDto(
            productos.Count,
            productos.Sum(p => p.Precio * p.Stock),
            porCategoria);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductDto>> GetRelatedAsync(
        long id,
        int maximo = 3,
        CancellationToken cancellationToken = default)
    {
        var producto = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto is null)
        {
            return Array.Empty<ProductDto>();
        }

        logger.LogDebug("Leyendo sugerencias del producto {Id}", id);

        var relacionados = await context.Products
            .AsNoTracking()
            .Where(p => p.Categoria == producto.Categoria && p.Id != id)
            .OrderBy(p => p.Precio)
            .Take(maximo)
            .ToListAsync(cancellationToken);

        return relacionados.ToDtoList();
    }
}
