using System.ComponentModel.DataAnnotations;

namespace ProductosWeb.Shared.ViewModels;

/// <summary>
/// Formulario de la vista de Binding.
/// El objeto Cabecera se envía con prefijo (Cabecera.Nombre, Cabecera.Precio...) y las
/// colecciones, por índice (Etiquetas[0], Recargos[1].Concepto): así se ve el model binding
/// avanzado en acción, con una sola vista.
/// </summary>
public class BindingFormViewModel
{
    /// <summary>Datos del producto; el formulario los envía con el prefijo "Cabecera".</summary>
    [Required(ErrorMessage = "Debes rellenar la cabecera del producto")]
    public ProductFormViewModel Cabecera { get; set; } = new();

    /// <summary>Lista simple de textos; cada elemento es un campo del formulario con su índice.</summary>
    public List<string> Etiquetas { get; set; } = [];

    /// <summary>Lista de objetos; cada fila aporta dos campos con el mismo índice.</summary>
    public List<RecargoViewModel> Recargos { get; set; } = [];
}
