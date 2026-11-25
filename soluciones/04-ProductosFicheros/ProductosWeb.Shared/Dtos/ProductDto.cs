using ProductosWeb.Shared.Models.Enums;

namespace ProductosWeb.Shared.Dtos;

/// <summary>
/// Representación pública de un producto para las vistas.
/// Envuelve la entidad sin exponer el modelo de la base de datos:
/// las vistas solo conocen este tipo y el modelo siguen siendo intercambiables.
/// </summary>
/// <param name="Id">Identificador del producto.</param>
/// <param name="Nombre">Nombre visible.</param>
/// <param name="Descripcion">Descripción larga.</param>
/// <param name="Precio">Precio unitario en euros.</param>
/// <param name="Stock">Unidades disponibles.</param>
/// <param name="Anio">Año de la edición del producto.</param>
/// <param name="Categoria">Categoría (el enum, para que la vista pueda hacer switch).</param>
/// <param name="EsNovedad">Si el producto entra en el escaparate de novedades.</param>
/// <param name="Imagen">Ruta de la imagen, ya resuelta con la de por defecto.</param>
/// <param name="Disponible">Si queda alguna unidad.</param>
public record ProductDto(
    long Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    int Anio,
    ProductCategory Categoria,
    bool EsNovedad,
    string Imagen,
    bool Disponible)
{
    /// <summary>
    /// Etiquetas libres del producto. Va como propiedad y no como parámetro posicional
    /// para no romper las construcciones que no tienen etiquetas.
    /// </summary>
    public IReadOnlyList<string> Etiquetas { get; init; } = [];
}
