using System.Globalization;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Mappers;

/// <summary>
/// Fábrica y descripción del formulario de Binding avanzado.
/// Las dos visiones del ejemplo usan estos mismos métodos, así que la página de Binding
/// empieza siempre con el mismo modelo y pinta siempre el mismo resumen.
/// </summary>
public static class BindingMapper
{
    /// <summary>
    /// Monta el modelo de partida de la página: una cabecera completa, una etiqueta y un recargo.
    /// Con índices ya visibles (Etiquetas[0], Recargos[0]) se entiende de un vistazo
    /// cómo se llaman los campos que el formulario va a enviar.
    /// </summary>
    /// <returns>Formulario de Binding listo para el GET.</returns>
    public static BindingFormViewModel CrearDeEjemplo() => new()
    {
        Cabecera = new ProductFormViewModel
        {
            Nombre = "Smartphone Aurora X5",
            Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
            Precio = 449.99m,
            Stock = 10,
            Categoria = ProductCategory.Smartphones
        },
        Etiquetas = ["oferta"],
        Recargos = [new RecargoViewModel { Concepto = "Envío urgente", Importe = 4.95m }]
    };

    /// <summary>
    /// Describe, línea a línea, todo lo que el model binding ha recogido del POST.
    /// Es la prueba visible de que los prefijos (Cabecera.Nombre) y los índices
    /// (Etiquetas[0], Recargos[1].Importe) han llegado completos hasta el servidor.
    /// </summary>
    /// <param name="model">Formulario recibido.</param>
    /// <returns>Texto multilínea con cada campo y su valor.</returns>
    public static string DescripcionBinding(this BindingFormViewModel model)
    {
        var lineas = new List<string>
        {
            $"Cabecera.Nombre = {model.Cabecera.Nombre}",
            $"Cabecera.Descripcion = {model.Cabecera.Descripcion}",
            $"Cabecera.Precio = {model.Cabecera.Precio.ToString(CultureInfo.CurrentCulture)}",
            $"Cabecera.Stock = {model.Cabecera.Stock}",
            $"Cabecera.Categoria = {model.Cabecera.Categoria}"
        };

        for (var i = 0; i < model.Etiquetas.Count; i++)
        {
            lineas.Add($"Etiquetas[{i}] = {model.Etiquetas[i]}");
        }

        for (var i = 0; i < model.Recargos.Count; i++)
        {
            lineas.Add($"Recargos[{i}].Concepto = {model.Recargos[i].Concepto}");
            lineas.Add($"Recargos[{i}].Importe = {model.Recargos[i].Importe.ToString(CultureInfo.CurrentCulture)}");
        }

        return string.Join(Environment.NewLine, lineas);
    }
}
