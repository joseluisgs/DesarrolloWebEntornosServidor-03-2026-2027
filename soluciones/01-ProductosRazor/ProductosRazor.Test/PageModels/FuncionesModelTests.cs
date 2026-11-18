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
/// Tests del PageModel de la demostración de funciones y delegados.
/// </summary>
[TestFixture]
public class FuncionesModelTests
{
    private Mock<IProductService> _servicio = null!;

    [SetUp]
    public void SetUp() => _servicio = new Mock<IProductService>();

    [Test]
    public async Task OnGetAsync_ConProductos_DejaElPrecioMaximo()
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

        var pageModel = PageModelHelper.Preparar(new FuncionesModel(_servicio.Object));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Productos.Should().HaveCount(2);
        pageModel.PrecioMaximo.Should().Be(749.00m);
    }

    [Test]
    public async Task OnGetAsync_SinProductos_DejaPrecioCero()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        var pageModel = PageModelHelper.Preparar(new FuncionesModel(_servicio.Object));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.PrecioMaximo.Should().Be(0);
    }
}
