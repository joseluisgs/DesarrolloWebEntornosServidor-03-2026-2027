using ProductosWeb.Shared.Data;
using ProductosWeb.Shared.Data.Seed;

namespace ProductosWeb.Shared.Infrastructures;

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
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        if (isDevelopment)
        {
            await context.Database.EnsureDeletedAsync();
        }

        await context.Database.EnsureCreatedAsync();

        if (isDevelopment)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();
            await seeder.SeedAsync();
        }
    }
}
