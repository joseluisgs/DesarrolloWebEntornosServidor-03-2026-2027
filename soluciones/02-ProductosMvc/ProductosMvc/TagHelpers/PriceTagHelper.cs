using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ProductosMvc.TagHelpers;

/// <summary>
/// Tag Helper propio: convierte <c>&lt;precio&gt;</c> en una etiqueta HTML formateada.
/// La regla del nombre decide el tag: a la clase se le quita el sufijo "TagHelper"
/// y el resto, en minúsculas y con guiones, es el elemento que la vista escribe.
/// PriceTagHelper → <c>&lt;precio&gt;</c>.
/// Se registra una sola vez en _ViewImports.cshtml con @addTagHelper *, ProductosMvc.
/// </summary>
[HtmlTargetElement("precio", Attributes = "importe")]
public class PriceTagHelper : TagHelper
{
    /// <summary>Importe a mostrar en euros.</summary>
    public decimal Importe { get; set; }

    /// <summary>Si es true, el precio se resalta con una clase extra.</summary>
    public bool Destacado { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // La cultura no se fija aquí: la trae la petición (el tema 22 la deja elegir al usuario).
        var texto = Importe.ToString("C", CultureInfo.CurrentCulture);

        output.TagName = "span";

        // Trampa clásica: si la vista escribe <precio ... />, el output llega con
        // TagMode.SelfClosing y el contenido que asignemos se descarta al escribir el HTML,
        // saliendo <span class="precio" />. Forzando la etiqueta de cierre se pinta el importe.
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.SetAttribute("class", Destacado ? "precio precio--destacado" : "precio");
        output.Content.SetContent(texto);
    }
}
