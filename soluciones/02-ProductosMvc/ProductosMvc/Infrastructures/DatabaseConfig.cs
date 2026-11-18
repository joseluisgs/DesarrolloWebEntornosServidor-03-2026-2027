using Microsoft.EntityFrameworkCore;
using ProductosMvc.Data;
using ProductosMvc.Data.Seed;

namespace ProductosMvc.Infrastructures;

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
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Fail-fast: en producción, si falta la cadena de conexión, la app no debe arrancar
        // "funcionando" contra un fichero local que nadie sabe dónde está.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (environment.IsDevelopment())
            {
                connectionString = "Data Source=productos.db";
            }
            else
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection no está definida. " +
                    "En producción es obligatorio configurarla (appsettings.json, user-secrets o variables de entorno).");
            }
        }

        services.AddDbContext<ProductsDbContext>(options => options.UseSqlite(connectionString));

        // Semilla estilo TiendaAPI: una clase inyectable, resuelta al arrancar.
        services.AddScoped<ProductSeeder>();

        return services;
    }
}
