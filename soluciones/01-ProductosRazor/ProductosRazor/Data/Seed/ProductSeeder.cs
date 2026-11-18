using Microsoft.EntityFrameworkCore;
using ProductosRazor.Models;
using ProductosRazor.Models.Enums;

namespace ProductosRazor.Data.Seed;

/// <summary>
/// Semilla del catálogo de productos.
/// Es una clase inyectable con un <c>SeedAsync()</c>, igual que el SqlSeeder de la TiendaAPI,
/// y no un método estático: así puede usar el contexto, el logger y la configuración
/// como cualquier otro servicio, y se puede sustituir en los tests.
/// La resuelve DatabaseInitializationExtensions justo después de crear el esquema.
/// </summary>
/// <param name="context">Contexto de la base de datos.</param>
/// <param name="logger">Logger para dejar constancia de lo que se siembra.</param>
public class ProductSeeder(ProductsDbContext context, ILogger<ProductSeeder> logger)
{
    /// <summary>
    /// Siembra el catálogo solo si la tabla está vacía: recargar la app no duplica datos.
    /// </summary>
    /// <returns>Tarea asíncrona.</returns>
    public async Task SeedAsync()
    {
        if (await context.Products.AnyAsync())
        {
            logger.LogInformation("El catálogo ya tiene datos: se omite la siembra");
            return;
        }

        logger.LogInformation("Sembrando el catálogo de productos...");

        // Ojo con el índice único del nombre: dos productos no pueden llamarse igual.
        var productos = new List<Product>
        {
            new()
            {
                Nombre = "Auriculares Aurora BT",
                Descripcion = "Auriculares inalámbricos con cancelación de ruido activa y 30 horas de autonomía.",
                Precio = 89.90m,
                Stock = 24,
                Categoria = ProductCategory.Audio
            },
            new()
            {
                Nombre = "Altavoz Bocadillo Mini",
                Descripcion = "Altavoz portátil resistente al agua, con Bluetooth 5.3 y graves profundos.",
                Precio = 45.50m,
                Stock = 3,
                Categoria = ProductCategory.Audio
            },
            new()
            {
                Nombre = "Portátil Vega 14",
                Descripcion = "Portátil de 14 pulgadas con 16 GB de RAM, SSD de 512 GB y pantalla IPS.",
                Precio = 749.00m,
                Stock = 11,
                Categoria = ProductCategory.Laptops
            },
            new()
            {
                Nombre = "Portátil Vega 16 Pro",
                Descripcion = "Estación de trabajo móvil con tarjeta gráfica dedicada y 32 GB de RAM.",
                Precio = 1299.00m,
                Stock = 4,
                Categoria = ProductCategory.Laptops
            },
            new()
            {
                Nombre = "Móvil Zafiro 5G",
                Descripcion = "Smartphone de 6,5 pulgadas con cámara de 108 MP y batería de 5000 mAh.",
                Precio = 429.00m,
                Stock = 18,
                Categoria = ProductCategory.Smartphones
            },
            new()
            {
                Nombre = "Mando Pixelo Dual",
                Descripcion = "Mando inalámbrico con retroiluminación y batería de 40 horas.",
                Precio = 59.95m,
                Stock = 0,
                Categoria = ProductCategory.Gaming
            },
            new()
            {
                Nombre = "Teclado Mecánico Trueno",
                Descripcion = "Teclado mecánico compacto con switches lineales y teclas intercambiables.",
                Precio = 99.00m,
                Stock = 2,
                Categoria = ProductCategory.Gaming
            },
            new()
            {
                Nombre = "Cargador Viajero 65W",
                Descripcion = "Cargador de bolsillo con tres puertos USB-C y enchufe plegable.",
                Precio = 24.99m,
                Stock = 60,
                Categoria = ProductCategory.Accesorios
            }
        };

        await context.Products.AddRangeAsync(productos);
        await context.SaveChangesAsync();

        logger.LogInformation("Catálogo sembrado con {Total} productos", productos.Count);
    }
}
