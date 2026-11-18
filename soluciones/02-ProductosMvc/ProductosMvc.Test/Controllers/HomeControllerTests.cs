using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ProductosMvc.Controllers;
using ProductosMvc.Dtos;
using ProductosMvc.Models.Enums;
using ProductosMvc.Services;
using ProductosMvc.ViewModels;

namespace ProductosMvc.Test.Controllers;

/// <summary>
/// Tests del controlador del catálogo.
/// El controlador se construye a mano con un servicio simulado: no hace falta
/// arrancar el servidor para comprobar qué vista y qué ViewModel devuelve cada acción.
/// </summary>
[TestFixture]
public class HomeControllerTests
{
    private static readonly ProductDto ProductoDePrueba = new(
        1, "Auriculares Aurora", "Auriculares con cancelación de ruido.", 89.90m, 24,
        ProductCategory.Audio, "/images/producto-sin-imagen.svg", true);

    private Mock<IProductService> _servicio = null!;
    private HomeController _controlador = null!;

    [SetUp]
    public void SetUp()
    {
        _servicio = new Mock<IProductService>();

        _controlador = new HomeController(_servicio.Object, NullLogger<HomeController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [TearDown]
    public void TearDown() => _controlador?.Dispose();

    [Test]
    public async Task Index_ConProductos_DevuelveVistaConElCatalogo()
    {
        // Arrange
        var productos = new List<ProductDto> { ProductoDePrueba };
        var resumen = new CatalogSummaryDto(1, 89.90m * 24,
            new List<CategoryCountDto> { new("Audio", 1) });

        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(productos);
        _servicio.Setup(s => s.GetCatalogSummaryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(resumen);

        // Act
        var resultado = await _controlador.Index();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<CatalogoViewModel>().Subject;
        modelo.Productos.Should().ContainSingle();
        modelo.Resumen.TotalProductos.Should().Be(1);
    }

    [Test]
    public async Task Index_DejaElRecuentoEnViewData()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());
        _servicio.Setup(s => s.GetCatalogSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CatalogSummaryDto(0, 0, new List<CategoryCountDto>()));

        // Act
        await _controlador.Index();

        // Assert
        _controlador.ViewData["Mensaje"].Should().BeOfType<string>()
            .Which.Should().StartWith("Hay 0 productos en la tienda.");
    }

    [Test]
    public async Task Index_SinProductos_DevuelveVistaConLaListaVacia()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());
        _servicio.Setup(s => s.GetCatalogSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CatalogSummaryDto(0, 0, new List<CategoryCountDto>()));

        // Act
        var resultado = await _controlador.Index();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<CatalogoViewModel>().Subject;
        modelo.Productos.Should().BeEmpty();
    }

    [Test]
    public async Task Detalle_IdExistente_DevuelveVistaConSugerencias()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ProductoDePrueba);

        var sugerencias = new List<ProductDto>
        {
            new(2, "Altavoz Bocadillo", "Altavoz portátil.", 45.50m, 3,
                ProductCategory.Audio, "/images/producto-sin-imagen.svg", true)
        };
        _servicio.Setup(s => s.GetRelatedAsync(1, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sugerencias);

        // Act
        var resultado = await _controlador.Detalle(1);

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<DetalleViewModel>().Subject;
        modelo.Producto.Nombre.Should().Be("Auriculares Aurora");
        modelo.Sugerencias.Should().ContainSingle();
    }

    [Test]
    public async Task Detalle_IdInexistente_Devuelve404()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(9999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var resultado = await _controlador.Detalle(9999);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Categoria_ConFiltro_DevuelveSoloEsaCategoria()
    {
        // Arrange
        var audio = new List<ProductDto> { ProductoDePrueba };
        _servicio.Setup(s => s.GetByCategoryAsync(ProductCategory.Audio, It.IsAny<CancellationToken>()))
            .ReturnsAsync(audio);

        // Act
        var resultado = await _controlador.Categoria(ProductCategory.Audio);

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<CategoriaViewModel>().Subject;
        modelo.Categoria.Should().Be(ProductCategory.Audio);
        modelo.Productos.Should().ContainSingle();
    }

    [Test]
    public async Task Categoria_SinProductos_DevuelveVistaVacia()
    {
        // Arrange
        _servicio.Setup(s => s.GetByCategoryAsync(ProductCategory.Gaming, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        // Act
        var resultado = await _controlador.Categoria(ProductCategory.Gaming);

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<CategoriaViewModel>().Subject;
        modelo.Productos.Should().BeEmpty();
    }

    [Test]
    public async Task Directivas_DejaElResumenDelCatalogo()
    {
        // Arrange
        var resumen = new CatalogSummaryDto(3, 250.00m,
            new List<CategoryCountDto> { new("Audio", 1) });
        _servicio.Setup(s => s.GetCatalogSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(resumen);

        // Act
        var resultado = await _controlador.Directivas();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<DirectivasViewModel>().Subject;
        modelo.Resumen!.TotalProductos.Should().Be(3);
    }

    [Test]
    public async Task Estructuras_ConProductos_DejaLaListaYElStockTotal()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>
            {
                ProductoDePrueba,
                new(2, "Portátil Vega", "Portátil.", 749.00m, 0,
                    ProductCategory.Laptops, "/images/producto-sin-imagen.svg", true)
            });

        // Act
        var resultado = await _controlador.Estructuras();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<EstructurasViewModel>().Subject;
        modelo.Productos.Should().HaveCount(2);
        modelo.StockTotal.Should().Be(24);
    }

    [Test]
    public async Task Estructuras_SinProductos_DejaStockCero()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        // Act
        var resultado = await _controlador.Estructuras();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<EstructurasViewModel>().Subject;
        modelo.StockTotal.Should().Be(0);
    }

    [Test]
    public async Task Funciones_ConProductos_DejaElPrecioMaximo()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>
            {
                ProductoDePrueba,
                new(2, "Portátil Vega", "Portátil.", 749.00m, 0,
                    ProductCategory.Laptops, "/images/producto-sin-imagen.svg", true)
            });

        // Act
        var resultado = await _controlador.Funciones();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<FuncionesViewModel>().Subject;
        modelo.PrecioMaximo.Should().Be(749.00m);
    }

    [Test]
    public async Task Funciones_SinProductos_DejaPrecioCero()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        // Act
        var resultado = await _controlador.Funciones();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<FuncionesViewModel>().Subject;
        modelo.PrecioMaximo.Should().Be(0);
    }

    [Test]
    public void Error_SinExcepcion_DevuelveVistaSinRuta()
    {
        // Act
        var resultado = _controlador.Error();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<ErrorViewModel>().Subject;
        modelo.Ruta.Should().BeNull();
    }
}
