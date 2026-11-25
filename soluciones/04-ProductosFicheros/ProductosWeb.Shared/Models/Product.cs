using System.ComponentModel.DataAnnotations;
using ProductosWeb.Shared.Models.Enums;

namespace ProductosWeb.Shared.Models;

/// <summary>
/// Entidad de producto del catálogo.
/// El modelo se valida también en el formulario con DataAnnotations y FluentValidation:
/// aquí solo quedan las restricciones de la base de datos.
/// </summary>
public class Product
{
    /// <summary>Identificador único del producto (clave primaria en SQLite).</summary>
    public long Id { get; set; }

    /// <summary>Nombre del producto (obligatorio, hasta 200 caracteres).</summary>
    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción larga del producto (obligatoria, hasta 1000 caracteres).</summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Precio unitario en euros. La coma o el punto del formulario los normaliza el binder.</summary>
    [Range(0.01, 100000, ErrorMessage = "El precio debe estar entre 0,01 y 100.000")]
    public decimal Precio { get; set; }

    /// <summary>Unidades en stock (nunca negativas).</summary>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    /// <summary>Año de la edición del producto (entre 2000 y 2030).</summary>
    [Range(2000, 2030, ErrorMessage = "El año debe estar entre 2000 y 2030")]
    public int Anio { get; set; }

    /// <summary>Categoría del catálogo.</summary>
    public ProductCategory Categoria { get; set; }

    /// <summary>Si el producto entra en el escaparate de novedades.</summary>
    public bool EsNovedad { get; set; }

    /// <summary>
    /// Ruta pública de la imagen subida, por ejemplo
    /// <c>/uploads/productos/20261003220843_dc66fe7f_foto.jpg</c>.
    /// Si está vacía, la vista usa la de por defecto.
    /// </summary>
    public string? Imagen { get; set; }

    /// <summary>
    /// Etiquetas libres del producto (oferta, recomendado...). Es una colección en memoria
    /// y una sola columna de texto en SQLite: la conversión de valor del DbContext decide
    /// el formato, que aquí no se ve en ningún sitio.
    /// </summary>
    public List<string> Etiquetas { get; set; } = [];

    /// <summary>Si el producto admite pedidos ahora mismo (stock y precio en vigor).</summary>
    public bool Disponible => Stock > 0 && Precio > 0;
}
