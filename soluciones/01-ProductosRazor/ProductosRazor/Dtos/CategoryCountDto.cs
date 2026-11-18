namespace ProductosRazor.Dtos;

/// <summary>
/// Cuántos productos hay en una categoría concreta del catálogo.
/// </summary>
/// <param name="Categoria">Nombre de la categoría.</param>
/// <param name="Total">Número de productos en esa categoría.</param>
public record CategoryCountDto(string Categoria, int Total);
