using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NUnit.Framework;
using ProductosRazor.Dtos;
using ProductosRazor.Models.Enums;
using ProductosRazor.Pages;
using ProductosRazor.Services;
using ProductosRazor.Test.Helpers;

namespace ProductosRazor.Test.PageModels;

/// <summary>
/// Tests del PageModel de la ficha de producto.
/// </summary>
[TestFixture]
public class DetalleModelTests
{
    private static readonly ProductDto ProductoDePrueba = new(
        1, "Auriculares Aurora", "Auriculares con cancelación de ruido.", 89.90m, 24,
        ProductCategory.Audio, "/images/producto-sin-imagen.svg", true);

    private Mock<IProductService> _servicio = null!;

    [SetUp]
    public void SetUp() => _servicio = new Mock<IProductService>();

    [Test]
    public async Task OnGetAsync_IdExistente_DejaProductoYSugerencias()
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

        var pageModel = PageModelHelper.Preparar(new DetalleModel(_servicio.Object));

        // Act
        var resultado = await pageModel.OnGetAsync(1);

        // Assert
        resultado.Should().BeOfType<PageResult>();
        pageModel.Producto!.Nombre.Should().Be("Auriculares Aurora");
        pageModel.Sugerencias.Should().ContainSingle();
    }

    [Test]
    public async Task OnGetAsync_IdInexistente_Devuelve404()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(9999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        var pageModel = PageModelHelper.Preparar(new DetalleModel(_servicio.Object));

        // Act
        var resultado = await pageModel.OnGetAsync(9999);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
        pageModel.Producto.Should().BeNull();
    }
}
