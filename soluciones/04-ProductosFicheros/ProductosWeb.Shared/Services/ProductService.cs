using Microsoft.EntityFrameworkCore;
using ProductosWeb.Shared.Data;
using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.Models;

namespace ProductosWeb.Shared.Services;

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
    public async Task<ProductDto?> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Creando el producto {Nombre}", product.Nombre);

        // Comprobación previa: el índice único es la red de seguridad de la base de datos,
        // pero avisar aquí permite devolver null sin llegar a lanzar la excepción.
        var yaExiste = await context.Products
            .AnyAsync(p => p.Nombre == product.Nombre, cancellationToken);

        if (yaExiste)
        {
            logger.LogWarning("Ya existe un producto llamado {Nombre}", product.Nombre);
            return null;
        }

        try
        {
            context.Products.Add(product);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Caso de dos peticiones simultáneas con el mismo nombre: gana una, la otra recibe null.
            logger.LogWarning("No se pudo crear {Nombre}: el nombre ya existe en la base de datos", product.Nombre);
            context.Entry(product).State = EntityState.Detached;
            return null;
        }

        return product.ToDto();
    }

    /// <inheritdoc />
    public async Task<ProductDto?> UpdateAsync(long id, Product product, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Actualizando el producto {Id}", id);

        var existente = await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existente is null)
        {
            return null;
        }

        // No se cambia el Id: el que manda es el de la URL.
        existente.Nombre = product.Nombre;
        existente.Descripcion = product.Descripcion;
        existente.Precio = product.Precio;
        existente.Stock = product.Stock;
        existente.Anio = product.Anio;
        existente.Categoria = product.Categoria;
        existente.EsNovedad = product.EsNovedad;
        existente.Imagen = product.Imagen;

        // Las etiquetas se sustituyen enteras: el formulario manda la lista completa.
        // Se asigna una lista nueva (y no se limpia la vieja) porque EF Core rastrea la
        // colección original y sólo detecta el cambio si recibe otra instancia.
        existente.Etiquetas = product.Etiquetas.ToList();

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            logger.LogWarning("No se pudo actualizar {Id}: el nombre {Nombre} ya existe", id, product.Nombre);
            return null;
        }

        return existente.ToDto();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Eliminando el producto {Id}", id);

        var existente = await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existente is null)
        {
            return false;
        }

        context.Products.Remove(existente);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
