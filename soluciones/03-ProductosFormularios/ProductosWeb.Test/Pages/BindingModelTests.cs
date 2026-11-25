using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using ProductosWeb.RazorPages.Pages.Productos;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Pages;

/// <summary>
/// Tests del PageModel de la página de Binding avanzado.
/// Se comprueba el mismo comportamiento que en la acción del controlador MVC:
/// añadir filas repinta sin validar y probar valida y escribe el resumen.
/// </summary>
[TestFixture]
public class BindingModelTests
{
    private BindingModel _pagina = null!;

    [SetUp]
    public void SetUp()
    {
        _pagina = new BindingModel(new BindingFormValidator());

        var contexto = new PageContext(new ActionContext(
            new DefaultHttpContext(), new RouteData(), new ActionDescriptor(),
            new ModelStateDictionary()));

        contexto.ViewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(), new ModelStateDictionary());

        _pagina.PageContext = contexto;
        _pagina.TempData = new TempDataDictionary(
            new DefaultHttpContext(), new Mock<ITempDataProvider>().Object);
    }

    [Test]
    public void OnGet_DejaElModeloDeEjemplo()
    {
        // Act
        _pagina.OnGet();

        // Assert
        _pagina.Formulario.Cabecera.Nombre.Should().Be("Smartphone Aurora X5");
        _pagina.Formulario.Etiquetas.Should().ContainSingle().Which.Should().Be("oferta");
        _pagina.Formulario.Recargos.Should().ContainSingle();
    }

    [Test]
    public async Task OnPost_Probar_ColeccionesRecogidas_EscribeElResumen()
    {
        // Arrange
        _pagina.Formulario = new BindingFormViewModel
        {
            Cabecera = new ProductFormViewModel
            {
                Nombre = "Smartphone Aurora X5",
                Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
                Precio = 12.75m,
                Stock = 3,
                Categoria = ProductCategory.Smartphones
            },
            Etiquetas = ["oferta", "nuevo"],
            Recargos = [new RecargoViewModel { Concepto = "Envío urgente", Importe = 4.95m }]
        };

        // Act
        var resultado = await _pagina.OnPostAsync("probar");

        // Assert
        resultado.Should().BeOfType<PageResult>();
        var resumen = _pagina.ViewData["Resumen"].Should().BeOfType<string>().Subject;
        resumen.Should().Contain("Cabecera.Nombre = Smartphone Aurora X5");
        resumen.Should().Contain("Etiquetas[1] = nuevo");
        resumen.Should().Contain("Recargos[0].Concepto = Envío urgente");
    }

    [Test]
    public async Task OnPost_Probar_CabeceraInvalida_DevuelveLaPaginaConLosErrores()
    {
        // Arrange
        _pagina.Formulario = new BindingFormViewModel
        {
            Cabecera = new ProductFormViewModel { Nombre = "X", Descripcion = "corta" }
        };

        // Act
        var resultado = await _pagina.OnPostAsync("probar");

        // Assert
        resultado.Should().BeOfType<PageResult>();
        _pagina.ModelState.IsValid.Should().BeFalse();
        _pagina.ModelState["Cabecera.Nombre"]!.Errors.Should().NotBeEmpty();
        _pagina.ViewData["Resumen"].Should().BeNull();
    }

    [Test]
    public async Task OnPost_AnadirRecargo_AnadeUnaFilaSinValidar()
    {
        // Arrange
        _pagina.Formulario = new BindingFormViewModel();

        // Act
        var resultado = await _pagina.OnPostAsync("anadir-recargo");

        // Assert
        resultado.Should().BeOfType<PageResult>();
        _pagina.Formulario.Recargos.Should().ContainSingle();
        _pagina.ModelState.IsValid.Should().BeTrue();
    }
}
