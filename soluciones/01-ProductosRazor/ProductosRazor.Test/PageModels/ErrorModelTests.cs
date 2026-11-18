using FluentAssertions;
using NUnit.Framework;
using ProductosRazor.Pages;
using ProductosRazor.Test.Helpers;

namespace ProductosRazor.Test.PageModels;

/// <summary>
/// Tests del PageModel de la página de error.
/// Sin excepción registrada en el HttpContext, no hay nada que contar al usuario.
/// </summary>
[TestFixture]
public class ErrorModelTests
{
    [Test]
    public void OnGet_SinExcepcion_DejaLaRutaVacia()
    {
        // Arrange
        var pageModel = PageModelHelper.Preparar(new ErrorModel());

        // Act
        pageModel.OnGet();

        // Assert
        pageModel.Ruta.Should().BeNull();
        pageModel.Mensaje.Should().BeNull();
    }
}
