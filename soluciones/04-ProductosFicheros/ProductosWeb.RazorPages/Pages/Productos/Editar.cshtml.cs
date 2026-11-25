using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.Services.Storage;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.RazorPages.Pages.Productos;

/// <summary>
/// Página de edición de producto.
/// El GET precarga el formulario desde el Id de la ruta; el POST recibe el modelo completo
/// con un campo oculto Id y vuelve al listado con TempData si todo va bien.
/// Como el alta, el form es multipart y puede traer una imagen nueva.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="almacen">Almacenamiento de los ficheros subidos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
public class EditarModel(
    IProductService servicio,
    IStorageService almacen,
    IValidator<ProductFormViewModel> validador) : PageModel
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
    public async Task<IActionResult> OnPostAsync()
    {
        var resultado = await validador.ValidateAsync(Formulario);
        resultado.AddToModelState(ModelState);

        // Misma orden que en el alta: primero se decide si el formulario vale y sólo
        // después se convierte y se toca el disco.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entidad = Formulario.ToEntity();
        if (!await AplicaImagenAsync(entidad))
        {
            return Page();
        }

        var actualizado = await servicio.UpdateAsync(Formulario.Id, entidad);

        if (actualizado is null)
        {
            TempData["Error"] = "El producto que intentabas editar ya no existe";
            return RedirectToPage("/Productos/Index");
        }

        TempData["Exito"] = $"Producto \"{actualizado.Nombre}\" actualizado correctamente";
        return RedirectToPage("/Productos/Index");
    }

    /// <summary>
    /// Guarda el fichero que haya traído el formulario y escribe su ruta en la entidad.
    /// Sin fichero se conserva la imagen que ya tenía el producto.
    /// </summary>
    /// <param name="entidad">Entidad que se va a persistir.</param>
    /// <returns>true si se puede seguir; false si el servicio de almacenamiento rechazó el fichero.</returns>
    private async Task<bool> AplicaImagenAsync(Product entidad)
    {
        if (Formulario.ImagenFile is null)
        {
            return true;
        }

        var resultado = await almacen.SaveFileAsync(Formulario.ImagenFile, "productos");
        if (!resultado.Ok)
        {
            ModelState.AddModelError(nameof(Formulario.ImagenFile), resultado.Mensaje);
            return false;
        }

        entidad.Imagen = resultado.Ruta;
        return true;
    }
}
