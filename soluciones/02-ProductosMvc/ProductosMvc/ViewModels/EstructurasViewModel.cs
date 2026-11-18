using ProductosMvc.Dtos;

namespace ProductosMvc.ViewModels;

/// <summary>
/// Lo que necesita la vista de las estructuras de control.
/// La vista solo recorre y pinta: el stock total se calcula aquí, en el controlador,
/// no en un @@foreach con una suma dentro.
/// </summary>
/// <param name="Productos">Productos del catálogo, para recorrerlos desde la vista.</param>
/// <param name="StockTotal">Suma de todas las unidades en almacén.</param>
public record EstructurasViewModel(
    IReadOnlyList<ProductDto> Productos,
    int StockTotal);
