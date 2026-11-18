using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProductosRazor.Dtos;
using ProductosRazor.Models.Enums;
using ProductosRazor.Pages;
using ProductosRazor.Services;
using ProductosRazor.Test.Helpers;

namespace ProductosRazor.Test.PageModels;

/// <summary>
/// Tests del PageModel de la demostración de estructuras de control.
/// </summary>
[TestFixture]
public class EstructurasModelTests
{
    private Mock<IProductService> _servicio = null!;

    [SetUp]
    public void SetUp() => _servicio = new Mock<IProductService>();

    [Test]
    public async Task OnGetAsync_ConProductos_DejaLaListaYElStockTotal()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>
            {
                new(1, "Auriculares Aurora", "Auriculares.", 89.90m, 24,
                    ProductCategory.Audio, "/images/producto-sin-imagen.svg", true),
                new(2, "Portátil Vega", "Portátil.", 749.00m, 0,
                    ProductCategory.Laptops, "/images/producto-sin-imagen.svg", true)
            });

        var pageModel = PageModelHelper.Preparar(new EstructurasModel(_servicio.Object));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Productos.Should().HaveCount(2);
        pageModel.StockTotal.Should().Be(24);
    }

    [Test]
    public async Task OnGetAsync_SinProductos_DejaStockCero()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        var pageModel = PageModelHelper.Preparar(new EstructurasModel(_servicio.Object));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Productos.Should().BeEmpty();
        pageModel.StockTotal.Should().Be(0);
    }
}
