using ProductosRazor.Data;
using ProductosRazor.Data.Seed;

namespace ProductosRazor.Infrastructures;

/// <summary>
/// Extensiones de inicialización de la base de datos.
/// El patrón es el de la TiendaAPI: una extensión de WebApplication que crea su propio
/// scope, comprueba el esquema y resuelve el seeder desde ahí.
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Crea (o recrea) la base de datos y la siembra si hace falta.
    /// Development: borra el fichero SQLite, lo recrea y siembra el catálogo, para que cada
    /// arranque empiece de cero con datos conocidos.
    /// Cualquier otro entorno: crea la tabla si no existe y no siembra nada.
    /// </summary>
    /// <param name="app">Aplicación construida.</param>
    /// <param name="isDevelopment">Si el entorno actual es Development.</param>
    /// <returns>Tarea asíncrona.</returns>
    public static async Task InitializeDatabaseAsync(this WebApplication app, bool isDevelopment)
    {
        // using var: el scope se cierra al salir del método, nunca antes.
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if (isDevelopment)
        {
            logger.LogWarning("Entorno de desarrollo: se elimina y recrea la base de datos SQLite");

            context.Database.EnsureDeleted();
            await context.Database.EnsureCreatedAsync();

            var seeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();
            await seeder.SeedAsync();

            return;
        }

        await context.Database.EnsureCreatedAsync();
    }
}
