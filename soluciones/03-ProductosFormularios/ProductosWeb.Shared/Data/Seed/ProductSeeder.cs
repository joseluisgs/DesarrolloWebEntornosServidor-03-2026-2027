using Microsoft.EntityFrameworkCore;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Models.Enums;

namespace ProductosWeb.Shared.Data.Seed;

/// <summary>
/// Semilla del catálogo de productos.
/// Es una clase inyectable con un <c>SeedAsync()</c> y no un método estático: así puede usar
/// el contexto y el logger como cualquier otro servicio, y se puede sustituir en los tests.
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
            return;
        }

        logger.LogInformation("Siembrando el catálogo de productos de ejemplo");

        context.Products.AddRange(
            new Product
            {
                Nombre = "Smartphone Aurora X5",
                Descripcion = "Pantalla AMOLED de 6,5 pulgadas, 128 GB de almacenamiento y batería de 5000 mAh.",
                Precio = 449.99m,
                Stock = 25,
                Categoria = ProductCategory.Smartphones,
                Imagen = "/images/producto-sin-imagen.svg"
            },
            new Product
            {
                Nombre = "Portátil Nimbus Pro 14",
                Descripcion = "Procesador de 8 núcleos, 16 GB de RAM y SSD de 512 GB para trabajo y estudio.",
                Precio = 999.00m,
                Stock = 10,
                Categoria = ProductCategory.Laptops,
                Imagen = "/images/producto-sin-imagen.svg"
            },
            new Product
            {
                Nombre = "Auriculares Sonora Air",
                Descripcion = "Cancelación activa de ruido, 30 horas de autonomía y Bluetooth 5.3.",
                Precio = 129.50m,
                Stock = 60,
                Categoria = ProductCategory.Audio,
                Imagen = "/images/producto-sin-imagen.svg"
            },
            new Product
            {
                Nombre = "Mando Pulse Gamer",
                Descripcion = "Mando inalámbrico con retroiluminación y dos modos de vibración.",
                Precio = 59.95m,
                Stock = 0,
                Categoria = ProductCategory.Gaming,
                Imagen = "/images/producto-sin-imagen.svg"
            },
            new Product
            {
                Nombre = "Cable USB-C reforzado",
                Descripcion = "Cable de carga rápida de 2 metros con funda de nailon trenzado.",
                Precio = 12.99m,
                Stock = 200,
                Categoria = ProductCategory.Accesorios,
                Imagen = "/images/producto-sin-imagen.svg"
            });

        await context.SaveChangesAsync();
        logger.LogInformation("Catálogo sembrado con 5 productos");
    }
}
