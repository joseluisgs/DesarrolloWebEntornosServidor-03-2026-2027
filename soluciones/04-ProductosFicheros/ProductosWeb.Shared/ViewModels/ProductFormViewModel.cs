using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductosWeb.Shared.Models.Enums;

namespace ProductosWeb.Shared.ViewModels;

/// <summary>
/// Formulario de alta y edición de producto.
/// La vista lo usa con asp-for, que escribe el nombre de cada campo como atributo name:
/// Nombre, Descripcion, Anio, Precio, Stock, Categoria, EsNovedad y los índices de las etiquetas.
/// Los DataAnnotations cubren lo básico; las reglas que se cruzan están en el validador FluentValidation.
/// El fichero no viaja como texto: entra por ImagenFile y el servidor lo guarda con nombre propio.
/// </summary>
public class ProductFormViewModel
{
    /// <summary>0 en el alta; el Id real al editar. No aparece en el formulario.</summary>
    public long Id { get; set; }

    /// <summary>Nombre del producto (obligatorio, hasta 200 caracteres).</summary>
    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción larga del producto (obligatoria, hasta 1000 caracteres).</summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres")]
    [Display(Name = "Descripción")]
    [DataType(DataType.MultilineText)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Precio unitario en euros; el binder admite coma o punto.</summary>
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 100000, ErrorMessage = "El precio debe estar entre 0,01 y 100.000")]
    [Display(Name = "Precio (euros)")]
    public decimal Precio { get; set; }

    /// <summary>Unidades en stock (nunca negativas).</summary>
    [Range(0, 9999, ErrorMessage = "El stock debe estar entre 0 y 9.999")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    /// <summary>Año del modelo o de la edición; el formulario admite un catálogo acotado.</summary>
    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2000, 2030, ErrorMessage = "El año debe estar entre 2000 y 2030")]
    [Display(Name = "Año")]
    public int Anio { get; set; }

    /// <summary>Categoría del catálogo; el formulario envía el nombre del valor del enum.</summary>
    [Display(Name = "Categoría")]
    public ProductCategory Categoria { get; set; }

    /// <summary>Casilla de novedad: la cuarta forma de campo de un formulario completo.</summary>
    [Display(Name = "Novedad")]
    public bool EsNovedad { get; set; }

    /// <summary>Ruta opcional de la imagen; si se deja vacía, la vista usa la de por defecto.</summary>
    [StringLength(500, ErrorMessage = "La ruta de la imagen no puede superar los 500 caracteres")]
    [Display(Name = "Imagen (ruta o URL)")]
    public string? Imagen { get; set; }

    /// <summary>
    /// Fichero subido en el formulario multipart.
    /// No lleva DataAnnotations: el tipo, el tamaño y el contenido los decide el servicio de
    /// almacenamiento, que es donde vive esa validación en las dos visiones.
    /// </summary>
    [Display(Name = "Imagen del producto")]
    public IFormFile? ImagenFile { get; set; }

    /// <summary>Etiquetas libres del producto; el formulario añade y quita filas con JavaScript.</summary>
    public List<string> Etiquetas { get; set; } = [];
}
