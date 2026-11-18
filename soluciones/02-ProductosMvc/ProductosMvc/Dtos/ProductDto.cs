using ProductosMvc.Models.Enums;

namespace ProductosMvc.Dtos;

/// <summary>
/// Producto ya preparado para la vista.
/// El PageModel pide DTOs al servicio y la vista solo pinta lo que el DTO expone:
/// si mañana la entidad gana un campo interno, la vista no se entera.
/// </summary>
/// <param name="Id">Identificador del producto.</param>
/// <param name="Nombre">Nombre visible.</param>
/// <param name="Descripcion">Descripción larga.</param>
/// <param name="Precio">Precio unitario en euros.</param>
/// <param name="Stock">Unidades disponibles.</param>
/// <param name="Categoria">Categoría (el enum, para que la vista pueda hacer switch).</param>
/// <param name="Imagen">Ruta de la imagen, ya resuelta con la de por defecto.</param>
/// <param name="Disponible">Si queda alguna unidad.</param>
public record ProductDto(
    long Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    ProductCategory Categoria,
    string Imagen,
    bool Disponible);
