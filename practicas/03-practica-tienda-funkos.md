# Práctica 3: Construcción de la Tienda de Funkos

- [Práctica 3: Construcción de la Tienda de Funkos](#práctica-3-construcción-de-la-tienda-de-funkos)
  - [Objetivo](#objetivo)
  - [Descripción](#descripción)
  - [Tareas a Realizar](#tareas-a-realizar)
  - [Tecnologías](#tecnologías)
  - [Estructura de Proyecto](#estructura-de-proyecto)
  - [Formato de Entrega](#formato-de-entrega)

---

## Objetivo

Construir, de principio a fin y en sus **dos visiones** (Razor Pages y MVC), una aplicación web completa de gestión de una colección de **Funkos** (figuras de vinilo coleccionables): vistas dinámicas, formularios, estado, identidad, configuración por entornos, rendimiento, localización, pruebas y despliegue en contenedor.

---

## Descripción

A lo largo de los puntos de la unidad has ido proponiendo —en los **Retos** finales de cada punto— las piezas de una misma tienda. Esta práctica las junta todas en un único proyecto: en lugar de reescribir, refactorizas y amplías lo que ya hiciste.

**Contexto:** una tienda online necesita gestionar su catálogo de Funkos: consultarlos, crearlos, modificarlos y eliminarlos, con identidad de usuario, dos idiomas y despliegue en contenedor.

Un Funko tiene, como mínimo, estas propiedades: `Id`, `Nombre`, `Categoria`, `Anio`, `PrecioReferencia`, `Imagen` (opcional), `Etiquetas` (opcional), `Activo` y `EsNovedad`.

> 💡 **Consejo:** Sigue las fases en orden. Cada fase parte de la anterior: en lugar de reescribir, refactorizas y amplías lo que ya construiste. Y recuerda el orden del oficio: primero el papel, después el código.

---

## Tareas a Realizar

### 1. Diseña tu tienda en papel (Punto 01)

- Dibuja el mapa de tu tienda: qué páginas tiene, quién entra en cada una y qué datos pintan.
- Anota, para cada vista, qué modelo necesita y de dónde saldrán sus datos.

### 2. La portada con Razor (Puntos 02-04)

> ⚠️ **Advertencia:** No empieces por el controlador. Primero la vista que enseña el producto final.

- Crea la portada con Razor puro: directivas (`@page` o la estructura MVC que elijas), `@model`, estructuras de control para el listado y una función local que calcule el precio con IVA.
- Comprueba con F12 que el HTML pintado es el que esperabas y que cualquier `<script>` escrito en un nombre de funko aparece escapado.

### 3. Layout, parciales y componentes (Punto 05)

- Monta el `_Layout` con la barra de navegación y el pie, con `_ViewStart` y `_ViewImports`.
- Trocea la interfaz: parcial para cada tarjeta de funko y componente de vista para el resumen del carrito.
- Comprueba que todas las páginas comparten la estructura sin repetir HTML.

### 4. Tag Helpers de formulario y enlaces (Punto 06)

- Escribe los enlaces de navegación con `asp-page` (o `asp-controller`/`asp-action`) y los formularios con `asp-for` y sus avisos de validación.
- Crea un Tag Helper personalizado que pinte un sello de "nuevo" para los funkos con `EsNovedad` y úsalo en el listado.

### 5. La misma tienda con controladores MVC (Puntos 07-09)

- Implementa `ProductosController` con listado, detalle, alta y baja lógica; cada acción devuelve vista, JSON o redirección según el caso.
- Monta los ViewModels (`ListadoViewModel`, `FichaViewModel`, `AltaViewModel`) con POO de verdad y justifica en el README por qué la vista no pinta la entidad directamente.

### 6. Razor Pages: la tienda en páginas (Puntos 10-12)

- Rehaz la tienda como páginas Razor Pages con sus `@page`, sus `PageModel` y sus handlers `OnGet`/`OnPost`.
- Migración real: elige una acción de MVC y trasládala a una página, vista a vista, comprobando que responde igual; anota en el README la colisión de rutas que encuentres y cómo la resuelves.

### 7. Alta con formularios y model binding (Puntos 13-14)

- Diseña el formulario de alta con Tag Helpers y aplica el patrón PRG: tras el `POST`, redirige al listado con el aviso de éxito en TempData.
- Enlaza el objeto completo `AltaViewModel` con prefijo y una colección de etiquetas con casillas; comprueba qué le pasa al precio si llega con punto decimal y resuélvelo con cultura invariable.

### 8. Validaciones y seguridad (Punto 15)

- Valida en servidor con atributos y en la vista con sus avisos; comprueba que un envío con JavaScript desactivado sigue rechazado.
- Protege los `POST` con token antifalsificación; comprueba con `curl` que un envío sin token responde **400** y no toca nada.
- Revisa que ningún dato del usuario se pinta con `Html.Raw` y que el aviso de acceso fallido no revela cuál de los dos datos falla.

### 9. La foto del funko: ficheros (Punto 16)

- Implementa la subida de imagen con `IFormFile`, validación de tipo y tamaño, nombre generado por el servidor y guardado fuera del alcance.
- Comprueba con una ruta trucada (`..`) que no sales del directorio de subidas y monta la descarga con `File`.

### 10. Estado: visitas, avisos y cesta (Puntos 17-18)

- Añade el contador de visitas en sesión y el aviso de "funko añadido" con TempData; comprueba con F12 cuándo aparece cada cookie.
- Escribe una cookie de preferencias con `HttpOnly` y `Expires` y monta la cesta en sesión; comprueba que al cambiar un carácter de la cookie de sesión el servidor monta datos nuevos.

### 11. Dueños de la tienda con Identity (Punto 19)

- Configura Identity con `AddIdentity`, `AppDbContext` y sus políticas; alta y acceso con correo y clave.
- Protege la zona de gestión con `[Authorize]` y el rol `Admin`; comprueba que un usuario sin rol recibe la página de acceso denegado y que el cierre de sesión borra la identidad.

### 12. Configuración por entornos e Infrastructure (Punto 20)

- Separa la configuración por entornos (`appsettings`, `appsettings.Development.json`, `appsettings.Production.json`) y mueve los secretos a user secrets o variables de entorno.
- Crea la carpeta `Infrastructure` con sus concerns (`ServicesConfig`, `RepositoriesConfig`, `CacheConfig`, `LocalizationConfig`...) y deja el `Program.cs` como índice de llamadas.
- Declara una sección `Tienda` con `IOptions<TiendaConfig>` y comprueba que un entorno distinto cambia la configuración sin tocar código.

### 13. Rendimiento (Punto 21)

- Añade `IMemoryCache` al servicio de precios con caducidad y comprueba que la segunda petición no vuelve a calcular.
- Caché de salida en el listado público con etiqueta e invalidación al alta; comprueba con un contador que la respuesta se sirve sin ejecutar la página y que el alta lo refleja al instante.
- Activa la compresión y comprueba con `curl -i -H "Accept-Encoding: gzip"` que la respuesta sale comprimida.

### 14. Dos idiomas (Punto 22)

- Crea `Resources/SharedResource.resx` en español y su variante en inglés; localiza textos de portada, formulario y errores.
- Configura `LocalizationConfig` con las dos culturas y el selector con cookie; comprueba que `?lang=en-US` cambia el idioma, que `?lang=fr-FR` cae en el neutral y que el precio se formatea con la cultura activa.

### 15. Herramientas, pruebas y documentación (Puntos 23-24)

- Registra con `ILogger` las operaciones del catálogo (information al listar, warning sin stock, error con excepción) y revisa la página de error de desarrollo frente a la de producción.
- Crea el proyecto `FunkoApp.Test`: pruebas unitarias de la lógica con NUnit y FluentAssertions (patrón AAA y casos inválidos) y pruebas de portada y de flujo de alta con Playwright.
- Documenta la lógica con XMLDoc y escribe el README con qué es, cómo se ejecuta, cómo se prueban y cómo está montado.

### 16. Despliegue (Punto 25)

- Publica con `dotnet publish` y ejecuta la carpeta publicada con una variable de entorno distinta.
- Escribe el Dockerfile por fases con su `.dockerignore`; construye la imagen y arranca el contenedor con `-p` y `-e`, comprobando con `curl` que responde en el puerto publicado.
- Crea `.github/workflows/despliegue.yml` con dos trabajos encadenados (`pruebas` y `publicar`) y conecta el repositorio a un servicio como Render, con las variables en su panel.

---

## Tecnologías

| Tecnología | Para qué | Paquete NuGet |
|------------|----------|---------------|
| **ASP.NET Core** | Las dos visiones: Razor Pages y MVC | - |
| **ASP.NET Core Identity** | Usuarios, accesos y roles | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| **EF Core en memoria** | Persistencia de laboratorio | `Microsoft.EntityFrameworkCore.InMemory` |
| **NUnit + FluentAssertions** | Pruebas unitarias | `NUnit`, `FluentAssertions` |
| **Playwright** | Pruebas de página con navegador real | `Microsoft.Playwright`, `Microsoft.Playwright.NUnit` |
| **Docker** | Contenedor de despliegue | - |
| **C# 14** | Primary constructors, top-level statements | - |

---

## Estructura de Proyecto

```
FunkoApp/
├── FunkoApp.slnx
├── FunkoApp/                  (visión Razor Pages)
│   ├── Program.cs
│   ├── appsettings*.json
│   ├── Pages/
│   ├── Models/
│   ├── Services/
│   ├── Infrastructure/
│   └── Resources/
├── FunkoAppMvc/               (visión MVC)
│   ├── Program.cs
│   ├── appsettings*.json
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   ├── Services/
│   ├── Infrastructure/
│   └── Resources/
├── FunkoApp.Test/
│   ├── Models/
│   ├── Pages/
│   └── Mvc/
├── Dockerfile
├── .dockerignore
├── .github/workflows/despliegue.yml
└── README.md
```

---

## Formato de Entrega

- Repositorio en **GitHub** con las dos aplicaciones (`FunkoApp` y `FunkoAppMvc`) y el proyecto de pruebas, con commits incrementales (uno por fase).
- **README** con: qué es, cómo se ejecuta cada aplicación, cómo se prueban, cómo se construye la imagen y una sección de *Decisiones tomadas* (por qué esa caché, por qué ese orden de fuentes, por qué esa política de roles).
- Las pruebas deben pasar en verde (`dotnet test`).
- La aplicación debe arrancar desde su imagen (`docker run -p 8080:8080`) y responder en la portada con la configuración del entorno.
