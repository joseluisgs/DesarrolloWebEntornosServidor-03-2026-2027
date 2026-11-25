using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ProductosWeb.Shared.Data;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.Services;

namespace ProductosWeb.Test.Services;

/// <summary>
/// Tests del catálogo sobre una base de datos en memoria.
/// Cada test crea su propio contexto con un nombre de base de datos único,
/// así que no se pisan entre ellos y no hace falta fichero alguno.
/// </summary>
[TestFixture]
public class ProductServiceTests
{
    private ProductsDbContext _contexto = null!;
    private ProductService _servicio = null!;

    [SetUp]
    public void SetUp()
    {
        var opciones = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase($"productos-test-{Guid.NewGuid()}")
            .Options;

        _contexto = new ProductsDbContext(opciones);
        _servicio = new ProductService(_contexto, NullLogger<ProductService>.Instance);
    }

    [TearDown]
    public void TearDown() => _contexto.Dispose();

    /// <summary>
    /// Añade un producto de partida sin pasar por el servicio.
    /// </summary>
    /// <param name="nombre">Nombre del producto.</param>
    /// <returns>El producto añadido, con el Id asignado.</returns>
    private async Task<Product> SembrarAsync(string nombre)
    {
        var producto = new Product
        {
            Nombre = nombre,
            Descripcion = "Descripcion de prueba suficientemente larga.",
            Precio = 10.50m,
            Stock = 5,
            Categoria = ProductCategory.Audio
        };

        _contexto.Products.Add(producto);
        await _contexto.SaveChangesAsync();
        return producto;
    }

    [Test]
    public async Task Create_ProductoNuevo_DevuelveElDtoYLoGuarda()
    {
        // Arrange
        var producto = new Product
        {
            Nombre = "Auriculares Aurora",
            Descripcion = "Cancelación de ruido activa.",
            Precio = 89.90m,
            Stock = 24,
            Categoria = ProductCategory.Audio
        };

        // Act
        var creado = await _servicio.CreateAsync(producto);

        // Assert
        creado.Should().NotBeNull();
        creado!.Id.Should().BeGreaterThan(0);
        creado.Nombre.Should().Be("Auriculares Aurora");
        creado.Disponible.Should().BeTrue();

        var guardado = await _contexto.Products.AsNoTracking().SingleAsync();
        guardado.Nombre.Should().Be("Auriculares Aurora");
    }

    [Test]
    public async Task Create_NombreRepetido_DevuelveNull()
    {
        // Arrange: el índice único del modelo es la garantía; aquí se comprueba que el servicio lo traduce a null.
        await SembrarAsync("Auriculares Aurora");
        var repetido = new Product
        {
            Nombre = "Auriculares Aurora",
            Descripcion = "Otra descripción.",
            Precio = 5m,
            Stock = 1,
            Categoria = ProductCategory.Audio
        };

        // Act
        var resultado = await _servicio.CreateAsync(repetido);

        // Assert
        resultado.Should().BeNull();
        (await _contexto.Products.AsNoTracking().CountAsync()).Should().Be(1);
    }

    [Test]
    public async Task GetAll_OrdenaPorNombre()
    {
        // Arrange
        await SembrarAsync("Zurdo");
        await SembrarAsync("Alfa");

        // Act
        var productos = await _servicio.GetAllAsync();

        // Assert
        productos.Should().HaveCount(2);
        productos[0].Nombre.Should().Be("Alfa");
        productos[1].Nombre.Should().Be("Zurdo");
    }

    [Test]
    public async Task GetById_IdExistente_DevuelveElDto()
    {
        // Arrange
        var sembrado = await SembrarAsync("Auriculares Aurora");

        // Act
        var producto = await _servicio.GetByIdAsync(sembrado.Id);

        // Assert
        producto.Should().NotBeNull();
        producto!.Id.Should().Be(sembrado.Id);
        producto.Precio.Should().Be(10.50m);
    }

    [Test]
    public async Task GetById_IdInexistente_DevuelveNull()
    {
        // Act
        var producto = await _servicio.GetByIdAsync(9999);

        // Assert
        producto.Should().BeNull();
    }

    [Test]
    public async Task Update_ProductoExistente_CambiaLosCampos()
    {
        // Arrange
        var sembrado = await SembrarAsync("Nombre viejo");
        var cambios = new Product
        {
            Nombre = "Nombre nuevo",
            Descripcion = "Descripción nueva y más larga.",
            Precio = 99.99m,
            Stock = 7,
            Categoria = ProductCategory.Laptops
        };

        // Act
        var actualizado = await _servicio.UpdateAsync(sembrado.Id, cambios);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Nombre nuevo");
        actualizado.Precio.Should().Be(99.99m);
        actualizado.Categoria.Should().Be(ProductCategory.Laptops);

        var guardado = await _contexto.Products.AsNoTracking().SingleAsync();
        guardado.Nombre.Should().Be("Nombre nuevo");
    }

    [Test]
    public async Task Update_IdInexistente_DevuelveNull()
    {
        // Arrange
        var cambios = new Product { Nombre = "Nada", Descripcion = "Nada que ver aquí mismo.", Precio = 1, Stock = 1 };

        // Act
        var resultado = await _servicio.UpdateAsync(9999, cambios);

        // Assert
        resultado.Should().BeNull();
    }

    [Test]
    public async Task Delete_ProductoExistente_DevuelveTrueYLoQuita()
    {
        // Arrange
        var sembrado = await SembrarAsync("Auriculares Aurora");

        // Act
        var eliminado = await _servicio.DeleteAsync(sembrado.Id);

        // Assert
        eliminado.Should().BeTrue();
        (await _contexto.Products.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Test]
    public async Task Delete_IdInexistente_DevuelveFalse()
    {
        // Act
        var eliminado = await _servicio.DeleteAsync(9999);

        // Assert
        eliminado.Should().BeFalse();
    }

    [Test]
    public async Task Create_SinStock_SigueCreandoElProducto()
    {
        // Arrange: el stock a cero es un producto agotado, no un error de validación.
        var producto = new Product
        {
            Nombre = "Mando Pulse Gamer",
            Descripcion = "Mando inalámbrico con retroiluminación.",
            Precio = 59.95m,
            Stock = 0,
            Categoria = ProductCategory.Gaming
        };

        // Act
        var creado = await _servicio.CreateAsync(producto);

        // Assert
        creado.Should().NotBeNull();
        creado!.Disponible.Should().BeFalse();
    }
}
