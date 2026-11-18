using ProductosMvc.Dtos;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista de la ficha de producto.
/// Fíjate en Sugerencias: el DTO no las trae, porque un producto no "tiene" relacionados.
/// Ese dato extra de presentación es exactamente para lo que sirve un ViewModel.
/// </summary>
/// <param name="Producto">Producto a mostrar.</param>
/// <param name="Sugerencias">Otros productos de la misma categoría.</param>
public record DetalleViewModel(
    ProductDto Producto,
    IReadOnlyList<ProductDto> Sugerencias);
