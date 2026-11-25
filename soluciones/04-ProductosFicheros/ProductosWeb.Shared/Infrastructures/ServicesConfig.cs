using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.Services.Storage;

namespace ProductosWeb.Shared.Infrastructures;

/// <summary>
/// Configuración de servicios de negocio.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios del catálogo en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStorageService, StorageService>();
        return services;
    }
}
