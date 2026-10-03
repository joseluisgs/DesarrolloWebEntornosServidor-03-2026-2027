# Cuestionario de Investigación y Desarrollo (R&D) - ASP.NET Core MVC + Blazor Server

- [Cuestionario de Investigación y Desarrollo (R\&D) - ASP.NET Core MVC + Blazor Server](#cuestionario-de-investigación-y-desarrollo-rd---aspnet-core-mvc--blazor-server)
    - [1. **Optimización del Rendimiento en ViewData/ViewBag Global y Servicios Scoped**](#1-optimización-del-rendimiento-en-viewdataviewbag-global-y-servicios-scoped)
    - [2. **Decisión Arquitectónica:  Strongly-Typed Models vs. ViewBag/ViewData vs. TempData**](#2-decisión-arquitectónica--strongly-typed-models-vs-viewbagviewdata-vs-tempdata)
    - [3. **Filosofía del Motor de Plantillas Razor y Separación de Responsabilidades**](#3-filosofía-del-motor-de-plantillas-razor-y-separación-de-responsabilidades)
    - [4. **El Patrón Post-Redirect-Get (PRG) y la Gestión de TempData en ASP.NET Core**](#4-el-patrón-post-redirect-get-prg-y-la-gestión-de-tempdata-en-aspnet-core)
    - [5. **Protección CSRF en Diferentes Tipos de Peticiones (Forms vs AJAX/REST APIs)**](#5-protección-csrf-en-diferentes-tipos-de-peticiones-forms-vs-ajaxrest-apis)
    - [6. **Optimización de Consultas y Paginación en Entity Framework Core**](#6-optimización-de-consultas-y-paginación-en-entity-framework-core)
    - [7. **Layouts, Partial Views, View Components y View Models:  Arquitectura de Vistas en ASP.NET Core**](#7-layouts-partial-views-view-components-y-view-models--arquitectura-de-vistas-en-aspnet-core)
    - [8. **Validación Multicapa:  Client-Side, Server-Side y Database Constraints**](#8-validación-multicapa--client-side-server-side-y-database-constraints)
    - [9. **Arquitectura Limpia:  Controller Delgado, Service Rico, Repository Agnóstico**](#9-arquitectura-limpia--controller-delgado-service-rico-repository-agnóstico)
    - [10. **Gestión de Estado:  Sesiones, Cookies, Claims y Blazor Server Circuits**](#10-gestión-de-estado--sesiones-cookies-claims-y-blazor-server-circuits)



---

### 1. **Optimización del Rendimiento en ViewData/ViewBag Global y Servicios Scoped**

Las variables globales inyectadas mediante servicios Scoped o accesibles desde `ViewData`/`ViewBag` se evalúan en **cada petición HTTP** en ASP.NET Core MVC. Discuta detalladamente cómo la implementación de *operaciones costosas de Entity Framework Core* (como consultas sin índices o cálculos complejos en memoria) dentro de métodos que se ejecutan por cada request puede llevar al problema de **degradación del rendimiento**. Proponga y justifique al menos tres estrategias de optimización recomendadas (como *IMemoryCache*, *Lazy Loading diferido*, o *View Components con caché*) que el proyecto debe aplicar para mitigar este riesgo, considerando el trade-off entre coherencia de datos y performance.

---

### 2. **Decisión Arquitectónica:  Strongly-Typed Models vs. ViewBag/ViewData vs. TempData**

Aunque `ViewBag`, `ViewData` y `TempData` están disponibles para pasar datos del Controller a la Vista, las mejores prácticas recomiendan usar **Strongly-Typed Models** por defecto para el 95% de los casos.  Investigue y contraste las características que hacen a los modelos fuertemente tipados el *enfoque moderno, más seguro y mantenible* frente a diccionarios dinámicos.  Analice: 
- Las ventajas de IntelliSense y compilación type-safe
- Los problemas de refactoring con ViewBag/ViewData
- Los **casos especiales** donde `TempData` se justifica (patrón PRG)
- Cuándo `ViewBag` podría ser aceptable (datos muy puntuales)

Proporcione ejemplos de código que demuestren vulnerabilidades o errores en runtime que los modelos tipados previenen.

---

### 3. **Filosofía del Motor de Plantillas Razor y Separación de Responsabilidades**

Razor sigue el principio de **"Lógica de negocio en C# (Controller/Service), Presentación en la Vista"**. Analice por qué operaciones complejas como: 
- Manipulación de strings con expresiones regulares
- Transformaciones de datos (parsing, conversiones complejas)
- Lógica condicional de negocio (cálculo de descuentos, validaciones)

**NO deben** implementarse directamente en las vistas Razor usando bloques `@{ }`. Explique: 
- Dónde debe residir esta lógica (Service Layer, Extension Methods, View Models)
- Cómo los **View Components** ofrecen una solución arquitectónica limpia para encapsular lógica de presentación reutilizable
- Cuándo es aceptable usar pequeños fragmentos de código C# en Razor (ej:  formateo simple con `ToString()`)
- El impacto en testabilidad cuando la lógica se mezcla en las vistas

---

### 4. **El Patrón Post-Redirect-Get (PRG) y la Gestión de TempData en ASP.NET Core**

El Patrón PRG es una **buena práctica obligatoria** después de procesar formularios POST en ASP.NET Core MVC. Explique:

1. **El problema que resuelve:** El riesgo de reenvío accidental del formulario (409 Conflict, duplicación de registros en BD) cuando el usuario presiona F5 o el botón "Atrás" del navegador.

2. **Cómo funciona PRG paso a paso:**
   - POST procesa datos
   - Servidor responde con HTTP 303 See Other o 302 Found
   - Cliente hace GET automático a la URL de destino

3. **El papel de TempData:**
   - Por qué el `Model` tradicional se pierde en una redirección
   - Cómo `TempData` persiste datos entre la redirección (usando cookies o sesión)
   - La diferencia entre `TempData["key"]` y `TempData.Keep("key")`

4. **Alternativas y anti-patrones:**
   - Por qué `return View()` después de POST es un anti-patrón
   - Cuándo usar `RedirectToAction` vs `RedirectToPage` vs `Redirect`

Incluya un ejemplo de código completo mostrando el flujo correcto. 

---

### 5. **Protección CSRF en Diferentes Tipos de Peticiones (Forms vs AJAX/REST APIs)**

ASP.NET Core incluye protección CSRF integrada que valida tokens en **todos** los formularios POST, PUT y DELETE. Compare tres enfoques para manejar estos tokens:

1. **Tag Helpers en formularios HTML tradicionales:**
   ```html
   <form asp-action="Create" method="post">
       @* Token generado automáticamente *@
   </form>
   ```

2. **RequestVerificationToken manual en peticiones AJAX:**
   ```javascript
   fetch('/api/resource', {
       method: 'POST',
       headers: {
           'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
       }
   })
   ```

3. **Configuración de APIs REST sin CSRF (stateless con JWT):**
   - Por qué las APIs REST típicamente deshabilitan CSRF
   - Uso de `[IgnoreAntiforgeryToken]`
   - Autenticación basada en tokens (JWT Bearer)

Justifique:
- Por qué el enfoque de Tag Helpers es el más **seguro por defecto** para aplicaciones MVC tradicionales
- Los riesgos de seguridad al deshabilitar CSRF globalmente
- Cómo configurar políticas diferenciadas (CSRF para UI, JWT para APIs)

---

### 6. **Optimización de Consultas y Paginación en Entity Framework Core**

Cuando se implementa paginación con Entity Framework Core, el uso de ciertas técnicas puede generar cuellos de botella.  Analice:

1. **El problema del N+1 Query:**
   - Qué es y cómo detectarlo (SQL logging)
   - Diferencia entre Lazy Loading, Eager Loading (`Include`) y Explicit Loading
   - Cuándo cada estrategia es apropiada

2. **Paginación eficiente:**
   ```csharp
   // ❌ Ineficiente: carga todo en memoria
   var allItems = await context. Funkos. ToListAsync();
   var page = allItems.Skip(pageSize * (page - 1)).Take(pageSize);
   
   // ✅ Eficiente: paginación en base de datos
   var page = await context.Funkos
       .OrderBy(f => f.Nombre)
       .Skip(pageSize * (page - 1))
       .Take(pageSize)
       .ToListAsync();
   ```

3. **Problema del `Count()` automático:**
   - Por qué `context.Funkos.Count()` puede ser lento en tablas grandes
   - Alternativas:  estimaciones, caché, paginación sin contador total

4. **Proyecciones para optimizar transferencia:**
   - Uso de `Select()` para traer solo los campos necesarios
   - DTOs vs Entidades completas

Proponga una implementación completa de paginación optimizada con:
- Repository Pattern
- Result<T> para manejo de errores
- Caché de conteos

---

### 7. **Layouts, Partial Views, View Components y View Models:  Arquitectura de Vistas en ASP.NET Core**

ASP.NET Core MVC ofrece múltiples mecanismos para organizar y reutilizar código de vistas.  Compare y contraste:

1. **Layouts (`_Layout.cshtml`):**
   - Propósito: estructura base común (header, footer, scripts)
   - Uso de `@RenderBody()` y `@RenderSection()`
   - Configuración en `_ViewStart.cshtml`

2. **Partial Views (`_PartialViewName.cshtml`):**
   - Cuándo usar:  fragmentos simples reutilizables sin lógica
   - `@await Html.PartialAsync()` vs `<partial>`
   - Paso de modelos a partials

3. **View Components:**
   - Cuándo usar: lógica compleja (ej: widget de carrito, menú dinámico)
   - Estructura: clase C# + vista Razor
   - Ventajas sobre partials (testabilidad, dependency injection)

4. **View Models:**
   - Diferencia entre Entity, InputModel, DTO y ViewModel
   - Cuándo crear un ViewModel específico para una vista
   - Mapeo con AutoMapper

Diseñe la arquitectura de vistas para una aplicación de e-commerce indicando qué técnica usar para: 
- Navbar con contador de carrito
- Lista de productos paginada
- Formulario de checkout multi-paso
- Dashboard de administración

---

### 8. **Validación Multicapa:  Client-Side, Server-Side y Database Constraints**

La seguridad mediante validación requiere un enfoque de **defensa en profundidad**.  Analice las tres capas:

1. **Validación Client-Side (JavaScript):**
   - Propósito: UX (feedback inmediato)
   - Vulnerabilidad: fácilmente evitable (Postman, curl, navegador modificado)
   - Implementación: jQuery Validation Unobtrusive

2. **Validación Server-Side (Data Annotations + FluentValidation):**
   ```csharp
   public class CrearFunkoInputModel
   {
       [Required(ErrorMessage = "El nombre es obligatorio")]
       [StringLength(100, MinimumLength = 3)]
       public string Nombre { get; set; }
       
       [Range(0.01, 9999.99)]
       public decimal Precio { get; set; }
   }
   ```
   - Por qué es la **única barrera real de seguridad**
   - Diferencia entre Data Annotations y FluentValidation
   - Validación custom con `IValidatableObject`

3. **Database Constraints (SQL):**
   - Unique indexes, Foreign Keys, Check Constraints
   - Por qué son necesarias incluso con validación en código
   - Mapeo con EF Core Fluent API

Diseñe un sistema de validación completo para el registro de usuarios considerando:
- Email único (¿validas en código o en BD?)
- Contraseña segura (reglas, hashing)
- Fecha de nacimiento (mayor de edad)
- Manejo de errores concurrentes (dos usuarios registran el mismo email simultáneamente)

---

### 9. **Arquitectura Limpia:  Controller Delgado, Service Rico, Repository Agnóstico**

El patrón **Service Layer** es fundamental para mantener un código mantenible.  Compare estos dos enfoques:

**❌ Anti-patrón (Controller Gordo):**
```csharp
public class FunkosController : Controller
{
    private readonly AppDbContext _context;
    
    public async Task<IActionResult> Crear(CrearFunkoInputModel input)
    {
        // Validación en Controller
        if (await _context.Funkos.AnyAsync(f => f. Nombre == input.Nombre))
            return BadRequest("Nombre duplicado");
        
        // Lógica de negocio en Controller
        var funko = new Funko { Nombre = input.Nombre };
        _context.Funkos.Add(funko);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}
```

**✅ Arquitectura Limpia:**
```csharp
// Controller (Delgado)
public class FunkosController : Controller
{
    private readonly IFunkoService _service;
    
    public async Task<IActionResult> Crear(CrearFunkoInputModel input)
    {
        var result = await _service.CrearAsync(input);
        return result. Match(
            onSuccess: _ => RedirectToAction("Index"),
            onFailure: error => { ModelState.AddModelError("", error); return View(input); }
        );
    }
}

// Service (Rico)
public class FunkoService : IFunkoService
{
    private readonly IFunkoRepository _repo;
    
    public async Task<Result<FunkoDto>> CrearAsync(CrearFunkoInputModel input)
    {
        // Validación de negocio
        var existeNombre = await _repo.ExisteNombreAsync(input.Nombre);
        if (existeNombre) return Result.Failure<FunkoDto>("Nombre duplicado");
        
        // Lógica de negocio
        var funko = input.ToEntity();
        await _repo.AddAsync(funko);
        
        return Result.Success(funko.ToDto());
    }
}
```

Analice: 
- Ventajas de testabilidad (mock del service vs mock del DbContext)
- Reutilización (el service se puede llamar desde API, MVC, Blazor)
- Mantenibilidad (cambios de lógica solo en un lugar)
- Railway Oriented Programming (Result<T>)

Diseñe la arquitectura completa (Controller → Service → Repository → DbContext) para un caso de uso complejo:  **"Crear pedido con validación de stock, aplicación de cupón de descuento y envío de email de confirmación"**.

---

### 10. **Gestión de Estado:  Sesiones, Cookies, Claims y Blazor Server Circuits**

ASP.NET Core y Blazor Server ofrecen múltiples mecanismos de estado. Compare y contraste:

| Mecanismo                   | Ubicación                 | Persistencia              | Seguridad                        | Casos de Uso                     |
| --------------------------- | ------------------------- | ------------------------- | -------------------------------- | -------------------------------- |
| **HttpContext. Session**    | Servidor                  | Timeout configurable      | ✅ Alta                           | Carrito, preferencias temporales |
| **Cookies**                 | Cliente                   | Configurable (Expiration) | ⚠️ Media (firmadas pero visibles) | Recordar idioma, tema            |
| **Claims (Authentication)** | Servidor + Cookie cifrada | Sesión de login           | ✅ Alta                           | Identidad usuario, roles         |
| **TempData**                | Servidor/Cookie           | Una petición              | ✅ Alta                           | Mensajes flash (PRG)             |
| **Blazor Circuit State**    | Servidor                  | Conexión SignalR activa   | ✅ Alta                           | Estado de componente Blazor      |

**Preguntas de análisis:**

1. **Seguridad:**
   - ¿Por qué nunca almacenar información sensible en Cookies no cifradas?
   - ¿Cómo protege ASP.NET Core las Cookies de autenticación?
   - ¿Qué pasa si un atacante roba una Session Cookie?

2. **Blazor Server Circuits:**
   - ¿Qué pasa con el estado cuando se pierde la conexión SignalR?
   - ¿Cómo persiste datos entre reconexiones?
   - Diferencia entre `@inject` con Scoped service vs Singleton

3. **Diseño de aplicación híbrida:**
   Diseñe la estrategia de estado para una aplicación que tiene:
   - Páginas MVC (catálogo público)
   - Dashboard Blazor Server (administración)
   - API REST (móvil)
   
   Indique qué mecanismo usar para: 
   - Autenticación común (cookie compartida)
   - Carrito de compras (sesión en MVC, servicio scoped en Blazor)
   - Preferencias de idioma (cookie)
   - Token JWT para API

4. **Escalabilidad:**
   - Problemas de sesiones en memoria con múltiples servidores
   - Soluciones:  Redis, SQL Server Session State
   - Trade-offs de rendimiento vs consistencia

Proporcione código completo de configuración en `Program.cs` para:
- Sesión con Redis
- Cookie de autenticación compartida entre MVC y Blazor
- CORS para API con credenciales

---

