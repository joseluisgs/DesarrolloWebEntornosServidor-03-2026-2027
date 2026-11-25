using FluentAssertions;
using NUnit.Framework;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Test.Mappers;

/// <summary>
/// Tests del mapeo de formulario a entidad.
/// El binder de ASP.NET Core convierte los campos de texto vacíos en null (esa es la regla
/// ConvertEmptyStringToNull, activada por defecto), así que ToEntity recibe modelos a medias
/// y no se le puede permitir un nulo: un campo vacío en el alta provocaba un
/// NullReferenceException que el runtime señalaba en el new() del inicializador.
/// </summary>
[TestFixture]
public class ProductMapperTests
{
    [Test]
    public void ToEntity_CamposDeTextoANulo_DevuelveEntidadConCadenasVacias()
    {
        // Arrange: así llega un formulario enviado con Nombre y Descripcion vacíos.
        var formulario = new ProductFormViewModel
        {
            Nombre = null!,
            Descripcion = null!,
            Precio = 10m,
            Stock = 1,
            Anio = 2026
        };

        // Act
        var entidad = formulario.ToEntity();

        // Assert: sin excepción y con las cadenas listas para la base de datos.
        entidad.Nombre.Should().BeEmpty();
        entidad.Descripcion.Should().BeEmpty();
    }

    [Test]
    public void ToEntity_EtiquetasANulas_DevuelveListaVacia()
    {
        // Arrange: el formulario puede llegar sin ninguna fila de etiquetas.
        var formulario = new ProductFormViewModel
        {
            Nombre = "Auriculares Sonora Air",
            Descripcion = "Sonido cancelador de ruido y estuche rígido incluido.",
            Etiquetas = null!
        };

        // Act
        var entidad = formulario.ToEntity();

        // Assert
        entidad.Etiquetas.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToEntity_EtiquetasConFilasVacias_YConNulos_LosQuitaTodas()
    {
        // Arrange: el formulario pinta filas vacías y un elemento nulo no debería llegar,
        // pero si llega se trata como fila vacía.
        var formulario = new ProductFormViewModel
        {
            Nombre = "Cable USB-C reforzado",
            Descripcion = "Cable de 2 metros con trenzado de nylon.",
            Etiquetas = new List<string> { "", "  ", null!, "  oferta  ", null! }
        };

        // Act
        var entidad = formulario.ToEntity();

        // Assert: sólo queda la etiqueta con texto, además sin espacios sobrantes.
        entidad.Etiquetas.Should().ContainSingle().Which.Should().Be("oferta");
    }

    [Test]
    public void ToEntity_ModeloVacio_NoLanzaYNeviaEntidadPorDefecto()
    {
        // Arrange: modelo recién creado, como el que sale de un POST sin campos.
        var formulario = new ProductFormViewModel();

        // Act
        var entidad = formulario.ToEntity();

        // Assert
        entidad.Should().NotBeNull();
        entidad.Nombre.Should().BeEmpty();
        entidad.Etiquetas.Should().BeEmpty();
        entidad.Imagen.Should().BeNull();
    }
}
