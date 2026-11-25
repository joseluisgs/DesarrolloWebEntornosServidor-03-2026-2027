using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Models;

namespace ProductosWeb.Shared.Services;

/// <summary>
/// Catálogo de productos: las cuatro operaciones que necesita un formulario.
/// El servicio trabaja con la entidad y devuelve DTOs: la vista nunca toca el modelo de la base de datos.
/// </summary>
public interface IProductService
{
    /// <summary>Devuelve todo el catálogo ordenado por nombre.</summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de productos en formato DTO.</returns>
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Busca un producto por su identificador.</summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El producto en formato DTO, o null si no existe.</returns>
    Task<ProductDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un producto nuevo.
    /// El índice único de nombre en la base de datos garantiza que no se repita:
    /// si ya existe, no lanza excepción, devuelve null y el formulario avisa al usuario.
    /// </summary>
    /// <param name="product">Producto a crear (con Id a 0).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El producto creado en formato DTO, o null si el nombre ya existía.</returns>
    Task<ProductDto?> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un producto existente.</summary>
    /// <param name="id">Identificador del producto a actualizar.</param>
    /// <param name="product">Producto con los datos nuevos.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El producto actualizado en formato DTO, o null si no existe.</returns>
    Task<ProductDto?> UpdateAsync(long id, Product product, CancellationToken cancellationToken = default);

    /// <summary>Elimina un producto del catálogo.</summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>true si se eliminó; false si no existía.</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
