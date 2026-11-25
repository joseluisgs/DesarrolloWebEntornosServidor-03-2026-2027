using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ProductosWeb.Shared.Infrastructures;

// ============================================================
// 04-Productos Ficheros - visión MVC (puerto 5085)
// .NET 10 · C# 14 · SQLite local · sin servicios externos
//
// El foco de este ejemplo son los ficheros que trae un formulario:
// el campo IFormFile, su validación (extensión, tamaño y firma),
// el servicio de almacenamiento, el nombre único que decide el
// servidor, la descarga con Content-Disposition y el techo del
// endpoint. La capa de negocio vive en ProductosWeb.Shared y es
// la misma que usa la visión Razor Pages.
// ============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC con anti-forgery y el formulario multipart de siempre.
builder.Services.AddMvcControllers();

// 2) Capa de negocio, base de datos y validación: todo del proyecto compartido.
builder.Services
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddServices()
    .AddValidation()
    .AddAntiForgery();

// 3) Cultura por defecto: español de España, para que los decimales se pinten con coma.
var culturas = new[] { new CultureInfo("es-ES") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culturas[0]);
    options.SupportedCultures = culturas;
    options.SupportedUICultures = culturas;
});

var app = builder.Build();

// 4) La base de datos se crea y se siembra aquí, no en un migrador aparte.
await app.InitializeDatabaseAsync(app.Environment.IsDevelopment());

app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();

// 5) En MVC las vistas leen TempData: el proveedor por defecto usa cookies,
//    así que no hace falta registrar sesión alguna.
app.MapControllerRoute(
    name: "porDefecto",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.Run();
