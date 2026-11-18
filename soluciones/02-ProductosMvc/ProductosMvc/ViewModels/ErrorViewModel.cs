namespace ProductosMvc.ViewModels;

/// <summary>
/// Datos de la vista de error.
/// </summary>
/// <param name="Ruta">URL que produjo el error, si la hubo.</param>
/// <param name="Mensaje">Mensaje de la excepción; en producción conviene no mostrarlo al usuario.</param>
public record ErrorViewModel(string? Ruta, string? Mensaje);
