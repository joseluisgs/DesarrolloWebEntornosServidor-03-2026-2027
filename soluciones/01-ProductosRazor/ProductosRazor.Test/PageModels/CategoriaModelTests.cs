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
/// Tests del PageModel de la página de categoría.
/// </summary>
[TestFixture]
public class CategoriaModelTests
{
    private Mock<IProductService> _servicio = null!;

    [SetUp]
    public void SetUp() => _servicio = new Mock<IProductService>();

    [Test]
    public async Task OnGetAsync_ConCategoria_DejaElFiltroYLosProductos()
    {
        // Arrange
        var audio = new List<ProductDto>
        {
            new(1, "Auriculares Aurora", "Auriculares.", 89.90m, 24,
                ProductCategory.Audio, "/images/producto-sin-imagen.svg", true)
        };
        _servicio.Setup(s => s.GetByCategoryAsync(ProductCategory.Audio, It.IsAny<CancellationToken>()))
            .ReturnsAsync(audio);

        var pageModel = PageModelHelper.Preparar(new CategoriaModel(_servicio.Object));

        // Act
        var resultado = await pageModel.OnGetAsync(ProductCategory.Audio);

        // Assert
        resultado.Should().BeOfType<PageResult>();
        pageModel.Categoria.Should().Be(ProductCategory.Audio);
        pageModel.Productos.Should().ContainSingle();
    }

    [Test]
    public async Task OnGetAsync_SinProductos_DejaListaVacia()
    {
        // Arrange
        _servicio.Setup(s => s.GetByCategoryAsync(ProductCategory.Gaming, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());

        var pageModel = PageModelHelper.Preparar(new CategoriaModel(_servicio.Object));

        // Act
        var resultado = await pageModel.OnGetAsync(ProductCategory.Gaming);

        // Assert
        resultado.Should().BeOfType<PageResult>();
        pageModel.Productos.Should().BeEmpty();
    }
}
