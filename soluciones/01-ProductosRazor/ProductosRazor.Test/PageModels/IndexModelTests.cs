using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ProductosRazor.Dtos;
using ProductosRazor.Models.Enums;
using ProductosRazor.Pages;
using ProductosRazor.Services;
using ProductosRazor.Test.Helpers;

namespace ProductosRazor.Test.PageModels;

/// <summary>
/// Tests de un PageModel sin arrancar el servidor.
/// Se construye el PageModel a mano, se le ponen el PageContext y el TempData
/// que el framework le daría en una petición real, y se llama a OnGetAsync.
/// </summary>
[TestFixture]
public class IndexModelTests
{
    private Mock<IProductService> _servicio = null!;

    [SetUp]
    public void SetUp() => _servicio = new Mock<IProductService>();

    [Test]
    public async Task OnGetAsync_ConProductos_LosDejaListosParaLaVista()
    {
        // Arrange
        var productos = new List<ProductDto>
        {
            new(1, "Auriculares Aurora", "Auriculares con cancelación de ruido.", 89.90m, 24,
                ProductCategory.Audio, "/images/producto-sin-imagen.svg", true)
        };
        _servicio
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productos);

        var pageModel = PageModelHelper.Preparar(
            new IndexModel(_servicio.Object, NullLogger<IndexModel>.Instance));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Productos.Should().ContainSingle();
        pageModel.Productos[0].Nombre.Should().Be("Auriculares Aurora");
    }

    [Test]
    public async Task OnGetAsync_SinProductos_RetornaListaVacia()
    {
        // Arrange
        _servicio
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        var pageModel = PageModelHelper.Preparar(
            new IndexModel(_servicio.Object, NullLogger<IndexModel>.Instance));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Productos.Should().BeEmpty();
    }

    [Test]
    public async Task OnGetAsync_DejaElRecuentoEnViewData()
    {
        // Arrange
        _servicio
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>
            {
                new(1, "Auriculares Aurora", "Auriculares.", 89.90m, 24,
                    ProductCategory.Audio, "/images/producto-sin-imagen.svg", true)
            });

        var pageModel = PageModelHelper.Preparar(
            new IndexModel(_servicio.Object, NullLogger<IndexModel>.Instance));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.ViewData["Mensaje"].Should().BeOfType<string>()
            .Which.Should().StartWith("Hay 1 productos en la tienda.");
    }
}
