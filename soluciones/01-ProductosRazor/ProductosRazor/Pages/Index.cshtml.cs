using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductosRazor.Dtos;
using ProductosRazor.Services;

namespace ProductosRazor.Pages;

/// <summary>
/// Catálogo de productos.
/// El PageModel solo hace una cosa: pedirle los datos al servicio y dejarlos en una
/// propiedad que la vista pueda leer. Ni consulta la base de datos ni monta HTML.
/// </summary>
/// <param name="service">Servicio del catálogo.</param>
/// <param name="logger">Logger de la página.</param>
public class IndexModel(IProductService service, ILogger<IndexModel> logger) : PageModel
{
    /// <summary>
    /// Productos del catálogo, ya convertidos a DTO.
    /// La vista itera esta propiedad; nunca ve la entidad Product.
    /// </summary>
    public IReadOnlyList<ProductDto> Productos { get; private set; } = Array.Empty<ProductDto>();

    /// <summary>
    /// Handler que responde al GET de la página. El nombre OnGetAsync no se elige:
    /// es el que busca el framework por el verbo HTTP.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    /// <returns>Tarea asíncrona.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        Productos = await service.GetAllAsync(cancellationToken);
        logger.LogInformation("Catálogo servido con {Total} productos", Productos.Count);

        // ViewData lleva los datos sueltos de la página: aquí el recuento y la hora de la consulta.
        // En Razor Pages existe ViewData, pero NO ViewBag (eso es exclusivo de los controladores
        // MVC); por eso esta demo se puede copiar tal cual en el ejemplo 02.
        ViewData["Mensaje"] =
            $"Hay {Productos.Count} productos en la tienda. Última consulta a las {DateTime.Now:HH:mm:ss}.";
    }
}
