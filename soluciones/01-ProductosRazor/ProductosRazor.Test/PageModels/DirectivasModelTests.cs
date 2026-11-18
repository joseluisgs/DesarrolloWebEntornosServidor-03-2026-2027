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
/// Tests de las páginas didácticas: sus PageModels hacen lo mismo que cualquier otro,
/// pedir datos al servicio y dejarlos listos para la vista.
/// </summary>
[TestFixture]
public class DirectivasModelTests
{
    [Test]
    public async Task OnGetAsync_DejaElResumenDelCatalogo()
    {
        // Arrange
        var resumen = new CatalogSummaryDto(3, 250.00m,
            new List<CategoryCountDto> { new("Audio", 1) });

        var servicio = new Mock<IProductService>();
        servicio.Setup(s => s.GetCatalogSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(resumen);

        var pageModel = PageModelHelper.Preparar(new DirectivasModel(servicio.Object));

        // Act
        await pageModel.OnGetAsync();

        // Assert
        pageModel.Resumen.Should().NotBeNull();
        pageModel.Resumen!.TotalProductos.Should().Be(3);
    }
}
