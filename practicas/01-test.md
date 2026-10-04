**Test: Desarrollo de Páginas Web Dinámicas en .NET**

**Instrucciones:** Lee cada pregunta cuidadosamente y selecciona la opción que consideres correcta.

---

## PARTE 1 (Puntos 00-12)

**Punto 00: Comandos de la CLI de .NET**

1.  ¿Qué comando crea un proyecto Razor Pages vacío con la CLI de .NET?
    A) `dotnet new mvc -n MiApp`
    B) `dotnet new razorpage -n MiApp`
    C) `dotnet new page -n MiApp`
    D) `dotnet new web -n MiApp`

2.  Ejecutas `dotnet run` y el proyecto se compila automáticamente. ¿Qué comando usarías si solo quieres reconstruir sin arrancar la aplicación?
    A) `dotnet build`
    B) `dotnet restore`
    C) `dotnet clean`
    D) `dotnet publish`

3.  Guardas un secreto con `dotnet user-secrets set "Email:Clave" "abc"`. ¿Dónde se guarda realmente ese valor?
    A) En `appsettings.json` del proyecto
    B) En un fichero `.env` de la raíz
    C) En la carpeta de usuario, fuera del repositorio
    D) En la memoria del proceso hasta que se reinicia

**Punto 01: Fundamentos de Razor**

4.  ¿Qué es Razor en ASP.NET Core?
    A) Un motor de plantillas del servidor que genera HTML a partir de C#
    B) Un lenguaje de estilos para el navegador
    C) Un ORM para bases de datos
    D) Un servidor web alternativo a Kestrel

5.  Escribes `@producto.Nombre` dentro de un `.cshtml`. ¿Qué produce esa expresión?
    A) La cadena literal "@producto.Nombre"
    B) El valor de la propiedad `Nombre` insertado en el HTML, escapado
    C) Un error de compilación porque falta `@model`
    D) La definición de una variable JavaScript

6.  ¿Qué hace Razor con el contenido HTML que escribiste si proviene de un campo con `<script>`?
    A) Lo inserta tal cual para no romper la página
    B) Lo escapa, de modo que se pinta como texto y no se ejecuta
    C) Lo elimina del HTML sin avisar
    D) Lo convierte en un comentario

7.  ¿Qué resultado produce `@Html.Raw("<b>Oferta</b>")`?
    A) El texto `&lt;b&gt;Oferta&lt;/b&gt;`
    B) Un error porque `Html.Raw` no existe en Razor Pages
    C) El HTML `<b>Oferta</b>` interpretado por el navegador
    D) La cadena literal con las arrobas

**Punto 02: Directivas y Sintaxis**

8.  ¿Qué directiva convierte un `.cshtml` en una página con URL propia?
    A) `@model`
    B) `@page`
    C) `@using`
    D) `@inject`

9.  ¿Para qué sirve la directiva `@model Producto` en una vista?
    A) Para declarar qué tipo de objeto recibe la vista desde el servidor
    B) Para crear la tabla `Productos` en la base de datos
    C) Para importar el espacio de nombres de Productos
    D) Para registrar un servicio en la inyección de dependencias

10. ¿Qué directiva inyecta un servicio en la vista desde la inyección de dependencias?
    A) `@inject`
    B) `@functions`
    C) `@section`
    D) `@inherits`

11. ¿Dónde vive una variable declarada con `@{ var total = 10; }` en una vista?
    A) En todas las vistas del proyecto
    B) Solo en esa vista, al compilar su plantilla
    C) En la sesión del usuario
    D) En la configuración de la aplicación

**Punto 03: Estructuras de Control**

12. ¿Cuál de estas estructuras pinta un bloque de HTML solo si el carrito tiene líneas?
    A) `@for (var i = 0; i < 10; i++)`
    B) `@if (carrito.Lineas.Any())`
    C) `@switch (carrito.Estado)`
    D) `@while (carrito.Lineas.Count > 0)`

13. En una vista, ¿qué produce un `@foreach (var linea in Model.Lineas)`?
    A) Una comprobación única de la primera línea
    B) Un bloque que se repite por cada línea del modelo
    C) Un bucle infinito si la lista está vacía
    D) Una ordenación automática de las líneas

14. ¿Cuál es la diferencia entre `@for` y `@foreach` en una vista?
    A) No hay diferencia, son sinónimos
    B) `@for` recorre un rango con índice; `@foreach` recorre una colección
    C) `@foreach` solo funciona con matrices
    D) `@for` no puede pintar HTML dentro

15. ¿Qué estructura usarías para pintar distintos mensajes según el estado del stock (agotado, bajo, disponible)?
    A) `@while`
    B) `@switch` sobre el estado
    C) `@foreach` sobre las categorías
    D) `@try` con captura de errores

**Punto 04: Funciones y Métodos en la Vista**

16. ¿Qué directiva declara métodos dentro de la propia vista?
    A) `@functions`
    B) `@model`
    C) `@section`
    D) `@inherits`

17. ¿Cuál es la principal ventaja de una función local en una vista?
    A) Permite repetir la misma lógica en varios puntos de la plantilla sin duplicarla
    B) Hace que la vista sea más rápida que el controlador
    C) Sustituye a la base de datos
    D) Permite usar SQL dentro de la vista

18. ¿Qué debe devolver una función de vista que se usa dentro del HTML?
    A) Siempre un objeto de base de datos
    B) Un valor o marcado HTML que la vista inserta donde se llama
    C) Una respuesta HTTP completa
    D) Un servicio registrado en DI

**Punto 05: Layouts, Parciales y Componentes**

19. ¿Qué fichero se ejecuta en todas las vistas para declarar el layout común?
    A) `_Layout.cshtml`
    B) `_ViewStart.cshtml`
    C) `_ViewImports.cshtml`
    D) `Index.cshtml`

20. ¿Para qué sirve `_ViewImports.cshtml`?
    A) Para declarar los `@using` y `@inject` compartidos por las vistas
    B) Para definir el HTML del pie de página
    C) Para conectar la vista con la base de datos
    D) Para compilar las plantillas

21. ¿Cuál es la forma declarativa, con Tag Helper, de insertar una vista parcial dentro de otra vista?
    A) Con `<vc:nombre-componente />`
    B) Con `<partial name="_Parcial" />`
    C) Con `@section`
    D) Con `@model _Parcial`

22. ¿Cuál es la diferencia entre una vista parcial y un componente de vista?
    A) La parcial es solo HTML; el componente tiene su propia clase con lógica e inyección
    B) No hay diferencia
    C) El componente solo funciona en MVC y la parcial solo en páginas
    D) La parcial se cachea y el componente no

**Punto 06: Tag Helpers**

23. ¿Qué son los Tag Helpers?
    A) Etiquetas HTML que el servidor transforma antes de mandar la respuesta
    B) Fragmentos de CSS generados por el servidor
    C) Funciones JavaScript del navegador
    D) Directivas de Razor compiladas a C#

24. ¿Qué atributo usa el Tag Helper de enlaces para generar una URL correcta a una página?
    A) `href` a mano con la ruta escrita
    B) `asp-page` (o `asp-controller` y `asp-action` en MVC)
    C) `data-url`
    D) `route`

25. ¿Qué pareja de Tag Helpers asocia un `<label>` con su campo y pinta los avisos de validación?
    A) `asp-items` y `asp-selected`
    B) `asp-for` y `asp-validation-for`
    C) `asp-area` y `asp-page`
    D) `asp-page` y `asp-route`

26. ¿Cómo registras un Tag Helper personalizado en una vista?
    A) Con `@addTagHelper *, MiEnsamblo` en `_ViewImports.cshtml`
    B) Con `@inject` en la vista
    C) Con `@functions`
    D) No hace falta registrarlo, se detecta solo

**Punto 07: Arquitectura MVC**

27. En el patrón MVC, ¿qué pieza decide qué vista se pinta?
    A) El modelo
    B) El controlador
    C) La vista
    D) El repositorio

28. ¿Cuál es la responsabilidad del modelo en MVC?
    A) Pintar el HTML de la respuesta
    B) Representar los datos y la lógica de negocio
    C) Enrutar las peticiones HTTP
    D) Gestionar la inyección de dependencias

29. ¿Qué garantiza la separación de capas en MVC?
    A) Que la vista pueda consultar la base de datos directamente
    B) Que cada pieza tenga su responsabilidad y se pueda probar por separado
    C) Que el controlador pinte HTML
    D) Que no haga falta escribir clases

30. ¿Qué es ProductosApp en la unidad?
    A) Un motor de plantillas
    B) El hilo conductor de la unidad, la misma tienda construida en las dos visiones
    C) Un paquete NuGet de Microsoft
    D) Una base de datos de ejemplo

**Punto 08: Controladores y Acciones**

31. ¿Qué devuelven las acciones de un controlador normalmente?
    A) `IActionResult` (vista, JSON, redirección o error)
    B) Solo cadenas HTML
    C) Solo objetos de base de datos
    D) Respuestas SOAP

32. Si una acción hace `return RedirectToAction("Index")` tras procesar un formulario, ¿qué patrón aplica?
    A) PRG: redirige tras el POST para no repetir el envío con F5
    B) Cacheado de la respuesta
    C) Autenticación del usuario
    D) Compresión del HTML

33. ¿Qué código devuelve una acción cuando no encuentra el recurso solicitado por id?
    A) `200` con la vista vacía
    B) `NotFound()` (404)
    C) `RedirectToAction`
    D) `Ok()`

34. ¿Para qué sirve TempData en un controlador?
    A) Para llevar un dato desde una acción hasta la vista siguiente, viviendo dos peticiones
    B) Para guardar la configuración de la aplicación
    C) Para cachear la respuesta entera
    D) Para autenticar al usuario

**Punto 09: ViewModels**

35. ¿Qué es un ViewModel?
    A) Un objeto con la forma exacta que pide cada vista, montado por el controlador
    B) Una vista que se ejecuta como modelo
    C) Un repositorio de datos
    D) Un componente de CSS

36. ¿Por qué no se pinta la entidad de negocio directamente en la vista?
    A) Porque las entidades no existen en C#
    B) Porque expone datos internos que la vista no necesita y acopla la capa de presentación a la persistencia
    C) Porque las vistas solo admiten cadenas
    D) Porque el ViewModel es más rápido que la entidad

37. ¿Qué aporta la inmutabilidad a un ViewModel?
    A) Que la vista pueda modificarlo después de pintarlo
    B) Que el objeto no cambie tras crearse y sea seguro de compartir
    C) Que desaparezca de la memoria automáticamente
    D) Que se guarde en la base de datos

**Punto 10: Razor Pages: Fundamentos**

38. ¿Qué convierte un `.cshtml` en una página con URL propia en Razor Pages?
    A) La carpeta `Views`
    B) La directiva `@page`
    C) El atributo `[Route]`
    D) La clase `Controller`

39. ¿Cómo se traduce la carpeta `Pages/Productos/Alta.cshtml` en una URL?
    A) `/Productos/Alta` (la ruta sale de la carpeta)
    B) `/Alta` siempre
    C) `/Pages/Productos/Alta` siempre
    D) No tiene URL, se llama por código

40. ¿Qué es un PageModel?
    A) La clase C# detrás de una página Razor Pages, con sus datos y sus handlers
    B) Una plantilla HTML con CSS
    C) Un controlador sin acciones
    D) Un tipo de base de datos

41. ¿Cuál es la gran diferencia entre Razor Pages y MVC desde el punto de vista de la organización?
    A) MVC no puede usar vistas
    B) Razor Pages organiza por páginas (lógica pegada a la vista) y MVC por controladores
    C) Razor Pages no admite modelos
    D) MVC es más moderno y Razor Pages está obsoleto

**Punto 11: PageModel y Handlers**

42. ¿Qué handler se ejecuta cuando el usuario entra en una página con un `GET`?
    A) `OnPost`
    B) `OnGet`
    C) `OnPut`
    D) `OnDelete`

43. ¿Qué handler se ejecuta cuando un formulario de la página se envía con `POST`?
    A) `OnGet`
    B) `OnPostAsync`
    C) `OnRender`
    D) `OnInit`

44. ¿Qué resultado devuelve `return RedirectToPage("/Productos/Listado")` desde un `OnPost`?
    A) La vista de la página actual
    B) Una redirección a la página indicada
    C) Un error 404
    D) Un JSON con los datos

45. Una página `@page` sin `@model` se pinta en el navegador. ¿Qué le ocurre a su `OnGet`?
    A) Se ejecuta igual que con modelo
    B) No se ejecuta: la página se pinta pero el handler nunca corre
    C) Se ejecuta dos veces
    D) Compila como controlador

**Punto 12: MVC frente a Razor Pages**

46. ¿Cuál de estas afirmaciones sobre las dos visiones es correcta?
    A) MVC es más potente y Razor Pages es una versión recortada
    B) Las dos resuelven lo mismo; cambia la forma de organizar la lógica
    C) Razor Pages no puede usar inyección de dependencias
    D) MVC no admite modelos

47. ¿Qué ocurre si declaras una ruta igual en un controlador y en una página Razor Pages en el mismo proyecto?
    A) Se ejecutan las dos a la vez
    B) Hay colisión de rutas y solo responde una
    C) ASP.NET Core las fusiona automáticamente
    D) La página siempre gana

48. ¿Cuándo conviene convivir las dos visiones en un mismo `Program.cs`?
    A) Nunca, es incompatible
    B) Cuando un sitio migrado convive con partes nuevas mientras no se termina la migración
    C) Solo en aplicaciones de una sola página
    D) Solo si se usa Blazor

49. Al migrar una acción de MVC a una página, ¿qué se hace primero?
    A) Se reescribe todo el proyecto
    B) Se migra vista a vista comprobando que la nueva página responde igual que la acción
    C) Se borra el controlador entero
    D) Se cambia la base de datos

## PARTE 2 (Puntos 13-25)

**Punto 13: Formularios Web**

50. ¿Qué elemento HTML envía un formulario al servidor?
    A) `<input type="submit">` o `<button type="submit">` dentro de un `<form>`
    B) `<a href>`
    C) `<div onclick>`
    D) `<label>`

51. ¿Cuál es el objetivo del patrón PRG (Post-Redirect-Get)?
    A) Enviar el formulario dos veces
    B) Tras procesar el POST, redirigir para que F5 no repita la operación
    C) Comprimir el HTML del formulario
    D) Autenticar antes de pintar

52. ¿Qué Tag Helper rellena automáticamente el atributo `asp-for` de un campo?
    A) El Tag Helper de enlaces
    B) El Tag Helper de formularios (`asp-for` asocia el campo con la propiedad del modelo)
    C) El Tag Helper de imagen
    D) El Tag Helper de zona

53. ¿Qué le pasa al servidor si un campo con `required` llega vacío porque el navegador no validó?
    A) Lo acepta y guarda un valor vacío
    B) La validación en servidor lo rechaza y el formulario vuelve con los errores
    C) Se produce un error 500 siempre
    D) El campo se rellena con un valor por defecto

**Punto 14: Model Binding**

54. ¿Qué es el model binding?
    A) El enlace automático de los datos de la petición (ruta, query, formulario) a los parámetros del código
    B) La conexión con la base de datos
    C) La plantilla de la vista
    D) La caché de la respuesta

55. Un campo del formulario se llama `producto.Nombre`. ¿Qué propiedad rellena?
    A) `Nombre` de un objeto `producto` enlazado como parámetro
    B) Una propiedad llamada literalmente `producto.Nombre`
    C) Nada, esos nombres no se enlazan
    D) La variable de sesión `producto`

56. ¿Qué fuente alimenta los parámetros de una acción cuando la URL es `/Productos/Detalle/5`?
    A) La query string
    B) La ruta
    C) El cuerpo del formulario
    D) La cookie de sesión

57. ¿Por qué puede fallar el enlace de un campo `precio` con valor `3.50` en un equipo en español?
    A) Porque `decimal` no se admite en formularios
    B) Porque la cultura espera coma decimal y el enlace no convierte el punto
    C) Porque el campo debe llamarse `precioReferencia`
    D) Porque falta el atributo `required`

**Punto 15: Validaciones y Seguridad**

58. ¿Qué atributo marca un campo como obligatorio en el modelo?
    A) `[Required]`
    B) `[HttpGet]`
    C) `[Route]`
    D) `[Bind]`

59. ¿Cuál de estos ataques consiste en que otra web dispare un `POST` contra tu aplicación usando la cookie que ya tienes?
    A) SQL injection
    B) Cross-Site Request Forgery (CSRF)
    C) Cross-Site Scripting (XSS)
    D) Path traversal

60. ¿Cómo defiendes tus formularios del CSRF en Razor Pages y en MVC?
    A) Con el token antifalsificación (automático en páginas y atributo en MVC)
    B) Con `Html.Raw`
    C) Con la caché de salida
    D) Con la compresión

61. ¿Qué ocurre si pintas un dato del usuario con `@Html.Raw` en un listado?
    A) Se escapa como texto
    B) El navegador puede ejecutar el script incrustado (XSS)
    C) La vista no compila
    D) Se guarda en la sesión

**Punto 16: Ficheros y Almacenamiento**

62. ¿Qué tipo recibe el servidor cuando se sube un fichero en un formulario?
    A) `string`
    B) `IFormFile`
    C) `FileStream`
    D) `FileInfo`

63. ¿Qué atributo necesita el `<form>` para poder subir ficheros?
    A) `method="post"` y `enctype="multipart/form-data"`
    B) `type="file"` solo
    C) `accept="image/*"` solo
    D) Ninguno, se sube solo

64. ¿Qué es el path traversal y con qué se corta?
    A) Un error de rutas de ASP.NET Core; se corta validando que la ruta resuelta queda dentro del directorio permitido
    B) Un ataque que escapa del directorio de subidas con rutas como `..`; se corta validando ruta final y nombre generado por el servidor
    C) Un fallo de la caché; se corta con Redis
    D) Un problema de compresión; se corta con Brotli

**Punto 17: Gestión del Estado**

65. ¿Por qué HTTP se describe como un protocolo sin estado?
    A) Porque no soporta HTTPS
    B) Porque cada petición es independiente: el servidor no recuerda las anteriores por sí solo
    C) Porque no admite cookies
    D) Porque solo trabaja con GET

66. ¿Qué dato vive exactamente dos peticiones y viaja en cookie por defecto?
    A) TempData
    B) ViewData
    C) ViewBag
    D) La configuración

67. ¿Cuál es la diferencia entre ViewData y ViewBag?
    A) ViewData es un diccionario tipado por clave; ViewBag es un envoltorio dinámico sobre el mismo almacén
    B) ViewData solo existe en MVC y ViewBag solo en páginas
    C) ViewData se guarda en cookie y ViewBag en sesión
    D) No hay diferencia real

68. Un usuario añade un producto y se le redirige al listado con el aviso "añadido". ¿Qué mecanismo lleva ese aviso?
    A) ViewData, porque vive mucho
    B) TempData, que sobrevive al redirect y se pinta una vez
    C) La configuración de la aplicación
    D) El layout

**Punto 18: Cookies y Sesiones**

69. ¿Qué es una cookie en el contexto de una web?
    A) Un par `nombre=valor` que el servidor pide guardar y el navegador devuelve en cada petición
    B) Un fichero del servidor
    C) Una variable de C# en memoria
    D) Una conexión con la base de datos

70. ¿Qué atributo impide que JavaScript lea una cookie desde el navegador?
    A) `Expires`
    B) `HttpOnly`
    C) `Secure`
    D) `Path`

71. ¿Qué viaja realmente en la cookie `.AspNetCore.Session`?
    A) Todos los datos de la sesión
    B) Un identificador protegido; los datos viven en el servidor
    C) La identidad del usuario
    D) El token antifalsificación

72. ¿Qué ocurre si cambias un carácter del valor de la cookie de sesión?
    A) Nada, el servidor la acepta igual
    B) El servidor no encuentra datos detrás de esa llave y monta una sesión nueva
    C) Se borra la base de datos
    D) La aplicación se reinicia

**Punto 19: Autenticación con Identity**

73. ¿Qué registra `AddIdentity` en la aplicación?
    A) Usuarios, accesos y roles tal y como los trae el framework
    B) Solo una cookie manual
    C) Un ORM
    D) Un servidor web

74. ¿Qué gestor se encarga de comprobar credenciales y crear la cookie de identidad?
    A) `UserManager`
    B) `SignInManager`
    C) `RoleManager`
    D) `IOptionsManager`

75. ¿Qué respuesta recibe un usuario identificado sin el rol que exige `[Authorize(Roles = "Admin")]`?
    A) 200 con la vista de administración
    B) 302 a la página de acceso denegado
    C) 404
    D) 500

76. ¿Qué papel juega la cookie de identidad en cada petición?
    A) Se usa solo en el login
    B) El servidor la desprotege y reconstruye el usuario antes de decidir si la ruta se abre
    C) Guarda los datos de la sesión
    D) Comprime la respuesta

**Punto 20: Configuración y Entornos**

77. ¿Qué fichero define los valores comunes a todos los entornos?
    A) `appsettings.Development.json`
    B) `appsettings.json`
    C) `.env`
    D) `launchSettings.json` es de desarrollo, no el común

78. Si una clave aparece en `appsettings.json` y también en `appsettings.Production.json`, ¿cuál gana en producción?
    A) La del fichero base
    B) La de `appsettings.Production.json`
    C) Ninguna, la aplicación falla al arrancar
    D) La de `appsettings.Development.json`

79. ¿Qué gana si una clave está tanto en un fichero de configuración como en una variable de entorno?
    A) El fichero de configuración
    B) La variable de entorno
    C) Se produce un error de compilación
    D) Se queda el valor más corto

80. ¿Qué ventaja da mover el cableado de `Program.cs` a clases `XConfig` con métodos de extensión?
    A) Que la aplicación arranque más rápido
    B) Que el arranque quede legible y cada concern (datos, caché, CORS...) tenga su fichero
    C) Que no haga falta configurar nada
    D) Que se pueda borrar `Program.cs`

**Punto 21: Optimización y Rendimiento**

81. ¿Qué manda la caché de salida?
    A) Los valores calculados en un servicio
    B) La respuesta HTTP entera, devuelta sin ejecutar la página ni la acción
    C) Los datos de la base de datos
    D) El HTML del layout

82. ¿Por qué no se puede cachear con salida compartida una zona privada que pinta datos del usuario?
    A) Porque es ilegal
    B) Porque la caché no distingue usuarios y la respuesta de uno podría servirse a otro
    C) Porque la caché solo admite imágenes
    D) Porque `IMemoryCache` no funciona en producción

83. ¿Qué negocia el navegador para recibir la respuesta comprimida?
    A) La cabecera `Accept-Encoding`
    B) La cabecera `Cookie`
    C) El parámetro `?lang`
    D) La cabecera `Location`

84. ¿Cuándo tiene sentido comprimir una respuesta con Brotli o gzip?
    A) Con texto (HTML, CSS, JSON), porque gana mucho; con imágenes, no, porque ya vienen comprimidas
    B) Solo con imágenes
    C) Nunca, la red ya es rápida
    D) Solo con ficheros de más de un megabyte

**Punto 22: Internacionalización y Localización**

85. ¿Dónde viven los textos traducidos de una aplicación localizada?
    A) En las vistas, escritos a mano por idioma
    B) En ficheros `.resx` con pares clave-valor por idioma
    C) En la base de datos
    D) En las cookies del navegador

86. ¿Qué hace el proveedor de cultura de la query string (por ejemplo `?lang=en-US`)?
    A) Instala un idioma nuevo
    B) Dice al middleware qué cultura usar en esa petición
    C) Cambia el idioma del servidor
    D) Traduce la base de datos

87. Si pides `?lang=fr-FR` y la aplicación no tiene ficheros de recursos para francés, ¿qué ocurre?
    A) La aplicación falla con error 500
    B) Los textos caen en el recurso neutral (el de la casa)
    C) Se traducen automáticamente
    D) La petición se rechaza con 404

88. ¿Qué muestra `(0.21m).ToString("C", new CultureInfo("es-ES"))` frente a la misma llamada con `en-US`?
    A) `0,21 €` y `$0.21`
    B) `$0.21` y `0,21 €`
    C) El mismo texto en los dos casos
    D) Un error de compilación

**Punto 23: Herramientas, Prueba y Depuración**

89. Un error de compilación te dice `Pages/Alarma.cshtml.cs(16,1): error CS1519`. ¿Qué información te da?
    A) El código, el mensaje, el fichero y la línea donde está el problema
    B) Solo el nombre del proyecto
    C) La solución automáticamente
    D) El tiempo que tardó el build

90. ¿Qué ves en la pestaña Red de F12?
    A) El código fuente del servidor
    B) Las peticiones con su estado, cabeceras y tiempos
    C) La base de datos
    D) Los logs del servidor

91. ¿Qué diferencia hay entre la página de error en `Development` y en `Production`?
    A) Ninguna
    B) En desarrollo muestra la excepción y su traza; en producción, un aviso genérico sin detalles
    C) En producción muestra más detalle
    D) En desarrollo no se puede depurar

92. ¿Qué es `ILogger<T>`?
    A) Un depurador visual del navegador
    B) El registro de eventos de la aplicación con niveles (information, warning, error) y categorías
    C) Un validador de formularios
    D) Un cliente HTTP

**Punto 24: Pruebas y Documentación**

93. ¿Qué cubre principalmente una prueba unitaria de la lógica de presentación?
    A) La página pintada en un navegador
    B) Totales, avisos y validaciones de la lógica, sin HTTP
    C) El cableado de `Program.cs`
    D) El rendimiento del servidor

94. ¿Qué paquete de NuGet automatiza un navegador real para probar tus páginas?
    A) `Microsoft.Playwright.NUnit`
    B) `FluentAssertions`
    C) `Moq`
    D) `Serilog`

95. ¿Qué resumen devuelve `dotnet test` cuando las 15 pruebas pasan?
    A) `Superado: 15, Con error: 0`
    B) `Build succeeded`
    C) `Published`
    D) `Healthy`

96. ¿Para qué sirve el patrón AAA en una prueba?
    A) Para nombrar el fichero de la prueba
    B) Para estructurarla en preparar, ejecutar y comprobar, de modo que se lea como una frase
    C) Para compilar la aplicación
    D) Para desplegar en producción

**Punto 25: Despliegue con Docker**

97. ¿Qué entrega `dotnet publish -c Release`?
    A) Una carpeta lista para ejecutarse en el servidor, sin incluir el marco de .NET
    B) Una imagen de Docker automáticamente
    C) Un repositorio en GitHub
    D) La base de datos migrada

98. ¿Cuál es la ventaja de un Dockerfile por fases?
    A) La imagen de ejecución se queda sin el kit de desarrollo ni el código fuente
    B) La aplicación arranca sin configuración
    C) No hace falta `Program.cs`
    D) El contenedor no necesita puerto

99. En un flujo de GitHub Actions con dos trabajos encadenados por `needs`, ¿qué significa que las pruebas vayan antes?
    A) Que el segundo trabajo solo empieza si el primero termina en verde; si falla, no se publica nada
    B) Que las pruebas se ejecutan en el navegador del usuario
    C) Que la imagen se construye dos veces
    D) Que el YAML se ejecuta solo los domingos

100. Conectas tu repositorio a un servicio como Render y el servicio construye con tu Dockerfile. ¿Dónde pones las variables de entorno de producción?
    A) En el Dockerfile, con `ENV`
    B) En el panel del servicio, nunca en el repositorio
    C) En `appsettings.json` y se sube a git
    D) Dentro de la imagen con un `RUN echo`
