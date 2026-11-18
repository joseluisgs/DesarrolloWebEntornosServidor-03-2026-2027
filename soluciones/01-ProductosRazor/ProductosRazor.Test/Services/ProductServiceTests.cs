using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProductosRazor.Data;
using ProductosRazor.Models;
using ProductosRazor.Models.Enums;
using ProductosRazor.Services;

namespace ProductosRazor.Test.Services;

/// <summary>
/// Tests del servicio del catálogo.
/// La base de datos es InMemory: cada test empieza con un esquema limpio y sin tocar ningún fichero.
/// </summary>
[TestFixture]
public class ProductServiceTests
{
    private ProductsDbContext _context = null!;
    private ProductService _service = null!;

    [SetUp]
    public void SetUp()
    {
        // Nombre aleatorio para que cada test tenga su propia base de datos en memoria
        var opciones = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ProductsDbContext(opciones);
        _context.Database.EnsureCreated();

        _service = new ProductService(_context, NullLogger<ProductService>.Instance);

        _context.Products.AddRange(
            new Product
            {
                Nombre = "Auriculares Aurora",
                Descripcion = "Auriculares con cancelación de ruido.",
                Precio = 89.90m,
                Stock = 24,
                Categoria = ProductCategory.Audio
            },
            new Product
            {
                Nombre = "Portátil Vega",
                Descripcion = "Portátil de 14 pulgadas.",
                Precio = 749.00m,
                Stock = 0,
                Categoria = ProductCategory.Laptops
            },
            new Product
            {
                Nombre = "Móvil Zafiro",
                Descripcion = "Smartphone de 6,5 pulgadas.",
                Precio = 429.00m,
                Stock = 5,
                Categoria = ProductCategory.Smartphones
            });

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task GetAllAsync_ConProductos_RetornaTodosOrdenadosPorNombre()
    {
        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Select(p => p.Nombre).Should().BeInAscendingOrder();
    }

    [Test]
    public async Task GetByIdAsync_IdExistente_RetornaElProducto()
    {
        // Arrange
        var esperado = _context.Products.Single(p => p.Nombre == "Portátil Vega");

        // Act
        var resultado = await _service.GetByIdAsync(esperado.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(esperado.Id);
        resultado.Precio.Should().Be(749.00m);
    }

    [Test]
    public async Task GetByIdAsync_IdInexistente_RetornaNull()
    {
        // Act
        var resultado = await _service.GetByIdAsync(9999);

        // Assert
        resultado.Should().BeNull();
    }

    [Test]
    public async Task GetByCategoryAsync_ConFiltro_RetornaSoloEsaCategoria()
    {
        // Act
        var resultado = await _service.GetByCategoryAsync(ProductCategory.Audio);

        // Assert
        resultado.Should().ContainSingle();
        resultado[0].Categoria.Should().Be(ProductCategory.Audio);
    }

    [Test]
    public async Task GetCatalogSummaryAsync_ConProductos_CuentaTotalesYValor()
    {
        // Act
        var resultado = await _service.GetCatalogSummaryAsync();

        // Assert
        resultado.TotalProductos.Should().Be(3);
        resultado.ValorInventario.Should().Be(24 * 89.90m + 0 * 749.00m + 5 * 429.00m);
        resultado.PorCategoria.Should().HaveCount(3);
    }

    [Test]
    public async Task GetRelatedAsync_ProductoExistente_RetornaHermanosDeSuCategoria()
    {
        // Arrange
        var origen = _context.Products.Single(p => p.Nombre == "Auriculares Aurora");

        // Act: añado un segundo producto de Audio para que haya hermanos que devolver
        _context.Products.Add(new Product
        {
            Nombre = "Altavoz Bocadillo",
            Descripcion = "Altavoz portátil.",
            Precio = 45.50m,
            Stock = 3,
            Categoria = ProductCategory.Audio
        });
        await _context.SaveChangesAsync();

        var resultado = await _service.GetRelatedAsync(origen.Id, 3);

        // Assert
        resultado.Should().ContainSingle();
        resultado[0].Nombre.Should().Be("Altavoz Bocadillo");
    }

    [Test]
    public async Task GetRelatedAsync_ProductoInexistente_RetornaVacio()
    {
        // Act
        var resultado = await _service.GetRelatedAsync(9999, 3);

        // Assert
        resultado.Should().BeEmpty();
    }
}
