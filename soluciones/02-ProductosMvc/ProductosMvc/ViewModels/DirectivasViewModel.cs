using ProductosMvc.Dtos;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista de la demostración de directivas.
/// En Razor Pages estos mismos datos viven como propiedades del PageModel;
/// en MVC viajan en un ViewModel, que es la forma idiomática del controlador.
/// </summary>
/// <param name="Resumen">Resumen del catálogo, para comparar con el servicio inyectado en la vista.</param>
public record DirectivasViewModel(CatalogSummaryDto? Resumen);
