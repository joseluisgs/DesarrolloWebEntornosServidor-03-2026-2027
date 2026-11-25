namespace ProductosWeb.Shared.Infrastructures;

/// <summary>
/// Configuración compartida de MVC y Razor Pages.
/// Las dos visiones del ejemplo usan los mismos avisos de anti-forgery: el comportamiento
/// del formulario es idéntico en ambas.
/// </summary>
public static class MvcConfig
{
    /// <summary>
    /// Configura los controladores MVC con el anti-forgery por defecto.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddMvcControllers(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        return services;
    }

    /// <summary>
    /// Configura Razor Pages con el mismo anti-forgery que la visión MVC.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddRazorPagesWeb(this IServiceCollection services)
    {
        services.AddRazorPages();
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
