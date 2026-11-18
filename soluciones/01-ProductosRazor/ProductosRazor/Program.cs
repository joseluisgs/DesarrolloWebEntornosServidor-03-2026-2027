using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ProductosRazor.Infrastructures;

// ============================================================
// 01-Productos Razor Pages - catálogo de productos
// .NET 10 · C# 14 · SQLite local · sin servicios externos
//
// El Program.cs es deliberadamente corto: todo lo que decide
// cómo se monta la app vive en las Config classes de Infrastructures/.
// ============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// 1) Motor de vistas: aquí SOLO hay Razor Pages, no hay MVC.
//    AddRazorPages registra el enrutado de páginas, los PageModels
//    y los Tag Helpers que trae el SDK.
builder.Services.AddRazorPages();

// 2) Cultura por defecto: español de España.
//    El tema 22 la convierte en una elección del usuario (cookie de idioma);
//    aquí solo fijamos la que se usa si nadie decide nada.
var culturas = new[] { new CultureInfo("es-ES") };
builder.Services.Configure<RequestLocalizationOptions>(opciones =>
{
    opciones.DefaultRequestCulture = new RequestCulture("es-ES");
    opciones.SupportedCultures = culturas;
    opciones.SupportedUICultures = culturas;
});

// 3) Infraestructura estilo Tienda: Config classes encadenables.
//    AddDatabase monta SQLite y registra el seeder;
//    AddServices registra la lógica de negocio.
builder.Services
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddServices();

var app = builder.Build();

// Esquema y datos: en Development se borra la BD, se recrea y se siembra.
// El detalle de la semilla vive en Data/Seed/ProductSeeder.cs.
await app.InitializeDatabaseAsync(app.Environment.IsDevelopment());

if (!app.Environment.IsDevelopment())
{
    // La página de error vive en Pages/Error.cshtml y la sirve el middleware.
    // En Development un error sin controlar muestra la página del desarrollador.
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Pipeline: estáticos (wwwroot), enrutado y localización.
// Caché, compresión, Identity o Docker llegan en los ejemplos 06, 08 y 11.
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();

app.MapRazorPages();

// ============================================================
// BANNER
// ============================================================
var puerto = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault() ?? "5081";
Console.WriteLine("========================================");
Console.WriteLine("  01-Productos Razor Pages");
Console.WriteLine($"  Catálogo:      http://localhost:{puerto}/");
Console.WriteLine($"  Detalle:       http://localhost:{puerto}/producto/1");
Console.WriteLine($"  Categoría:     http://localhost:{puerto}/Categoria?categoria=Audio");
Console.WriteLine($"  Directivas:    http://localhost:{puerto}/Directivas");
Console.WriteLine($"  Estructuras:   http://localhost:{puerto}/Estructuras");
Console.WriteLine($"  Funciones:     http://localhost:{puerto}/Funciones");
Console.WriteLine($"  Error:         http://localhost:{puerto}/Error");
Console.WriteLine($"  Entorno:       {app.Environment.EnvironmentName}");
Console.WriteLine("========================================");

app.Run();

// Necesario para los tests de integración con WebApplicationFactory.
public partial class Program { }
