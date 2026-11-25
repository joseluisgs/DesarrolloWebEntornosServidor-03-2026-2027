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
using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.Services.Storage;
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Pages;

/// <summary>
/// Tests del PageModel de alta de Razor Pages.
/// Se usa el validador REAL para comprobar que las dos capas de validación llegan al
/// ModelState, igual que en el controlador MVC: el comportamiento debe ser el mismo.
/// El almacenamiento se simula, igual que en el test del controlador.
/// </summary>
[TestFixture]
public class CrearModelTests
{
    private Mock<IProductService> _servicio = null!;
    private Mock<IStorageService> _almacen = null!;
    private CrearModel _pagina = null!;

    [SetUp]
    public void SetUp()
    {
        _servicio = new Mock<IProductService>();
        _almacen = new Mock<IStorageService>();
        _pagina = new CrearModel(_servicio.Object, _almacen.Object, new ProductFormValidator());
        Preparar(_pagina);
    }

    /// <summary>
    /// Monta el entorno mínimo que un PageModel necesita para devolver vistas y escribir TempData.
    /// PageModel.ViewData es de solo lectura (es PageContext.ViewData), por eso se asigna el contexto entero.
    /// </summary>
    /// <param name="pagina">PageModel a preparar.</param>
    private static void Preparar(PageModel pagina)
    {
        var contexto = new PageContext(new ActionContext(
            new DefaultHttpContext(), new RouteData(), new ActionDescriptor(),
            new ModelStateDictionary()));

        contexto.ViewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(), new ModelStateDictionary());

        pagina.PageContext = contexto;
        pagina.TempData = new TempDataDictionary(
            new DefaultHttpContext(), new Mock<ITempDataProvider>().Object);
    }

    /// <summary>
    /// Fórmula un formulario válido para partir de él en los tests de alta.
    /// </summary>
    /// <returns>Formulario que pasa todas las reglas.</returns>
    private static ProductFormViewModel FormularioValido() => new()
    {
        Nombre = "Smartphone Aurora X5",
        Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
        Precio = 449.99m,
        Stock = 10,
        Anio = 2026,
        Categoria = ProductCategory.Smartphones
    };

    [Test]
    public void OnGet_DejaElFormularioVacio()
    {
        // Act
        _pagina.OnGet();

        // Assert
        _pagina.Formulario.Should().NotBeNull();
        _pagina.Formulario.Nombre.Should().BeEmpty();
    }

    [Test]
    public async Task OnPost_FormularioValido_RedirigeAlCatalogoConTempData()
    {
        // Arrange
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Smartphone Aurora X5", "desc", 449.99m, 10, 2026,
                ProductCategory.Smartphones, true, "/images/producto-sin-imagen.svg", true));

        _pagina.Formulario = FormularioValido();

        // Act
        var resultado = await _pagina.OnPostAsync();

        // Assert: el patrón Post-Redirect-Get devuelve un redirect, no la propia página.
        var redirect = resultado.Should().BeOfType<RedirectToPageResult>().Subject;
        redirect.PageName.Should().Be("/Productos/Index");
        _pagina.TempData["Exito"].Should().BeOfType<string>()
            .Which.Should().Contain("creado correctamente");
    }

    [Test]
    public async Task OnPost_FormularioInvalido_DevuelveLaPaginaConLosErrores()
    {
        // Arrange
        _pagina.Formulario = FormularioValido();
        _pagina.Formulario.Nombre = "X";
        _pagina.Formulario.Descripcion = "corta";

        // Act
        var resultado = await _pagina.OnPostAsync();

        // Assert: devuelve la propia página, con el mismo modelo, para poder corregir.
        resultado.Should().BeOfType<PageResult>();
        _pagina.ModelState.IsValid.Should().BeFalse();
        _pagina.ModelState.ErrorCount.Should().BeGreaterThan(0);
        _pagina.ModelState["Nombre"]!.Errors.Should().NotBeEmpty();
        _pagina.ModelState["Descripcion"]!.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task OnPost_NombreRepetido_DevuelveLaPaginaConElAviso()
    {
        // Arrange
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        _pagina.Formulario = FormularioValido();

        // Act
        var resultado = await _pagina.OnPostAsync();

        // Assert
        resultado.Should().BeOfType<PageResult>();
        _pagina.ModelState["Nombre"]!.Errors
            .Should().Contain(e => e.ErrorMessage.Contains("Ya existe un producto con ese nombre"));
    }

    [Test]
    public async Task OnPost_PasaElFormularioAlServicioConElNombreLimpio()
    {
        // Arrange
        Product? capturado = null;
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => capturado = p)
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, 2026, ProductCategory.Audio, false, "i", true));

        _pagina.Formulario = FormularioValido();
        _pagina.Formulario.Nombre = "  Smartphone Aurora X5  ";

        // Act
        await _pagina.OnPostAsync();

        // Assert
        capturado.Should().NotBeNull();
        capturado!.Nombre.Should().Be("Smartphone Aurora X5");
    }
}
