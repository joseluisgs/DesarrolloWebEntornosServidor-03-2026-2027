using ProductosMvc.Dtos;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista de funciones y delegados.
/// </summary>
/// <param name="Productos">Productos del catálogo.</param>
/// <param name="PrecioMaximo">Precio más alto del catálogo, para demostrar los delegados.</param>
public record FuncionesViewModel(
    IReadOnlyList<ProductDto> Productos,
    decimal PrecioMaximo);
