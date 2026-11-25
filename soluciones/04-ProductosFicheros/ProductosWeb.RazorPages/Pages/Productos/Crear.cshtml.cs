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
/// Página de alta de producto.
/// OnPostAsync es el equivalente exacto del POST del controlador MVC: dos capas de validación,
/// el fichero de imagen opcional delegado en el almacenamiento y, si todo va bien, el patrón
/// Post-Redirect-Get con TempData.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="almacen">Almacenamiento de los ficheros subidos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
public class CrearModel(
    IProductService servicio,
    IStorageService almacen,
    IValidator<ProductFormViewModel> validador) : PageModel
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

        // Se decide si el formulario vale ANTES de convertirlo y de escribir en disco: un
        // formulario inválido no se convierte ni guarda ficheros.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entidad = Formulario.ToEntity();
        if (!await AplicaImagenAsync(entidad))
        {
            return Page();
        }

        var creado = await servicio.CreateAsync(entidad);

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
    /// Guarda el fichero que haya traído el formulario y escribe su ruta en la entidad.
    /// Sin fichero no se toca la imagen que se haya escrito a mano en el otro campo.
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
