using Microsoft.Extensions.DependencyInjection;
using ProductosWeb.Shared.Binders;

namespace ProductosWeb.Shared.Infrastructures;

/// <summary>
/// Configuración compartida de MVC y Razor Pages.
/// Las dos visiones del ejemplo usan el mismo binder de decimales y los mismos avisos de ModelState:
/// el comportamiento del formulario es idéntico en ambas.
/// </summary>
public static class MvcConfig
{
    /// <summary>
    /// Configura los controladores MVC con el binder de decimales y el anti-forgery por defecto.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddMvcControllers(this IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
        {
            // En primera posición: gana al binder de decimales de por defecto, que solo entiende el punto.
            options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
        });

        return services;
    }

    /// <summary>
    /// Configura Razor Pages con el mismo binder de decimales que la visión MVC.
    /// El registro no va en RazorPagesOptions (esa opción solo admite convenciones de página):
    /// va en la configuración de MVC que Razor Pages usa por debajo, con AddMvcOptions.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddRazorPagesWeb(this IServiceCollection services)
    {
        services.AddRazorPages()
            .AddMvcOptions(options =>
            {
                options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
            });

        return services;
    }

    /// <summary>
    /// Ajustes comunes del anti-forgery: la vista emite el token con el form Tag Helper
    /// y los POST lo exigen con ValidateAntiForgeryToken.
    /// No se toca HeaderName: ese ajuste es para llamadas AJAX que mandan el token en una
    /// cabecera; los formularios de este ejemplo usan el campo oculto de siempre.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddAntiForgery(this IServiceCollection services)
    {
        services.AddAntiforgery();
        return services;
    }
}
