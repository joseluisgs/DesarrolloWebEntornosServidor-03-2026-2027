using System.ComponentModel.DataAnnotations;

namespace ProductosWeb.Shared.ViewModels;

/// <summary>
/// Recargo opcional que se aplica sobre el precio del producto.
/// Cada fila del formulario de Binding se envía como Recargos[i].Concepto y Recargos[i].Importe.
/// </summary>
public class RecargoViewModel
{
    /// <summary>Nombre del recargo (obligatorio).</summary>
    [Required(ErrorMessage = "El concepto del recargo es obligatorio")]
    [StringLength(100, ErrorMessage = "El concepto no puede superar los 100 caracteres")]
    [Display(Name = "Concepto")]
    public string Concepto { get; set; } = string.Empty;

    /// <summary>Importe del recargo en euros; el binder admite coma o punto.</summary>
    [Range(0.01, 1000, ErrorMessage = "El importe debe estar entre 0,01 y 1.000")]
    [Display(Name = "Importe (euros)")]
    public decimal Importe { get; set; }
}
