# Test de Desarrollo Web con ASP.NET Core MVC + Blazor Server (100 Preguntas)

- [Test de Desarrollo Web con ASP.NET Core MVC + Blazor Server (100 Preguntas)](#test-de-desarrollo-web-con-aspnet-core-mvc--blazor-server-100-preguntas)
  - [I. Arquitectura y Fundamentos de ASP.NET Core MVC (Preguntas 1-25)](#i-arquitectura-y-fundamentos-de-aspnet-core-mvc-preguntas-1-25)
  - [II. Razor Views y Tag Helpers (Preguntas 26-45)](#ii-razor-views-y-tag-helpers-preguntas-26-45)
  - [III. Model Binding, Validación y Formularios (Preguntas 46-65)](#iii-model-binding-validación-y-formularios-preguntas-46-65)
  - [IV. Estado, Sesiones y Autenticación (Preguntas 66-80)](#iv-estado-sesiones-y-autenticación-preguntas-66-80)
  - [V. Blazor Server (Preguntas 81-95)](#v-blazor-server-preguntas-81-95)
  - [VI. Entity Framework Core y Testing (Preguntas 96-100)](#vi-entity-framework-core-y-testing-preguntas-96-100)


---

## I. Arquitectura y Fundamentos de ASP.NET Core MVC (Preguntas 1-25)

1. ¿Cuál de los siguientes no es un componente fundamental del patrón de diseño Model-View-Controller (MVC)?
   - A) Modelo
   - B) Vista
   - C) Controlador
   - D) Repository

2. En el patrón MVC de ASP.NET Core, ¿cuál es la principal función del componente Controlador?
   - A) Renderizar vistas Razor con HTML
   - B) Representar los datos y la lógica de negocio
   - C) Manejar las peticiones HTTP y coordinar Model y View
   - D) Almacenar información de sesión mediante cookies

3. En el flujo de una petición en ASP.NET Core MVC, ¿qué componente actúa como el núcleo del pipeline que procesa las peticiones HTTP?
   - A) View Engine
   - B) Middleware Pipeline
   - C) Routing System
   - D) HomeController

4. ¿Cuál de los siguientes enfoques para pasar datos del Controller a la Vista es considerado el más moderno y recomendado en ASP.NET Core MVC?
   - A) ViewBag
   - B) ViewData
   - C) Strongly-typed Model
   - D) TempData

5. ¿Para qué se utiliza la anotación `[ApiController]` en una clase de controlador? 
   - A) Definir la lógica de negocio (Service Layer)
   - B) Mapear una entidad a una tabla con Entity Framework
   - C) Habilitar características automáticas para APIs REST
   - D) Manejar vistas Razor y devolver HTML

6. La diferencia clave entre la ejecución de código en el Servidor (C#) y la ejecución en el Cliente (JavaScript) es que el código del Servidor:
   - A) Puede manipular el DOM del navegador
   - B) Se puede ver y manipular fácilmente por el usuario
   - C) Es usado solo para validación instantánea de formularios
   - D) Accede de forma segura a la Base de Datos y ejecuta la lógica de negocio

7. ¿Cuál es la técnica de ASP.NET Core MVC utilizada para vincular automáticamente los datos de un formulario POST a un objeto C# (DTO/InputModel)?
   - A) Route Parameters
   - B) Query String Binding
   - C) Model Binding
   - D) Dependency Injection

8. El principal objetivo de utilizar el patrón Post-Redirect-Get (PRG) después de procesar un formulario POST es: 
   - A) Permitir el uso de TempData
   - B) Optimizar el rendimiento de Entity Framework
   - C) Prevenir el reenvío accidental del formulario si el usuario refresca la página
   - D) Configurar el código de estado HTTP 200 OK

9. Cuando se usa una redirección (`return RedirectToAction(... )`), ¿qué objeto de ASP.NET Core se utiliza para enviar mensajes que sobreviven esa única redirección?
   - A) ViewBag
   - B) HttpContext. Session
   - C) ViewData
   - D) TempData

10. Dentro del Modelo (lógica de negocio), ¿cuál es la capa que encapsula la lógica de negocio, aplica Railway Oriented Programming (ROP) y orquesta los repositorios? 
    - A) Controller
    - B) Entity/Model
    - C) Repository
    - D) Service

11. En ASP.NET Core MVC, ¿qué sistema traduce las URLs entrantes a acciones específicas de controladores?
    - A) View Engine
    - B) Middleware Pipeline
    - C) Routing System
    - D) Model Binder

12. ¿Qué anotación se utiliza en un parámetro de método de Controller para extraer una variable de la URL, como el `id` en `/funkos/{id}`?
    - A) `[FromQuery]`
    - B) `[FromBody]`
    - C) `[FromRoute]`
    - D) `[FromServices]`

13. ¿Cuál es el framework de seguridad integrado en ASP.NET Core para gestionar autenticación y autorización?
    - A) IdentityServer
    - B) Entity Framework Security
    - C) ASP.NET Core Identity
    - D) Tag Helper Security

14. ¿Qué resultado de acción (`IActionResult`) se utiliza para devolver una vista Razor desde un controlador?
    - A) JsonResult
    - B) ViewResult
    - C) RedirectResult
    - D) ContentResult

15. En ASP.NET Core, ¿qué anotación se usa para especificar la ruta HTTP y el verbo (GET, POST, etc.) de una acción de controlador?
    - A) `[Action]`
    - B) `[Route]` y `[HttpGet]`/`[HttpPost]`
    - C) `[Controller]`
    - D) `[Authorize]`

16. ¿Qué tipo de resultado devuelve `RedirectToAction("Index", "Home")`?
    - A) ViewResult
    - B) JsonResult
    - C) RedirectToActionResult
    - D) ContentResult

17. En el contexto de ASP.NET Core MVC, ¿qué es un ViewModel?
    - A) Una entidad de base de datos decorada con Data Annotations
    - B) Un objeto que encapsula datos específicos para una vista particular
    - C) Un servicio inyectado en el controlador
    - D) Una clase que hereda de Controller

18. ¿Cuál es el archivo de configuración principal de ASP. NET Core donde se definen cadenas de conexión y configuraciones de la aplicación?
    - A) web.config
    - B) appsettings.json
    - C) Program.cs
    - D) Startup.cs

19. ¿Qué método de Program.cs se utiliza para registrar servicios en el contenedor de Dependency Injection?
    - A) app.UseRouting()
    - B) builder.Services.AddScoped<IService, Service>()
    - C) app.MapControllerRoute()
    - D) app.Run()

20. ¿Cuál es la diferencia principal entre `ViewBag` y `ViewData` en ASP.NET Core MVC?
    - A) ViewBag es tipado fuerte, ViewData es dinámico
    - B) ViewBag usa sintaxis dinámica, ViewData usa diccionario con claves string
    - C) ViewBag solo funciona en Razor Pages
    - D) No hay diferencia, son alias

21. ¿Qué componente de ASP.NET Core se encarga de transformar el nombre lógico de la vista en la ruta física del archivo . cshtml?
    - A) Routing Middleware
    - B) View Engine
    - C) Model Binder
    - D) Action Invoker

22. En ASP.NET Core MVC, ¿qué patrón arquitectónico separa las responsabilidades en Presentación, Lógica de Negocio y Acceso a Datos?
    - A) MVVM (Model-View-ViewModel)
    - B) Layered Architecture
    - C) Microservices
    - D) Event-Driven Architecture

23. ¿Qué tipo de lifetime de servicio en Dependency Injection crea una nueva instancia en cada petición HTTP?
    - A) Singleton
    - B) Transient
    - C) Scoped
    - D) Factory

24. ¿Cuál es el propósito del middleware `UseStaticFiles()` en ASP.NET Core?
    - A) Servir archivos estáticos como CSS, JavaScript e imágenes
    - B) Comprimir respuestas HTTP
    - C) Habilitar autenticación
    - D) Configurar el enrutamiento

25. ¿Qué anotación se utiliza para indicar que una acción de controlador requiere autenticación?
    - A) `[Authenticated]`
    - B) `[Secure]`
    - C) `[Authorize]`
    - D) `[RequireAuth]`

---

## II. Razor Views y Tag Helpers (Preguntas 26-45)

26. En la sintaxis de Razor, ¿qué símbolo se utiliza para alternar entre código C# y HTML?
    - A) `{}`
    - B) `@`
    - C) `$`
    - D) `%`

27. ¿Qué directiva Razor se usa para definir el tipo de modelo fuertemente tipado en una vista? 
    - A) `@using`
    - B) `@inject`
    - C) `@model`
    - D) `@page`

28. ¿Qué medida de seguridad implementa Razor automáticamente al renderizar variables con `@variable` para prevenir ataques XSS?
    - A) CSRF Protection
    - B) HTML Encoding
    - C) SQL Injection Prevention
    - D) URL Encoding

29. ¿Cómo se escribe un comentario en Razor que no se envía al cliente?
    - A) `<!-- comentario -->`
    - B) `// comentario`
    - C) `@* comentario *@`
    - D) `/* comentario */`

30. ¿Qué característica de Razor permite definir una plantilla base (layout) que las vistas hijas pueden extender?
    - A) Partial Views
    - B) View Components
    - C) Layouts (_Layout.cshtml)
    - D) Tag Helpers

31. Si en Razor se utiliza un bucle `@foreach` y la colección está vacía, ¿qué construcción permite mostrar un mensaje alternativo?
    - A) `@foreach else`
    - B) `@if (Model.Count == 0)`
    - C) `@empty`
    - D) `@default`

32. Dentro de un `@foreach` en Razor, ¿cómo se puede acceder al índice actual de la iteración?
    - A) Usando `@item. Index`
    - B) Usando una variable contador manual
    - C) Usando `Select((item, index) => ...)`
    - D) No es posible acceder al índice en Razor

33. ¿Qué Tag Helper de ASP.NET Core se utiliza para generar enlaces (`<a>`) a acciones de controladores de forma tipada?
    - A) `<a href-action="Index">`
    - B) `<a asp-action="Index">`
    - C) `<a route-action="Index">`
    - D) `<a link-action="Index">`

34. ¿Cuál es el operador que se utiliza en Razor para concatenar cadenas de texto en expresiones C#?
    - A) `&`
    - B) `~`
    - C) `+`
    - D) `.Concat()`

35. ¿Qué operador de C# se puede usar en Razor para proporcionar un valor por defecto si una variable es null?
    - A) `|default`
    - B) `??`
    - C) `?.`
    - D) `||`

36. Si necesitas insertar una vista parcial reutilizable (como un navbar) dentro de otra vista Razor, ¿qué helper usarías?
    - A) `@Html.Extend()`
    - B) `@Html.Include()`
    - C) `@await Html.PartialAsync()`
    - D) `@Html. Macro()`

37. ¿Qué son los View Components en ASP.NET Core? 
    - A) Componentes de JavaScript reutilizables
    - B) Componentes Razor reutilizables con lógica propia similar a mini-controladores
    - C) Clases de servicio inyectables
    - D) Entidades de base de datos

38. ¿Dónde debe residir la lógica de negocio compleja según las mejores prácticas de Razor?
    - A) En la vista Razor usando código C# inline
    - B) En el Controller o Service Layer
    - C) En Tag Helpers personalizados
    - D) En archivos JavaScript

39. ¿Qué directiva Razor se usa para inyectar servicios directamente en una vista?
    - A) `@using`
    - B) `@inject`
    - C) `@model`
    - D) `@service`

40. ¿Cuál es el propósito del archivo `_ViewStart.cshtml`?
    - A) Iniciar la aplicación
    - B) Definir código que se ejecuta antes de renderizar cada vista (ej:  asignar Layout)
    - C) Importar namespaces globalmente
    - D) Registrar servicios

41. ¿Qué archivo se utiliza para importar namespaces y Tag Helpers globalmente para todas las vistas?
    - A) `_ViewStart.cshtml`
    - B) `_ViewImports.cshtml`
    - C) `appsettings.json`
    - D) `Program.cs`

42. ¿Qué Tag Helper se utiliza para vincular un formulario a un modelo y habilitar Model Binding en POST?
    - A) `<form method="post">`
    - B) `<form asp-controller="..." asp-action="...">`
    - C) `<form bind-model="...">`
    - D) `<form data-model="...">`

43. ¿Qué Tag Helper genera automáticamente campos de entrada HTML basados en las propiedades del modelo?
    - A) `<label asp-for="...">`
    - B) `<input asp-for="...">`
    - C) `<span asp-validation-for="...">`
    - D) Todas las anteriores

44. ¿Qué Tag Helper muestra mensajes de validación para una propiedad específica del modelo?
    - A) `<error asp-for="...">`
    - B) `<validation asp-for="...">`
    - C) `<span asp-validation-for="...">`
    - D) `<div asp-error="...">`

45. ¿Cómo se renderiza HTML sin codificar (raw HTML) en Razor?
    - A) `@Html.Display(html)`
    - B) `@Html.Raw(html)`
    - C) `@(html)`
    - D) `@{= html }`

---

## III. Model Binding, Validación y Formularios (Preguntas 46-65)

46. ¿Qué anotación de Data Annotations indica que una propiedad es obligatoria? 
    - A) `[NotNull]`
    - B) `[Required]`
    - C) `[Mandatory]`
    - D) `[NotEmpty]`

47. ¿Qué anotación limita la longitud máxima de una cadena de texto? 
    - A) `[MaxLength(100)]`
    - B) `[Length(100)]`
    - C) `[StringLength(100)]`
    - D) A y C son correctas

48. ¿Qué anotación valida que un valor numérico esté dentro de un rango específico? 
    - A) `[Between(1, 100)]`
    - B) `[Range(1, 100)]`
    - C) `[MinMax(1, 100)]`
    - D) `[Limit(1, 100)]`

49. ¿Qué anotación valida que una cadena cumpla con el formato de dirección de correo electrónico?
    - A) `[Email]`
    - B) `[EmailAddress]`
    - C) `[ValidEmail]`
    - D) `[EmailFormat]`

50. Para activar la validación en un método POST del Controller, ¿qué anotación debe preceder al parámetro del modelo?
    - A) `[Validate]`
    - B) `[Valid]`
    - C) `[Check]`
    - D) Validación se activa automáticamente con Model Binding

51. ¿Qué propiedad del Controller se verifica para determinar si el modelo pasó todas las validaciones?
    - A) `Model.IsValid`
    - B) `ModelState.IsValid`
    - C) `Validation.IsValid`
    - D) `Request.IsValid`

52. Si `ModelState.IsValid` es `false`, ¿qué acción se debe tomar típicamente?
    - A) Lanzar una excepción
    - B) Devolver la vista con el modelo para mostrar errores
    - C) Redirigir a una página de error
    - D) Ignorar los errores y guardar de todos modos

53. ¿Qué helper HTML muestra un resumen de todos los errores de validación del modelo?
    - A) `@Html.ErrorSummary()`
    - B) `@Html.ValidationSummary()`
    - C) `@Html.Errors()`
    - D) `@Html.ShowErrors()`

54. ¿Qué anotación permite especificar un mensaje de error personalizado para una validación? 
    - A) `[Message("... ")]`
    - B) `[Error("...")]`
    - C) `[ErrorMessage("...")]`
    - D) Todas las anotaciones aceptan `ErrorMessage = "... "`

55. ¿Qué librería de validación usa ASP.NET Core por defecto cuando se utilizan Data Annotations?
    - A) jQuery Validation
    - B) FluentValidation
    - C) DataAnnotations.Validator
    - D) System.ComponentModel.DataAnnotations

56. ¿Qué técnica se usa para validar relaciones complejas o reglas de negocio que no se pueden expresar con Data Annotations simples?
    - A) Custom Validation Attributes
    - B) IValidatableObject
    - C) FluentValidation
    - D) Todas las anteriores

57. ¿Cómo se puede implementar validación del lado del cliente en ASP.NET Core MVC?
    - A) Incluyendo jQuery Validation y Unobtrusive Validation scripts
    - B) Usando Tag Helpers que generan atributos `data-val-*`
    - C) Habilitando validación en `_ValidationScriptsPartial. cshtml`
    - D) Todas las anteriores

58. ¿Qué método se usa para agregar un error de validación manualmente en el Controller?
    - A) `ModelState.AddError("key", "message")`
    - B) `ModelState.AddModelError("key", "message")`
    - C) `Validation.Add("key", "message")`
    - D) `Errors.Add("key", "message")`

59. ¿Qué anotación valida que una propiedad string contenga solo caracteres alfabéticos?
    - A) `[Alpha]`
    - B) `[LettersOnly]`
    - C) `[RegularExpression(@"^[a-zA-Z]+$")]`
    - D) `[AlphaNumeric]`

60. ¿Qué es un InputModel en el contexto de ASP.NET Core MVC? 
    - A) Un modelo de base de datos (Entity)
    - B) Un DTO diseñado específicamente para recibir datos de formularios
    - C) Un ViewModel para mostrar datos
    - D) Un servicio inyectable

61. ¿Por qué es recomendable usar InputModels separados en lugar de usar directamente las entidades de base de datos en formularios?
    - A) Para evitar Over-Posting y Mass Assignment vulnerabilities
    - B) Para separar responsabilidades
    - C) Para tener validaciones específicas del formulario
    - D) Todas las anteriores

62. ¿Qué patrón de programación funcional se recomienda en el curso para manejar errores sin excepciones?
    - A) Option Pattern
    - B) Railway Oriented Programming (Result<T>)
    - C) Either Monad
    - D) Try-Catch Chains

63. ¿Qué librería se usa en el curso para implementar Railway Oriented Programming en C#?
    - A) LanguageExt
    - B) CSharpFunctionalExtensions
    - C) Optional
    - D) ErrorOr

64. ¿Qué método de `Result<T>` se utiliza para transformar el valor contenido en caso de éxito?
    - A) `Bind()`
    - B) `Map()`
    - C) `OnSuccess()`
    - D) `Then()`

65. En Railway Oriented Programming, ¿qué método se usa para encadenar operaciones que también devuelven un `Result<T>`?
    - A) `Map()`
    - B) `Bind()`
    - C) `Then()`
    - D) `FlatMap()`

---

## IV. Estado, Sesiones y Autenticación (Preguntas 66-80)

66. ¿Dónde se almacenan los datos de sesión en ASP.NET Core por defecto?
    - A) En cookies del cliente
    - B) En memoria del servidor
    - C) En base de datos
    - D) En archivos del servidor

67. ¿Qué middleware debe agregarse en Program.cs para habilitar el uso de sesiones?
    - A) `app.UseSession()`
    - B) `app.UseSessionState()`
    - C) `app.EnableSessions()`
    - D) `builder.Services.AddSession()`

68. ¿Qué método se usa para almacenar un valor string en la sesión?
    - A) `HttpContext.Session.SetString("key", "value")`
    - B) `Session["key"] = "value"`
    - C) `Session.Add("key", "value")`
    - D) `Session.Set("key", "value")`

69. ¿Qué método se usa para recuperar un valor string de la sesión?
    - A) `HttpContext.Session.GetString("key")`
    - B) `Session["key"]`
    - C) `Session.Get("key")`
    - D) `Session. Retrieve("key")`

70. ¿Cómo se almacenan objetos complejos (no primitivos) en la sesión de ASP.NET Core?
    - A) Directamente con Session. Set()
    - B) Serializándolos a JSON y almacenándolos como string
    - C) No se pueden almacenar objetos complejos
    - D) Usando Session.SetObject()

71. ¿Qué diferencia hay entre TempData y Session? 
    - A) TempData persiste solo para la siguiente petición, Session persiste durante toda la sesión
    - B) TempData es más rápido que Session
    - C) TempData se almacena en el cliente, Session en el servidor
    - D) No hay diferencia

72. ¿Qué provider de autenticación se configura con `AddCookie()` en ASP.NET Core?
    - A) JWT Bearer Authentication
    - B) Cookie-based Authentication
    - C) OAuth Authentication
    - D) Windows Authentication

73. ¿Qué método se llama para iniciar sesión de un usuario (crear un ClaimsPrincipal)?
    - A) `HttpContext.SignInAsync()`
    - B) `HttpContext.Login()`
    - C) `HttpContext.Authenticate()`
    - D) `User.Login()`

74. ¿Qué método se llama para cerrar sesión de un usuario?
    - A) `HttpContext.SignOutAsync()`
    - B) `HttpContext. Logout()`
    - C) `HttpContext.EndSession()`
    - D) `User.SignOut()`

75. ¿Qué anotación se usa para restringir el acceso a una acción solo a usuarios con un rol específico?
    - A) `[Authorize(Role = "Admin")]`
    - B) `[Authorize(Roles = "Admin")]`
    - C) `[RequireRole("Admin")]`
    - D) `[Role("Admin")]`

76. ¿Qué propiedad del Controller proporciona información sobre el usuario autenticado actual?
    - A) `HttpContext.User`
    - B) `User`
    - C) Ambas A y B
    - D) `CurrentUser`

77. ¿Qué método se usa para verificar si el usuario actual pertenece a un rol específico? 
    - A) `User.HasRole("Admin")`
    - B) `User.IsInRole("Admin")`
    - C) `User.CheckRole("Admin")`
    - D) `User.Role == "Admin"`

78. ¿Qué es un Claim en el contexto de autenticación en ASP.NET Core?
    - A) Una solicitud HTTP
    - B) Un dato sobre el usuario (nombre, email, rol, etc.)
    - C) Una sesión activa
    - D) Un permiso de autorización

79. ¿Qué interfaz se implementa en ASP.NET Core Identity para hashear contraseñas de forma segura?
    - A) `IPasswordHasher<TUser>`
    - B) `IHashService`
    - C) `ICryptoProvider`
    - D) `ISecurityHasher`

80. ¿Qué política de autorización permite definir requisitos complejos más allá de simples roles?
    - A) Role-based Authorization
    - B) Policy-based Authorization
    - C) Claim-based Authorization
    - D) Token-based Authorization

---

## V. Blazor Server (Preguntas 81-95)

81. ¿Qué es Blazor Server? 
    - A) Un framework para crear aplicaciones de consola
    - B) Un framework para crear aplicaciones web interactivas con C# ejecutándose en el servidor
    - C) Un reemplazo de ASP.NET Core MVC
    - D) Un ORM para bases de datos

82. ¿Qué protocolo usa Blazor Server para mantener la comunicación en tiempo real entre cliente y servidor?
    - A) HTTP/2
    - B) WebSockets (SignalR)
    - C) gRPC
    - D) REST

83. ¿Cuál es la extensión de archivo de los componentes Blazor? 
    - A) `.cshtml`
    - B) `.razor`
    - C) `.blazor`
    - D) `.component`

84. ¿Qué directiva se usa en un componente Blazor para definir parámetros que pueden pasarse desde el componente padre?
    - A) `@param`
    - B) `@input`
    - C) `[Parameter]`
    - D) `@bind`

85. ¿Qué directiva Blazor se usa para enlace bidireccional de datos (two-way binding)?
    - A) `@model`
    - B) `@bind`
    - C) `@value`
    - D) `@two-way`

86. ¿Qué directiva se usa para manejar eventos de clic en Blazor? 
    - A) `@click`
    - B) `@onclick`
    - C) `(click)`
    - D) `onclick=""`

87. ¿Qué bloque se usa en un componente Blazor para escribir código C#?
    - A) `@functions { }`
    - B) `@code { }`
    - C) `<script> </script>`
    - D) `@{ }`

88. ¿Qué método de ciclo de vida de Blazor se ejecuta cuando el componente se inicializa por primera vez?
    - A) `OnInit()`
    - B) `OnInitialized()` o `OnInitializedAsync()`
    - C) `OnLoad()`
    - D) `OnCreate()`

89. ¿Qué método se llama manualmente para forzar el re-renderizado de un componente Blazor?
    - A) `Refresh()`
    - B) `Update()`
    - C) `StateHasChanged()`
    - D) `Render()`

90. ¿Cómo se inyecta un servicio en un componente Blazor?
    - A) `@inject IService Service`
    - B) `[Inject] public IService Service { get; set; }`
    - C) Ambas A y B
    - D) `@service IService Service`

91. ¿Qué ventaja tiene Blazor Server sobre Blazor WebAssembly? 
    - A) Funciona offline
    - B) Menor latencia (todo se ejecuta en el cliente)
    - C) Menor tamaño de descarga inicial
    - D) No requiere conexión persistente al servidor

92. ¿Qué desventaja tiene Blazor Server comparado con Blazor WebAssembly?
    - A) Mayor tamaño de descarga inicial
    - B) Requiere conexión persistente al servidor (no funciona offline)
    - C) No soporta C#
    - D) No tiene reactividad

93. ¿Cómo se pasa un parámetro a un componente hijo en Blazor? 
    - A) `<ComponenteHijo Parametro="valor" />`
    - B) `<ComponenteHijo @bind-Parametro="valor" />`
    - C) `<ComponenteHijo param-Parametro="valor" />`
    - D) `<ComponenteHijo [Parametro]="valor" />`

94. ¿Qué se debe hacer cuando un componente Blazor usa recursos que deben liberarse (timers, suscripciones)?
    - A) Implementar `IDisposable`
    - B) No es necesario, se libera automáticamente
    - C) Llamar a `Dispose()` manualmente en `OnInitialized()`
    - D) Usar el método `OnDestroy()`

95. ¿Qué Tag Helper se usa para integrar un componente Blazor dentro de una vista Razor MVC?
    - A) `<blazor component="..." />`
    - B) `<component type="typeof(... )" render-mode="..." />`
    - C) `@component(... )`
    - D) `<razor-component name="..." />`

---

## VI. Entity Framework Core y Testing (Preguntas 96-100)

96. ¿Qué es Entity Framework Core? 
    - A) Un framework de UI
    - B) Un ORM (Object-Relational Mapper)
    - C) Un motor de plantillas
    - D) Un servidor web

97. ¿Qué clase se hereda para definir un contexto de base de datos en Entity Framework Core?
    - A) `Database`
    - B) `DbContext`
    - C) `EntityContext`
    - D) `DataContext`

98. ¿Qué comando se usa para crear una nueva migración en Entity Framework Core?
    - A) `dotnet ef migrations create NombreMigracion`
    - B) `dotnet ef migrations add NombreMigracion`
    - C) `dotnet ef database migrate`
    - D) `dotnet ef migration new NombreMigracion`

99. ¿Qué comando aplica las migraciones pendientes a la base de datos? 
    - A) `dotnet ef migrations apply`
    - B) `dotnet ef database migrate`
    - C) `dotnet ef database update`
    - D) `dotnet ef apply`

100. ¿Qué librería se usa en el curso para crear mocks de servicios en tests unitarios?
     - A) NSubstitute
     - B) Moq
     - C) FakeItEasy
     - D) Mockito

---
