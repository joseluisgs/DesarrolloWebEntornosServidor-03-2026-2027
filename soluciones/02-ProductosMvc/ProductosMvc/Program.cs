using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ProductosMvc.Infrastructures;

// ============================================================
// 02-Productos MVC - el mismo catálogo del ejemplo 01, en MVC
// .NET 10 · C# 14 · SQLite local · sin servicios externos
//
// Cambia el motor de vista, no el negocio: Models, Dtos, Mappers,
// Data, Services e Infrastructures son los mismos del ejemplo 01.
// Lo nuevo es Controllers/ y Views/.
// ============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC: controladores + vistas. En Razor Pages era AddRazorPages().
//    Esto registra el enrutado por convención (controller/action/id),
//    la localización de vistas y los Tag Helpers del SDK.
builder.Services.AddControllersWithViews();

// 2) Cultura por defecto: español de España (el tema 22 la deja elegir al usuario).
var culturas = new[] { new CultureInfo("es-ES") };
builder.Services.Configure<RequestLocalizationOptions>(opciones =>
{
    opciones.DefaultRequestCulture = new RequestCulture("es-ES");
    opciones.SupportedCultures = culturas;
    opciones.SupportedUICultures = culturas;
});

// 3) Infraestructura: idéntica a la del ejemplo 01.
builder.Services
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddServices();

var app = builder.Build();

// Esquema y datos: en Development se recrea la BD y se siembra el catálogo.
await app.InitializeDatabaseAsync(app.Environment.IsDevelopment());

if (!app.Environment.IsDevelopment())
{
    // La vista de error vive en Views/Shared/Error.cshtml y se sirve desde Home/Error.
    // En Development un error sin controlar muestra la página del desarrollador.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();

// Ruta por convención: Index, Categoria y Error responden aquí.
// Detalle no: su acción lleva [Route("producto/{id:int}")] y solo escucha ahí.
app.MapControllerRoute(
    name: "porDefecto",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============================================================
// BANNER
// ============================================================
var puerto = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault() ?? "5082";
Console.WriteLine("========================================");
Console.WriteLine("  02-Productos MVC");
Console.WriteLine($"  Catálogo:      http://localhost:{puerto}/");
Console.WriteLine($"  Detalle:       http://localhost:{puerto}/producto/1");
Console.WriteLine($"  Categoría:     http://localhost:{puerto}/Home/Categoria?categoria=Audio");
Console.WriteLine($"  Directivas:    http://localhost:{puerto}/Home/Directivas");
Console.WriteLine($"  Estructuras:   http://localhost:{puerto}/Home/Estructuras");
Console.WriteLine($"  Funciones:     http://localhost:{puerto}/Home/Funciones");
Console.WriteLine($"  Error:         http://localhost:{puerto}/Home/Error");
Console.WriteLine($"  Entorno:       {app.Environment.EnvironmentName}");
Console.WriteLine("========================================");

app.Run();

// Necesario para los tests de integración con WebApplicationFactory.
public partial class Program { }
