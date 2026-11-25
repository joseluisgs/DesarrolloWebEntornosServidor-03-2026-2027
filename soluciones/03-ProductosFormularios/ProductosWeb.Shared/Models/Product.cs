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

    /// <summary>Categoría del catálogo.</summary>
    public ProductCategory Categoria { get; set; }

    /// <summary>Ruta de la imagen; si está vacía, la vista usa la de por defecto.</summary>
    public string? Imagen { get; set; }

    /// <summary>Si el producto admite pedidos ahora mismo (stock y precio en vigor).</summary>
    public bool Disponible => Stock > 0 && Precio > 0;
}
