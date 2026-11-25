using FluentValidation;
using ProductosWeb.Shared.Validators;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Infrastructures;

/// <summary>
/// Configuración de la validación.
/// FluentValidation se registra a mano (sin el paquete FluentValidation.AspNetCore,
/// que está sin soporte oficial): los validadores se resuelven como servicios cualquiera
/// y cada acción decide cuándo invocarlos.
/// </summary>
public static class ValidationConfig
{
    /// <summary>
    /// Registra los validadores del ejemplo en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ProductFormViewModel>, ProductFormValidator>();
        return services;
    }
}
