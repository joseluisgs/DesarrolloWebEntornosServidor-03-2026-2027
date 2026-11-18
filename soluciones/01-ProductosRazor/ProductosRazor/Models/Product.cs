using System.ComponentModel.DataAnnotations;
using ProductosRazor.Models.Enums;

namespace ProductosRazor.Models;

/// <summary>
/// Entidad de producto del catálogo.
/// Es el dominio de todos los ejemplos de la unidad: la misma clase de la TiendaWeb,
/// aquí recortada a lo que este ejemplo necesita para enseñar Razor.
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

    /// <summary>Precio unitario en euros. La coma o el punto dependen de la cultura del usuario.</summary>
    [Range(0.01, 1_000_000, ErrorMessage = "El precio debe estar entre 0,01 y 1.000.000")]
    public decimal Precio { get; set; }

    /// <summary>Unidades disponibles en almacén.</summary>
    public int Stock { get; set; }

    /// <summary>Categoría del producto.</summary>
    public ProductCategory Categoria { get; set; }

    /// <summary>Ruta de la imagen; si no hay, se sirve la de por defecto.</summary>
    public string? Imagen { get; set; }

    /// <summary>Fecha de alta en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ruta de la imagen a servir en la vista, con la de por defecto de reserva.
    /// La vista no decide nada: pide esta propiedad y pinta lo que salga.
    /// </summary>
    public string ImagenOrDefault =>
        string.IsNullOrWhiteSpace(Imagen) ? "/images/producto-sin-imagen.svg" : Imagen;

    /// <summary>True si queda alguna unidad en almacén.</summary>
    public bool Disponible => Stock > 0;
}
