using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductosMvc.Models.Enums;
using ProductosMvc.Services;
using ProductosMvc.ViewModels;

namespace ProductosMvc.Controllers;

/// <summary>
/// Catálogo de productos en MVC.
/// Es el equivalente a los PageModels del ejemplo 01: recibe la petición, pide los datos
/// al servicio y devuelve una vista. La diferencia es que aquí un solo controlador agrupa
/// varias pantallas como acciones, en vez de una página con varios handlers.
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
/// <param name="logger">Logger del controlador.</param>
public class HomeController(IProductService service, ILogger<HomeController> logger) : Controller
{
    /// <summary>
    /// El catálogo completo. Responde a la ruta por convención "/".
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Index con su ViewModel.</returns>
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var productos = await service.GetAllAsync(cancellationToken);
        var resumen = await service.GetCatalogSummaryAsync(cancellationToken);

        logger.LogInformation("Catálogo servido con {Total} productos", productos.Count);

        // ViewData lleva los datos sueltos de la página: aquí el recuento y la hora de la consulta.
        // El ViewModel lleva lo que la vista necesita para pintar de verdad; ViewData es el cajón
        // de sastre de los avulsos. En Razor Pages existe ViewData, pero NO ViewBag: por eso la
        // demo de este ejemplo usa solo ViewData y se puede copiar tal cual en las dos arquitecturas.
        ViewData["Mensaje"] =
            $"Hay {productos.Count} productos en la tienda. Última consulta a las {DateTime.Now:HH:mm:ss}.";

        return View(new CatalogoViewModel(productos, resumen));
    }

    /// <summary>
    /// La ficha de un producto. Responde SOLO a "/producto/{id}".
    /// Lleva su propia plantilla de ruta con atributo, que es el equivalante en MVC
    /// a la línea @@page "{id:int}" de Razor Pages.
    /// </summary>
    /// <param name="id">Identificador que viene en la ruta; el ":int" la obliga a ser numérica.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Detalle, o 404 si el producto no existe.</returns>
    [Route("producto/{id:int}")]
    public async Task<IActionResult> Detalle(long id, CancellationToken cancellationToken = default)
    {
        var producto = await service.GetByIdAsync(id, cancellationToken);
        if (producto is null)
        {
            return NotFound();
        }

        var sugerencias = await service.GetRelatedAsync(id, 3, cancellationToken);
        return View(new DetalleViewModel(producto, sugerencias));
    }

    /// <summary>
    /// Los productos de una categoría. Responde a "/Home/Categoria?categoria=Audio".
    /// El parámetro llega de la cadena de consulta y el Model Binding lo convierte en el enum.
    /// </summary>
    /// <param name="categoria">Categoría pedida en la query string.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Categoria con su ViewModel.</returns>
    public async Task<IActionResult> Categoria(ProductCategory categoria, CancellationToken cancellationToken = default)
    {
        var productos = await service.GetByCategoryAsync(categoria, cancellationToken);
        return View(new CategoriaViewModel(categoria, productos));
    }

    /// <summary>
    /// Demostración de las directivas de Razor (@@inject, @@functions y @@section).
    /// Son las mismas en MVC que en Razor Pages: la vista se compila igual.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Directivas con su ViewModel.</returns>
    public async Task<IActionResult> Directivas(CancellationToken cancellationToken = default)
    {
        var resumen = await service.GetCatalogSummaryAsync(cancellationToken);
        return View(new DirectivasViewModel(resumen));
    }

    /// <summary>
    /// Demostración de las estructuras de control de C# dentro de una vista.
    /// El cálculo del stock total se hace aquí, no en la vista.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Estructuras con su ViewModel.</returns>
    public async Task<IActionResult> Estructuras(CancellationToken cancellationToken = default)
    {
        var productos = await service.GetAllAsync(cancellationToken);
        return View(new EstructurasViewModel(productos, productos.Sum(p => p.Stock)));
    }

    /// <summary>
    /// Demostración de funciones y delegados dentro de una vista.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>La vista Funciones con su ViewModel.</returns>
    public async Task<IActionResult> Funciones(CancellationToken cancellationToken = default)
    {
        var productos = await service.GetAllAsync(cancellationToken);
        var precioMaximo = productos.Count == 0 ? 0 : productos.Max(p => p.Precio);
        return View(new FuncionesViewModel(productos, precioMaximo));
    }

    /// <summary>
    /// La vista de error del sistema UseExceptionHandler.
    /// </summary>
    /// <returns>La vista Error con el detalle del fallo, si lo hubo.</returns>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Error()
    {
        var detalle = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        return View(new ErrorViewModel(detalle?.Path, detalle?.Error.Message));
    }
}
