using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página de alta de producto.
/// OnPostAsync es el equivalente exacto del POST del controlador MVC: dos capas de validación,
/// y si todo va bien el patrón Post-Redirect-Get con TempData.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
public class CrearModel(IProductService servicio, IValidator<ProductFormViewModel> validador) : PageModel
{
    /// <summary>Modelo que la vista pinta con asp-for.</summary>
    [BindProperty]
    public ProductFormViewModel Formulario { get; set; } = new();

    /// <summary>
    /// Primera visita al formulario: el modelo llega vacío.
    /// </summary>
    public void OnGet()
    {
        // No hay nada que preparar: el formulario empieza en blanco.
    }

    /// <summary>
    /// Recibe el alta. El token anti-forgery se comprueba solo, porque el form Tag Helper
    /// lo emitió y el PageModel lo exige por defecto en los POST.
    /// </summary>
    /// <returns>La propia página con los errores si los hay; redirect si se creó.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // DataAnnotations ya se ejecutaron solas al enlazar el modelo con [BindProperty].
        var resultado = await validador.ValidateAsync(Formulario);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var creado = await servicio.CreateAsync(ToEntity(Formulario));

        if (creado is null)
        {
            // El nombre ya existía: el índice único de la base de datos ha decidido.
            ModelState.AddModelError(nameof(Formulario.Nombre), "Ya existe un producto con ese nombre");
            return Page();
        }

        TempData["Exito"] = $"Producto \"{creado.Nombre}\" creado correctamente";
        return RedirectToPage("/Productos/Index");
    }

    /// <summary>
    /// Pasa del formulario al modelo que persiste el servicio. El Id solo se usa al editar.
    /// El mapeo a entidad vive en el PageModel porque es lógica de la vista; el mapeo a DTO,
    /// en cambio, vive en el proyecto compartido porque lo usan las dos visiones.
    /// </summary>
    /// <param name="model">Formulario validado.</param>
    /// <returns>Entidad lista para la base de datos.</returns>
    private static Product ToEntity(ProductFormViewModel model) => new()
    {
        Id = model.Id,
        Nombre = model.Nombre.Trim(),
        Descripcion = model.Descripcion.Trim(),
        Precio = model.Precio,
        Stock = model.Stock,
        Categoria = model.Categoria,
        Imagen = string.IsNullOrWhiteSpace(model.Imagen) ? null : model.Imagen.Trim()
    };
}
