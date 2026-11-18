# 01 - Productos Razor Pages

Catálogo de productos hecho con **Razor Pages** en ASP.NET Core (.NET 10). El ejemplo no enseña a montar una tienda: enseña **todo lo que hace falta para que una vista Razor funcione de verdad**, página a página, sobre un dominio real (`Product`) y con SQLite local. `dotnet run` es suficiente: no necesita Docker, ni Postgres, ni conexión a internet.

## Descripción

Razor Pages parece una carpeta de `.cshtml` y un PageModel. Eso es la mitad: la otra mitad son las piezas que deciden cómo se comportan todas las páginas a la vez (`_Layout`, `_ViewStart`, `_ViewImports`), las que reutilizan trozos de vista (parciales, componentes de vista) y las que meten lógica de servidor en el HTML (Tag Helpers propios). Este ejemplo las tiene todas, funcionando y con su página de ejemplo.

| Pieza | Dónde vive |
|-------|-----------|
| Modelo y enum | `Models/Product.cs`, `Models/Enums/ProductCategory.cs` |
| DTOs | `Dtos/` (el servicio nunca devuelve entidades) |
| Mapeo | `Mappers/ProductMapper.cs` |
| Datos | `Data/ProductsDbContext.cs` + `Data/Seed/ProductSeeder.cs` |
| Negocio | `Services/IProductService.cs` + `Services/ProductService.cs` |
| Infraestructura | `Infrastructures/` (`DatabaseConfig`, `DatabaseInitializationExtensions`, `ServicesConfig`) |
| Vistas | `Pages/` (7 páginas), `Pages/Shared/` (layout y parcial), `ViewComponents/`, `TagHelpers/` |

## Las siete páginas

Las mismas siete que el ejemplo 02, con el mismo contenido en cada una: son espejos.

| Página | URL | Qué demuestra |
|--------|-----|---------------|
| `Index` | `/` | `if`, `foreach`, **parcial** `_TarjetaProducto`, **componente de vista** `<vc:catalog-summary />`, **Tag Helper propio** `<precio>` y `ViewData` con el recuento y la hora |
| `Detalle` | `/producto/{id}` | Plantilla de ruta con `@page "~/producto/{id:int}"`, `NotFound()`, `switch` dentro del HTML y la sección de sugerencias |
| `Categoria` | `/Categoria?categoria=Audio` | Model Binding de un enum desde la query string, sin ningún `Parse` en el handler |
| `Directivas` | `/Directivas` | Tabla de directivas, `@inject` de dos servicios, `@functions` y `@section Scripts` |
| `Estructuras` | `/Estructuras` | `if / else if / else`, `switch` con patrón relacional, `foreach`, `for` y `while` |
| `Funciones` | `/Funciones` | Delegados `Func<>` en el bloque de código y métodos en `@functions` |
| `Error` | `/Error` | La página que sirve `UseExceptionHandler`, con la ruta que falló |

## Las tres piezas compartidas

- **`Pages/Shared/_Layout.cshtml`** — la plantilla: cabecera con el menú (catálogo, las tres demos y las cinco categorías), `@RenderBody()` y `@await RenderSectionAsync("Scripts", required: false)`.
- **`Pages/_ViewStart.cshtml`** — elige ese layout para todas las páginas, para no repetirlo en cada fichero.
- **`Pages/_ViewImports.cshtml`** — namespaces y los dos `@addTagHelper`: los del SDK (`Microsoft.AspNetCore.Mvc.TagHelpers`) y los propios (`ProductosRazor`).

## El pie de página

El layout cierra con el pie de la asignatura, idéntico al del ejemplo 02:

```html
<footer class="pie">
    <p>J.L. González · Desarrollo Web en Entornos Servidor (DAW) · IES Luis Vives</p>
</footer>
```

## El Tag Helper propio

```csharp
[HtmlTargetElement("precio", Attributes = "importe")]
public class PriceTagHelper : TagHelper
```

La regla del nombre decide el tag: a la clase se le quita el sufijo `TagHelper` y el resto, en minúsculas, es el elemento que la vista escribe. La vista pinta `<precio importe="89.90" destacado="true" />` y el navegador ve un `<span class="precio precio--destacado">89,90 €</span>` con el importe formateado según la cultura de la petición.

## La semilla

El patrón es el de la TiendaAPI: `ProductSeeder` es una **clase inyectable** con `SeedAsync()`, registrada en `DatabaseConfig` y resuelta desde un scope en `DatabaseInitializationExtensions`. No es un método estático, así que puede usar el contexto, el logger y la configuración como cualquier otro servicio, y se puede sustituir en los tests.

En Development la app borra el fichero `productos.db`, lo recrea con `EnsureCreated()` y siembra 8 productos (hay uno agotado y dos con pocas unidades, para que las etiquetas de stock se vean). En cualquier otro entorno solo crea la tabla si no existe y no siembra nada.

## Cómo ejecutarlo

```bash
dotnet restore
dotnet build
dotnet run --project ProductosRazor
```

Abre `http://localhost:5081/`. Para borrar los datos y volver a sembrar, para la app y vuelve a arrancarla, o borra `productos.db`.

Los secretos (por ejemplo, otra cadena de conexión) van en user-secrets, no en el `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=productos.db" --project ProductosRazor
```

## Tests

```bash
dotnet test
```

Veinte pruebas: siete del servicio (con EF Core **InMemory**, sin ficheros) y trece de los siete PageModels, construidos a mano con un `PageContext` y un `TempData` simulados, sin arrancar el servidor. Este ejemplo no es el de pruebas a fondo: eso es el 10.

## Verificación del espejo con el 02

Los dos ejemplos se comprueban con la misma lista de aserciones sobre las siete pantallas (tarjetas, `ViewData`, pie, menú, `switch` del detalle, 404, secciones de las demos y CSS). El estado tras la última revisión: build **0/0** en ambos, **20/20** tests en ambos y **44/44** comprobaciones de humo en cada uno.

Lo que cambia entre los dos es solo el mecanismo de arquitectura:

| Tema | Aquí (Razor Pages) | Allí (MVC) |
|------|--------------------|------------|
| Quién recibe la petición | El PageModel de cada página | Las acciones de `HomeController` |
| Qué le pasa a la vista | Propiedades del PageModel | Un ViewModel con `return View(modelo)` |
| URL de detalle | `@page "~/producto/{id:int}"` | `[Route("producto/{id:int}")]` |
| Demo de datos sueltos | `ViewData` en el PageModel | `ViewData` en la acción (no `ViewBag`: es exclusivo de MVC) |

Los cuerpos de las vistas didácticas (`Directivas`, `Estructuras`, `Funciones`, `Categoria`, `Detalle`, `Error`) son idénticos byte a byte salvo la línea `@model`, y el CSS es el mismo fichero en los dos proyectos.

## Errores frecuentes al tocarlo

| Error | Causa |
|-------|-------|
| `The layout '_Layout' was not found` | Falta `_ViewStart.cshtml` o se movió el layout fuera de `Pages/Shared/` |
| `The partial '...' was not found` | El nombre del `<partial>` no coincide con el del fichero `.cshtml` |
| `Tag helper 'precio' was not found` | Falta `@addTagHelper *, ProductosRazor` en `_ViewImports.cshtml` |
| El Tag Helper sale vacío: `<span class="precio" />` | La vista escribe `<precio ... />` y el output llega con `TagMode.SelfClosing`, que descarta el contenido asignado. Hay que poner `output.TagMode = TagMode.StartTagAndEndTag` |
| `/producto/1` devuelve 404 | La plantilla de ruta se quedó sin el ancla: `@page "{id:int}"` cuelga de la carpeta del fichero y la URL correcta sería `/Detalle/producto/1`; hace falta `@page "~/producto/{id:int}"` |
| `SQLite Error 19: UNIQUE constraint failed` | Dos productos con el mismo nombre: hay un índice único sobre `Nombre` |
| El precio se pinta con `$` y coma | La cultura de la petición no es española; el tema 22 la deja elegir al usuario |

## Qué mirar después

- **02-ProductosMvc** — el mismo catálogo de técnicas en arquitectura MVC, para comparar las dos visiones.
- **03-ProductosFormularios** — alta de productos con binding, validación y anti-forgery.
- **10-ProductosTesting** — las pruebas a fondo de las dos visiones.
