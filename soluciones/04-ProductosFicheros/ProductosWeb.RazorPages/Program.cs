using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ProductosWeb.Shared.Infrastructures;

// ============================================================
// 04-Productos Ficheros - visión Razor Pages (puerto 5086)
// .NET 10 · C# 14 · SQLite local · sin servicios externos
//
// Mismo ejemplo que la visión MVC, pero con PageModels en lugar
// de controladores. El IFormFile del formulario, el servicio de
// almacenamiento y la descarga con Content-Disposition son los
// mismos del proyecto compartido: lo único que cambia es dónde
// se engancha la lógica de cada vista.
// ============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// 1) Razor Pages con anti-forgery y el formulario multipart de siempre.
builder.Services.AddRazorPagesWeb();

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

// 5) En Razor Pages TempData funciona igual que en MVC: el proveedor por defecto
//    usa cookies, así que no hace falta registrar sesión alguna.
app.MapRazorPages();

app.Run();
