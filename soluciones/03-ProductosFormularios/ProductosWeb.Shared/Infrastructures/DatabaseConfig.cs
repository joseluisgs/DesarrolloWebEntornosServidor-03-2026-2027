using Microsoft.EntityFrameworkCore;
using ProductosWeb.Shared.Data;
using ProductosWeb.Shared.Data.Seed;

namespace ProductosWeb.Shared.Infrastructures;

/// <summary>
/// Configuración de la base de datos.
/// El DbContext y el seeder se registran aquí, no en Program.cs.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Registra SQLite y el seeder del catálogo.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app (appsettings + user-secrets + variables de entorno).</param>
    /// <param name="environment">Entorno actual; en producción no se admite la cadena por defecto.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=productos.db";

        // En producción la cadena por defecto sería un fichero compartido entre instancias:
        // mejor que la app pare a que arranque con una base de datos que no es la suya.
        if (!environment.IsDevelopment() && connectionString == "Data Source=productos.db")
        {
            throw new InvalidOperationException(
                "Falta la cadena de conexión. Configúrala con 'dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"'.");
        }

        services.AddDbContext<ProductsDbContext>(options => options.UseSqlite(connectionString));

        // El seeder es un servicio más: la resuelve la extensión de inicialización.
        services.AddScoped<ProductSeeder>();

        return services;
    }
}
