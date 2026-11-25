# 03 - Productos Formularios

El catálogo de los ejemplos 01 y 02, ahora con **alta, edición y borrado de productos**: formularios con validación de dos capas, model binding propio para decimales, anti-forgery y el patrón Post-Redirect-Get. Todo ello en **las dos visiones** (Razor Pages y MVC) sobre la misma capa de negocio compartida.

## Descripción

Leer un catálogo es fácil; escribirlo es donde empiezan los problemas reales: el usuario manda "12,75" con coma española, rellena el nombre con un carácter, intenta mandar el formulario dos veces o un enlace de otra página intenta borrar un producto a sus espaldas. Este ejemplo ataca esos cuatro frentes y, de paso, enseña el model binding avanzado (prefijos y colecciones por índice).

| Pieza | Dónde vive |
|-------|-----------|
| Modelo, DTOs, mapeo, datos, negocio e infraestructura | `ProductosWeb.Shared/` (compartido por las dos visiones) |
| Binder de decimales | `Shared/Binders/` (`DecimalModelBinder`, `DecimalModelBinderProvider`) |
| Validación de dos capas | `Shared/ViewModels/` (DataAnnotations) + `Shared/Validators/` (FluentValidation) |
| Visión Razor Pages (puerto 5084) | `ProductosWeb.RazorPages/Pages/Productos/` |
| Visión MVC (puerto 5083) | `ProductosWeb.Mvc/Controllers/`, `ProductosWeb.Mvc/Views/` |
| Tests | `ProductosWeb.Test/` (binder, validadores, servicio y las dos visiones) |

## Las tres pantallas y la página de Binding

| Pantalla | MVC | Razor Pages | Qué demuestra |
|----------|-----|-------------|---------------|
| Catálogo | `GET /` | `GET /Productos` | Tabla, `TempData` de confirmación y borrado por POST |
| Alta | `GET/POST /Productos/Crear` | `GET/POST /Productos/Crear` | Form Tag Helper con anti-forgery y validación de dos capas |
| Edición | `GET/POST /Productos/Editar/{id}` | `GET/POST /Productos/Editar/{id}` | Precarga, campo oculto `Id` y PRG con `TempData` |
| Binding | `GET/POST /Productos/Binding` | `GET/POST /Productos/Binding` | Prefijo `Cabecera.*` y colecciones `Etiquetas[i]`, `Recargos[i].*` |

## Las dos capas de validación, en el mismo `ModelState`

Los DataAnnotations viven en el ViewModel y se ejecutan solos al enlazar el modelo; FluentValidation añade las reglas que cruzan campos o se leen mejor en lenguaje de validación. El paquete es solo `FluentValidation` (el integrado con ASP.NET Core está sin soporte oficial), así que cada acción invoca el validador y vuelca el resultado a mano:

```csharp
var resultado = await validador.ValidateAsync(model);
resultado.AddToModelState(ModelState);

if (!ModelState.IsValid)
{
    return View(model);   // o return Page() en Razor Pages
}
```

`AddToModelState` es una extensión propia (`Shared/Extensions/ValidationResultExtensions.cs`). Ojo a dónde aterriza cada error: los de FluentValidation usan el nombre de la propiedad, así que en la página de Binding el error del nombre aparece en `Cabecera.Nombre`, no en la clave vacía.

## El binder de decimales

`DecimalModelBinder` acepta coma y punto indistintamente ("12,75" y "12.50" dan el mismo número), se registra en la lista de `ModelBinderProviders` y resuelve el problema de que el hilo venga en una cultura anglosajona. En MVC se inserta en `AddControllersWithViews(options)`; en Razor Pages **no** va en `RazorPagesOptions` (esa opción solo admite convenciones de página), sino en la configuración de MVC que Razor Pages usa por debajo:

```csharp
services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
    });
```

## Post-Redirect-Get con TempData

Si el alta sale bien, el controlador no devuelve la vista del formulario: redirige con un `302` y guarda el mensaje en `TempData`, que viaja en cookies y se consume al leerlo. Sin PRG, recargar el navegador repetiría el alta; sin `TempData`, el usuario no vería nunca la confirmación. El borrado también es POST con token: un enlace `<a href>` dispara un GET, y un GET no debe borrar nada.

## El anti-forgery

El form Tag Helper emite el campo oculto `__RequestVerificationToken` y cada POST lo exige con `[ValidateAntiForgeryToken]`. No se toca `HeaderName`: ese ajuste es para llamadas AJAX; los formularios usan el campo oculto de siempre. El token va emparejado con una cookie, de modo que GET y POST tienen que compartir sesión.

## La página de Binding

No persiste nada: muestra, línea a línea, todo lo que el binder ha recogido (`Cabecera.Nombre = ...`, `Etiquetas[1] = nuevo`, `Recargos[0].Importe = 4,95`). El partial `_FormularioProducto` se reutiliza sin tocarlo: el `for` del partial tag helper establece el prefijo del campo, y los bucles `@for` escriben los índices de cada colección. Los botones de "añadir fila" hacen un POST que repinta sin validar; el de "probar" valida las dos capas y escribe el resumen.

## Cómo ejecutarlo

```bash
dotnet restore
dotnet build
dotnet run --project ProductosWeb.RazorPages   # http://localhost:5084
dotnet run --project ProductosWeb.Mvc          # http://localhost:5083
```

La base de datos es un `productos.db` local: en Development se borra y se siembra con cada arranque, así que siempre partes de los mismos 5 productos.

## Tests

```bash
dotnet test
```

Sesenta y cuatro pruebas: diez del binder decimal (con un `IValueProvider` de prueba y un `ModelBinderProviderContext` simulado), dieciséis de los dos validadores (los del producto incluyen etiquetas y recargos por índice), diez del servicio con EF Core InMemory, doce del controlador y seis de los PageModels del alta y del Binding. Los tests de controlador y páginas usan el validador **real**, no un mock, para comprobar de verdad que las dos capas llegan al mismo `ModelState`.

## Verificación de las dos visiones

Las dos visiones se comprueban con la misma lista de aserciones: ciclo completo de alta, validación inválida, precio con coma, nombre duplicado, edición, borrado, `TempData` que se consume, anti-forgery (un POST sin token devuelve `400`) y la página de Binding con sus prefijos e índices. El estado tras la última revisión: build **0/0**, **64/64** tests y **124/124** comprobaciones de humo (43 + 19 de Binding, repetidas en cada visión).

El CSS y el partial del formulario son idénticos byte a byte en los dos proyectos. Lo que cambia es de dónde se invoca cada cosa: `PageModel` con handlers, o `Controller` con acciones.

## Errores frecuentes al tocarlo

| Error | Causa |
|-------|-------|
| `Unable to resolve service for type 'ISessionStore'` al arrancar | `app.UseSession()` sin almacén registrado; `TempData` por defecto usa cookies y no necesita sesión |
| El POST devuelve `400` aunque el token se manda | El token va emparejado con una cookie: si GET y POST no comparten sesión, el servidor lo rechaza |
| El binder de decimales no se aplica en Razor Pages | Está en `RazorPagesOptions`, que no tiene `ModelBinderProviders`; va en `AddMvcOptions` |
| El error de FluentValidation no aparece en el resumen | Aterriza en el nombre de la propiedad (`Cabecera.Nombre`, `Etiquetas[0]`), no en la clave vacía |
| 404 en la raíz de Razor Pages | No hay página en `/`; el índice está en `/Productos` |
| `CS1572` en el controlador | Un `<param>` del comentario XML que no corresponde a ningún parámetro de la acción |

## Qué mirar después

- **04-ProductosApi** — la misma capa de negocio detrás de una API REST con Web API.
- **12-mvc-vs-razorpages** — el tema que compara ambas arquitecturas para decidir cuándo usar cada una.
