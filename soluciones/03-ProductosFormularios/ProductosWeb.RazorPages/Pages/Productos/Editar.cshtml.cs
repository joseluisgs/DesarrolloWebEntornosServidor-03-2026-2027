using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página de edición de producto.
/// El GET precarga el formulario desde el Id de la ruta; el POST recibe el modelo completo
/// con un campo oculto Id y vuelve al listado con TempData si todo va bien.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
public class EditarModel(IProductService servicio, IValidator<ProductFormViewModel> validador) : PageModel
{
    /// <summary>Modelo que la vista pinta con asp-for; en el POST llega relleno desde el form.</summary>
    [BindProperty]
    public ProductFormViewModel Formulario { get; set; } = new();

    /// <summary>
    /// Precarga el formulario con el producto de la ruta. Si no existe, se vuelve al catálogo.
    /// </summary>
    /// <param name="id">Identificador del producto a editar.</param>
    /// <returns>La propia página, o redirect si el producto no existe.</returns>
    public async Task<IActionResult> OnGetAsync(long id)
    {
        var producto = await servicio.GetByIdAsync(id);

        if (producto is null)
        {
            TempData["Error"] = "El producto solicitado no existe";
            return RedirectToPage("/Productos/Index");
        }

        Formulario = new ProductFormViewModel().ToFormViewModel(producto);
        return Page();
    }

    /// <summary>
    /// Recibe la edición. El Id viaja en el modelo, no en la URL, y el servicio lo usa para
    /// localizar el registro que hay que actualizar.
    /// </summary>
    /// <returns>La propia página con los errores si los hay; redirect si se actualizó.</returns>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resultado = await validador.ValidateAsync(Formulario);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return Page();
        }
        if (id != Formulario.Id)
        {
            ModelState.AddModelError(string.Empty, "El Id del formulario no coincide con el de la ruta");
            return Page();
        }

        var actualizado = await servicio.UpdateAsync(Formulario.Id, ToEntity(Formulario));

        if (actualizado is null)
        {
            TempData["Error"] = "El producto que intentabas editar ya no existe";
            return RedirectToPage("/Productos/Index");
        }

        TempData["Exito"] = $"Producto \"{actualizado.Nombre}\" actualizado correctamente";
        return RedirectToPage("/Productos/Index");
    }

    /// <summary>
    /// Pasa del formulario al modelo que persiste el servicio.
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
