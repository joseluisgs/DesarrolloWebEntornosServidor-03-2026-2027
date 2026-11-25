using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página de Binding avanzado.
/// No persiste nada: su misión es mostrar, línea a línea, todo lo que el model binding
/// ha recogido del formulario, con prefijos y colecciones incluidos.
/// </summary>
/// <param name="validador">Validador del formulario de Binding.</param>
public class BindingModel(IValidator<BindingFormViewModel> validador) : PageModel
{
    /// <summary>Formulario con prefijo e índices; [BindProperty] es quien lo rellena desde el POST.</summary>
    [BindProperty]
    public BindingFormViewModel Formulario { get; set; } = BindingMapper.CrearDeEjemplo();

    /// <summary>
    /// Primera visita: el modelo de partida lleva una cabecera, una etiqueta y un recargo,
    /// para que los índices se vean desde el principio.
    /// </summary>
    public void OnGet()
    {
        // El modelo ya viene preparado por BindingMapper.CrearDeEjemplo().
    }

    /// <summary>
    /// Recibe la página. Añadir una fila no valida (es repintado, no negocio);
    /// el botón de probar sí valida las dos capas y, si todo cuadra, escribe el resumen.
    /// </summary>
    /// <param name="accion">Botón pulsado: anadir-etiqueta, anadir-recargo o probar.</param>
    /// <returns>La propia página con el repintado o con el resumen.</returns>
    public async Task<IActionResult> OnPostAsync(string? accion)
    {
        if (accion == "anadir-etiqueta")
        {
            Formulario.Etiquetas.Add(string.Empty);
            return Page();
        }

        if (accion == "anadir-recargo")
        {
            Formulario.Recargos.Add(new RecargoViewModel());
            return Page();
        }

        var resultado = await validador.ValidateAsync(Formulario);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        ViewData["Resumen"] = Formulario.DescripcionBinding();
        return Page();
    }
}
