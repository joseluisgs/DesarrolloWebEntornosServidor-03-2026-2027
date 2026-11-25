using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Mappers;

/// <summary>
/// Del modelo al DTO y del DTO al formulario, con métodos de extensión.
/// Es el único sitio que sabe cómo se traduce un Product a las formas que usan las vistas.
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
                product.ImagenOrDefault(),
                product.Disponible);

    /// <summary>
    /// Convierte una lista de productos en su lista de DTOs.
    /// </summary>
    /// <param name="products">Lista a convertir.</param>
    /// <returns>Lista de DTOs, vacía si la entrada era null.</returns>
    public static IReadOnlyList<ProductDto> ToDtoList(this IEnumerable<Product> products) =>
        products.Select(p => p.ToDto()!).ToList();

    /// <summary>
    /// Rellena el formulario de alta o edición a partir de un producto ya existente.
    /// Edita la misma instancia para que el controlador conserve el Id al enviarla de vuelta.
    /// </summary>
    /// <param name="model">Formulario a rellenar.</param>
    /// <param name="product">Producto origen.</param>
    /// <returns>El mismo formulario, listo para la vista.</returns>
    public static ProductFormViewModel ToFormViewModel(this ProductFormViewModel model, Product product)
    {
        model.Id = product.Id;
        model.Nombre = product.Nombre;
        model.Descripcion = product.Descripcion;
        model.Precio = product.Precio;
        model.Stock = product.Stock;
        model.Categoria = product.Categoria;
        model.Imagen = product.Imagen;
        return model;
    }

    /// <summary>
    /// Rellena el formulario a partir del DTO que devuelve el servicio.
    /// Es la ruta que usa el GET de edición: el servicio ya ha traducido la entidad.
    /// </summary>
    /// <param name="model">Formulario a rellenar.</param>
    /// <param name="dto">DTO origen.</param>
    /// <returns>El mismo formulario, listo para la vista.</returns>
    public static ProductFormViewModel ToFormViewModel(this ProductFormViewModel model, ProductDto dto)
    {
        model.Id = dto.Id;
        model.Nombre = dto.Nombre;
        model.Descripcion = dto.Descripcion;
        model.Precio = dto.Precio;
        model.Stock = dto.Stock;
        model.Categoria = dto.Categoria;
        model.Imagen = dto.Imagen;
        return model;
    }

    /// <summary>
    /// Devuelve la imagen del producto o la de por defecto si no tiene.
    /// </summary>
    /// <param name="product">Producto a consultar.</param>
    /// <returns>Ruta de imagen usable en la vista.</returns>
    private static string ImagenOrDefault(this Product product) =>
        string.IsNullOrWhiteSpace(product.Imagen) ? "/images/producto-sin-imagen.svg" : product.Imagen;
}
