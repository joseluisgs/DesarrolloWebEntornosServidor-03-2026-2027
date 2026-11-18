using ProductosMvc.Dtos;
using ProductosMvc.Models.Enums;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista de una categoría concreta.
/// </summary>
/// <param name="Categoria">Categoría filtrada, para el título y la miga de pan.</param>
/// <param name="Productos">Productos de esa categoría.</param>
public record CategoriaViewModel(
    ProductCategory Categoria,
    IReadOnlyList<ProductDto> Productos);
