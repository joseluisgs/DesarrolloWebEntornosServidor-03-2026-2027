# 02 - Productos MVC

El mismo catálogo del ejemplo 01, hecho con **MVC** en ASP.NET Core (.NET 10). El negocio no cambia en absoluto: `Models`, `Dtos`, `Mappers`, `Data`, `Services` e `Infrastructures` son los mismos ficheros. Lo que cambia es el motor de vista, y ese es justo el objetivo de este ejemplo: ver **la misma técnica en las dos arquitecturas** y quedarse con la diferencia.

## Descripción

En Razor Pages cada pantalla es una carpeta con su `.cshtml` y su PageModel. En MVC cada pantalla es una **acción** de un controlador, y la vista vive en `Views/`, separada de la lógica. Las dos arquitecturas usan las mismas piezas de presentación (parciales, componentes de vista, Tag Helpers, `_ViewStart`, `_ViewImports`); cambia de dónde se invocan y quién responde a la petición.

| Pieza | Dónde vive |
|-------|-----------|
| Modelo, DTOs, mapeo, datos, negocio e infraestructura | Idénticos al ejemplo 01 |
| Controlador | `Controllers/HomeController.cs` |
| ViewModels | `ViewModels/` (`Catalogo`, `Detalle`, `Categoria`, `Directivas`, `Estructuras`, `Funciones`, `Error`) |
| Vistas | `Views/Home/`, `Views/Shared/` y `Views/_ViewImports.cshtml` |
| Componente de vista y Tag Helper | `ViewComponents/`, `TagHelpers/` |

## Las siete acciones

Las mismas siete pantallas que el ejemplo 01, con el mismo contenido en cada vista: son espejos.

| Acción | URL | Qué demuestra |
|--------|-----|---------------|
| `Index` | `/` | Ruta por convención, `ViewData` al lado del ViewModel, componente `vc:` y parcial |
| `Detalle` | `/producto/{id}` | **Ruta por atributo** con `:int`, `NotFound()` y sugerencias que solo existen en el ViewModel |
| `Categoria` | `/Home/Categoria?categoria=Audio` | Model Binding de un enum desde la query string y miga de pan |
| `Directivas` | `/Home/Directivas` | Tabla de directivas, `@inject` de dos servicios, `@functions` y `@section Scripts` |
| `Estructuras` | `/Home/Estructuras` | `if / else if / else`, `switch` con patrón relacional, `foreach`, `for` y `while` |
| `Funciones` | `/Home/Funciones` | Delegados `Func<>` en el bloque de código y métodos en `@functions` |
| `Error` | `/Home/Error` | La vista de error de `UseExceptionHandler`, con la ruta que falló |

## El punto de la comparación con el 01

| Concepto | Razor Pages (ejemplo 01) | MVC (este ejemplo) |
|----------|--------------------------|--------------------|
| Motor | `AddRazorPages()` | `AddControllersWithViews()` |
| Quién recibe la petición | `PageModel` | `Controller` con acciones |
| Dónde vive la lógica | `Pages/Index.cshtml.cs` | `Controllers/HomeController.cs` |
| Dónde vive la vista | `Pages/Index.cshtml` | `Views/Home/Index.cshtml` |
| Qué se le pasa a la vista | Propiedades del PageModel | El ViewModel, con `return View(modelo)` |
| Plantilla de ruta | `@page "~/producto/{id:int}"` | `[Route("producto/{id:int}")]` |
| Ruta por defecto | Carpeta de `Pages/` | `{controller=Home}/{action=Index}/{id?}` |
| `_ViewImports` y `_ViewStart` | En `Pages/` | En `Views/` |
| Parciales y componentes | Mismos, en `Pages/Shared/` | Mismos, en `Views/Shared/` |
| Demo de datos sueltos | `ViewData` en el PageModel | `ViewData` en la acción (`ViewBag` existe solo aquí) |

## El ViewModel expone solo lo que pide la vista

La lección del tema 9, hecha código: el DTO de producto no trae "sugerencias", porque un producto no *tiene* relacionados. Ese dato es de presentación, así que vive en `DetalleViewModel`, que junta el producto con los tres más baratos de su misma categoría. El servicio no se entera y el DTO no crece.

```csharp
public record DetalleViewModel(
    ProductDto Producto,
    IReadOnlyList<ProductDto> Sugerencias);
```

`ViewData` también aparece, pero solo para los datos sueltos de una sola página (el recuento del catálogo y la hora de la consulta). No se compara en tiempo de compilación: si te equivocas de nombre, el fallo llega en runtime. La demo no usa `ViewBag` a propósito: es exclusivo de los controladores MVC y no existe en Razor Pages, y así la misma línea se puede copiar tal cual en el ejemplo 01.

## El pie de página

El layout cierra con el pie de la asignatura, idéntico al del ejemplo 01:

```html
<footer class="pie">
    <p>J.L. González · Desarrollo Web en Entornos Servidor (DAW) · IES Luis Vives</p>
</footer>
```

## La semilla y los datos

Igual que en el 01: `ProductSeeder` es una clase inyectable con `SeedAsync()`, registrada en `DatabaseConfig` y resuelta desde un scope en `DatabaseInitializationExtensions`. En Development la app borra `productos.db`, lo recrea y siembra los mismos 8 productos.

## Cómo ejecutarlo

```bash
dotnet restore
dotnet build
dotnet run --project ProductosMvc
```

Abre `http://localhost:5082/`.

## Tests

```bash
dotnet test
```

Veinte pruebas: siete del servicio (EF Core InMemory) y trece de las siete acciones del controlador, construido a mano con un servicio simulado. Cada acción se comprueba devolviendo la vista y el ViewModel esperados, o el `404` cuando no hay producto.

## Verificación del espejo con el 01

Los dos ejemplos se comprueban con la misma lista de aserciones sobre las siete pantallas (tarjetas, `ViewData`, pie, menú, `switch` del detalle, 404, secciones de las demos y CSS). El estado tras la última revisión: build **0/0** en ambos, **20/20** tests en ambos y **44/44** comprobaciones de humo en cada uno.

Los cuerpos de las vistas didácticas (`Directivas`, `Estructuras`, `Funciones`, `Categoria`, `Detalle`, `Error`) son idénticos byte a byte salvo la línea `@model`, y el CSS (`wwwroot/css/site.css`) es el mismo fichero en los dos proyectos. Las diferencias reales son de arquitectura y están en la tabla anterior.

## Errores frecuentes al tocarlo

| Error | Causa |
|-------|-------|
| `The view 'Views/Home/X.cshtml' was not found` | La acción llama a `View()` sin modelo y la vista no está en la ruta esperada (`Views/{Controlador}/{Accion}.cshtml`) |
| `AmbiguousActionException` | Dos acciones responden a la misma URL: una por convención y otra por atributo |
| La acción recibe `categoria = 0` | La query string no coincidía con el nombre del parámetro, o el valor no es un nombre válido del enum |
| El enlace genera `/Home/Detalle/1` en vez de `/producto/1` | Falta el `[Route("producto/{id:int}")]` en la acción, o se quitó y volvió a la ruta por convención |
| `CS0246: no se encuentra 'XViewModel'` en la compilación de una vista | Falta `@using ProductosMvc.ViewModels` en `Views/_ViewImports.cshtml` |
| El Tag Helper sale vacío | Igual que en el 01: `output.TagMode = TagMode.StartTagAndEndTag` |

## Qué mirar después

- **03-ProductosFormularios** — alta de productos con binder propio, validación y anti-forgery, en las dos visiones.
- **12-mvc-vs-razorpages** — el tema que compara ambas arquitecturas para decidir cuándo usar cada una.
