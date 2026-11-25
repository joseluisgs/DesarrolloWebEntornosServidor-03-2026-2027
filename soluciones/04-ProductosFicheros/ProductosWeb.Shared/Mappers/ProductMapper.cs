using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Mappers;

/// <summary>
/// Mapeador de productos: conversión entre la entidad del modelo de dominio y el DTO.
/// Centraliza el mapeo para que los controladores y servicios no repitan
/// el mismo código de asignación de propiedades en cada acción.
/// Los métodos son de extensión: se escriben <c>product.ToDto()</c> y se leen como el propio DTO.
/// </summary>
public static class ProductMapper
{
    /// <summary>
    /// Convierte una entidad de producto en su DTO para la vista.
    /// </summary>
    /// <param name="product">Producto (o null); un null se devuelve como null sin comprobaciones extra.</param>
    /// <returns>El producto convertido en DTO, o null si no hay producto.</returns>
    public static ProductDto? ToDto(this Product? product) =>
        product is null
            ? null
            : new ProductDto(
                product.Id,
                product.Nombre,
                product.Descripcion,
                product.Precio,
                product.Stock,
                product.Anio,
                product.Categoria,
                product.EsNovedad,
                product.ImagenOrDefault(),
                product.Disponible)
            {
                Etiquetas = product.Etiquetas.ToList()
            };

    /// <summary>
    /// Convierte una colección de productos en una lista de DTOs.
    /// </summary>
    /// <param name="products">Colección de productos.</param>
    /// <returns>Lista de DTOs (vacía si la entrada es null).</returns>
    public static IReadOnlyList<ProductDto> ToDtoList(this IEnumerable<Product> products) =>
        products.Select(p => p.ToDto()!).ToList();

    /// <summary>
    /// Rellena el ViewModel de formulario con los datos de una entidad para precargar
    /// la edición. Recibe el ViewModel y lo devuelve modificado: así las dos visiones
    /// llaman a un único método en lugar de copiar campo a campo.
    /// </summary>
    /// <param name="model">ViewModel a rellenar.</param>
    /// <param name="product">Producto con los datos.</param>
    /// <returns>El mismo ViewModel, ya relleno.</returns>
    public static ProductFormViewModel ToFormViewModel(this ProductFormViewModel model, Product product)
    {
        model.Id = product.Id;
        model.Nombre = product.Nombre;
        model.Descripcion = product.Descripcion;
        model.Precio = product.Precio;
        model.Stock = product.Stock;
        model.Anio = product.Anio;
        model.Categoria = product.Categoria;
        model.EsNovedad = product.EsNovedad;
        model.Imagen = product.Imagen;
        model.Etiquetas = product.Etiquetas.ToList();
        return model;
    }

    /// <summary>
    /// Rellena el ViewModel de formulario con los datos de un DTO, que es lo que devuelve
    /// el servicio al buscar el producto a editar.
    /// </summary>
    /// <param name="model">ViewModel a rellenar.</param>
    /// <param name="dto">Producto en formato DTO.</param>
    /// <returns>El mismo ViewModel, ya relleno.</returns>
    public static ProductFormViewModel ToFormViewModel(this ProductFormViewModel model, ProductDto dto)
    {
        model.Id = dto.Id;
        model.Nombre = dto.Nombre;
        model.Descripcion = dto.Descripcion;
        model.Precio = dto.Precio;
        model.Stock = dto.Stock;
        model.Anio = dto.Anio;
        model.Categoria = dto.Categoria;
        model.EsNovedad = dto.EsNovedad;
        model.Imagen = dto.Imagen;
        model.Etiquetas = dto.Etiquetas.ToList();
        return model;
    }

    /// <summary>
    /// Devuelve la imagen del producto o la de por defecto si no tiene.
    /// </summary>
    /// <param name="product">Producto a consultar.</param>
    /// <returns>Ruta de imagen usable en la vista.</returns>
    private static string ImagenOrDefault(this Product product) =>
        string.IsNullOrWhiteSpace(product.Imagen) ? "/images/producto-sin-imagen.svg" : product.Imagen;

    /// <summary>
    /// Convierte el ViewModel de formulario en entidad para su persistencia.
    /// Las etiquetas se quedan con las que traigan texto: el formulario pinta filas vacías
    /// y el validador ya ha dicho cuáles no valen, así que aquí sólo se guardan las buenas.
    /// Los campos de texto se reciben con <c>?? string.Empty</c> y las etiquetas con
    /// <c>model.Etiquetas ?? []</c> porque un binder puede devolver el modelo a medias y este
    /// método se llama antes de comprobar el ModelState: aquí no se puede permitir un nulo.
    /// </summary>
    /// <param name="model">ViewModel con los datos del formulario.</param>
    /// <returns>La entidad lista para guardar.</returns>
    public static Product ToEntity(this ProductFormViewModel model) =>
        new()
        {
            Id = model.Id,
            Nombre = (model.Nombre ?? string.Empty).Trim(),
            Descripcion = (model.Descripcion ?? string.Empty).Trim(),
            Precio = model.Precio,
            Stock = model.Stock,
            Anio = model.Anio,
            Categoria = model.Categoria,
            EsNovedad = model.EsNovedad,
            Imagen = model.Imagen,
            Etiquetas = (model.Etiquetas ?? [])
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .ToList()
        };
}
