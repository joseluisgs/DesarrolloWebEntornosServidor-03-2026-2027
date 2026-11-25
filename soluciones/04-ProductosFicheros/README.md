# 04 - Productos Ficheros

El catálogo de los ejemplos anteriores, ahora con **imágenes que sube el usuario**: campo `IFormFile`, validación de tipo, tamaño y firma, servicio de almacenamiento con nombre único que decide el servidor, servir desde `wwwroot/uploads` y descarga con `Content-Disposition`. Todo ello en **las dos visiones** (Razor Pages y MVC) sobre la misma capa de negocio compartida.

## Descripción

Subir un fichero parece trivial hasta que preguntas lo básico: ¿de quién es el nombre?, ¿qué pasa si alguien manda un `.exe` renombrado a `.jpg`?, ¿y si manda diez gigabytes?, ¿y si la ruta del fichero intenta salirse de la carpeta? Este ejemplo responde a esas preguntas con un servicio de almacenamiento propio, y de paso enseña el formulario multipart, el techo por petición y la descarga de ficheros.

| Pieza | Dónde vive |
|-------|-----------|
| Modelo, DTOs, mapeo, datos, negocio e infraestructura | `ProductosWeb.Shared/` (compartido por las dos visiones) |
| Servicio de almacenamiento | `Shared/Services/Storage/` (`IStorageService`, `StorageService`, `StorageResult`) |
| Validación de dos capas | `Shared/ViewModels/` (DataAnnotations) + `Shared/Validators/` (FluentValidation) |
| Visión Razor Pages (puerto 5086) | `ProductosWeb.RazorPages/Pages/Productos/` |
| Visión MVC (puerto 5085) | `ProductosWeb.Mvc/Controllers/`, `ProductosWeb.Mvc/Views/` |
| Tests | `ProductosWeb.Test/` (almacenamiento, validadores, servicio y las dos visiones) |

## Las cinco pantallas

| Pantalla | MVC | Razor Pages | Qué demuestra |
|----------|-----|-------------|---------------|
| Catálogo | `GET /` | `GET /Productos` | Miniatura, enlace de descarga y `TempData` de confirmación |
| Alta | `GET/POST /Productos/Crear` | `GET/POST /Productos/Crear` | Form multipart con los seis tipos de campo, incluido el fichero |
| Edición | `GET/POST /Productos/Editar/{id}` | `GET/POST /Productos/Editar/{id}` | Imagen actual, sustitución y descarga |
| Subir | `GET/POST /Productos/Subir` | `GET/POST /Productos/Subir` | Fichero suelto para probar los rechazos sin rellenar el alta |
| Descargar | `GET /Productos/Descargar?ruta=...` | `GET /Productos/Descargar?ruta=...` | `PhysicalFile` con `Content-Disposition: attachment` |

## El formulario multipart

Un `input type=file` solo viaja si el formulario declara `enctype="multipart/form-data"`; sin él, el navegador envía el formulario como texto plano y el fichero llega vacío. El Tag Helper del form no lo pone solo, así que hay que escribirlo a mano:

```html
<form method="post" enctype="multipart/form-data">
    <input type="file" name="ImagenFile" accept=".jpg,.jpeg,.png,.gif" />
</form>
```

En el servidor, el `IFormFile` llega como propiedad del ViewModel (opcional) y el binder lo rellena con el fichero subido. El token anti-forgery sigue viajando, y en un multipart es otro campo del mismo formulario, no una cabecera.

Un detalle del binder que decide si el alta revienta o no: los campos de texto **vacíos llegan como `null`**, no como cadena vacía (esa es la regla `ConvertEmptyStringToNull`, activada por defecto). Por eso `ToEntity` recibe cada texto con `?? string.Empty`, y por eso las acciones comprueban el `ModelState` antes de convertir el modelo y de tocar el disco: un formulario a medias vuelve a la vista con errores, y no deja ficheros huérfanos en `wwwroot/uploads`.

## Las etiquetas en la base de datos

El formulario recoge etiquetas como texto libre en filas que el propio JavaScript añade y quita (`Etiquetas[0]`, `Etiquetas[1]`...). En memoria eso es una `List<string>`, pero en SQLite no hay tipo lista: hay una sola columna de texto llamada `Etiquetas`. Quien traduce una forma a otra es la conversión de valor de `ProductsDbContext.OnModelCreating`.

- **Al guardar**: las etiquetas se unen con `;` (`oferta;recomendado`), escapando dentro de cada una el `;`, la `,` y la barra invertida. Sin ese escape, una etiqueta con `;` dentro partiría la columna por la mitad.
- **Al leer**: el texto se parte por `;` y cada etiqueta se descodifica recorriéndola una sola vez. Si se encadenaran `Replace`, un `\s` que ya estaba guardado volvería a coincidir con el patrón siguiente y saldría como `;`.
- **Comparador**: a una colección con conversión le falta un comparador de valor; sin él EF Core no sabe decidir si dos listas son la misma y avisa en el arranque con el código `10620`. El `ValueComparer<List<string>>` compara elemento a elemento y guarda una copia de la lista como instantánea.

**Ejemplo real:** Almacenes grandes como IKEA dejan al cliente etiquetar sus propios artículos (favorito, regalo, pendiente). Igual que aquí, esas etiquetas viajan como texto en una sola columna y la aplicación las vuelve a convertir en lista al leerlas.

El resto del camino hasta la base de datos: `Product.Etiquetas` es la propiedad de la entidad, `ProductDto.Etiquetas` va como propiedad `init` para no tocar el constructor posicional que usan los tests y el sembrador, `ToEntity` se queda solo con las filas que traen texto y `UpdateAsync` sustituye la lista entera con una instancia nueva, que es como EF Core detecta el cambio. En el catálogo se pintan como chipes debajo de la descripción, y en la edición vuelven a aparecer en sus filas.

## El servicio de almacenamiento

Toda la decisión de guardar un fichero vive en `StorageService`, que implementa `IStorageService` y devuelve un `StorageResult` con `Ok`, `Mensaje` y `Ruta`. El controlador y el PageModel no abren ficheros: solo preguntan si se pudo guardar y qué ruta pública usar. Las comprobaciones, en orden:

- **Extensión**: lista blanca configurada en `appsettings.json` (`Storage:AllowedExtensions`: `.jpg`, `.jpeg`, `.png`, `.gif`).
- **Techo por fichero**: `Storage:MaxFileSize` (5 MB por defecto), independiente del techo por petición.
- **Firma**: los primeros bytes se comparan con la firma real del formato (`FF D8 FF` para JPEG, los 8 bytes del PNG, `GIF87a`/`GIF89a` para GIF). Renombrar un `.exe` a `.jpg` no basta.
- **Nombre**: lo decide el servidor, nunca el cliente. Combina fecha, un fragmento de GUID y el nombre saneado (`20261101101010_ab12cd34_foto.jpg`), de modo que dos subidas con el mismo nombre no se pisan y un nombre con `../` no escribe fuera del almacén.

El resultado se guarda en la propiedad `Imagen` del producto como ruta pública (`/uploads/productos/...`), que es a la vez la URL para servirla y el identificador para descargarla.

## Servir y descargar

`wwwroot/uploads` es una carpeta estática más: el middleware de ficheros estáticos sirve la imagen con solo conocer su URL. La descarga es otra cosa: el navegador debe abrir el diálogo de guardado en lugar de pintar la imagen, y eso lo consigue `PhysicalFile` con su tercer parámetro, que escribe la cabecera `Content-Disposition: attachment; filename=...`:

```csharp
return PhysicalFile(rutaFisica, "application/octet-stream", Path.GetFileName(ruta));
```

La ruta que llega por la query se resuelve contra `wwwroot` con `GetPhysicalPath`; si escapa de él (`../../appsettings.json`, una ruta absoluta de Windows), devuelve `null` y la acción responde `404`. Es la defensa contra el path traversal, y está cubierta por tests y por el humo. Como la comprobación se hace contra `wwwroot`, el enlace de descarga sirve igual para una imagen subida que para la de por defecto: la vista no tiene que distinguirlas.

## El techo del endpoint

El servicio valida el tamaño del fichero, pero la petición puede ser mucho más grande que el fichero. `[RequestSizeLimit(10 * 1024 * 1024)]` en cada POST que admite subidas corta la petición antes de que ASP.NET Core la lea entera: un envío de gigabytes no llega nunca al servicio.

## Cómo ejecutarlo

```bash
dotnet restore
dotnet build
dotnet run --project ProductosWeb.RazorPages   # http://localhost:5086
dotnet run --project ProductosWeb.Mvc          # http://localhost:5085
```

La base de datos es un `productos.db` local: en Development se borra y se siembra con cada arranque, así que siempre partes de los mismos 5 productos. Las subidas van a `wwwroot/uploads/productos/`, que el propio servicio crea si no existe; esa carpeta está en el `.gitignore` porque los ficheros subidos no se versionan.

## Tests

```bash
dotnet test
```

Sesenta y siete pruebas: veinte del servicio de almacenamiento (con un `wwwroot` temporal que se borra al terminar y `FormFile` reales, no simulados), ocho del validador (incluido el rango del año), diez del servicio con EF Core InMemory, veinte del controlador, cinco de los PageModels del alta y cuatro del mapeo de formulario a entidad, que es donde se blindan los textos vacíos que el binder entrega como `null`. Los tests de controlador y páginas usan el validador **real**, no un mock, y el almacenamiento simulado para comprobar cómo reacciona cada vista al resultado de guardar.

## Verificación de las dos visiones

Las dos visiones se comprueban con la misma lista de aserciones: el formulario es multipart con los seis tipos de campo, el token anti-forgery corta los POST sin token con `400`, la subida válida redirige con `302` y el fichero se sirve desde la carpeta estática, la descarga llega con su `Content-Disposition`, los rechazos de extensión, firma y tamaño devuelven la vista con el motivo, el path traversal responde `404`, el alta completa pinta el badge de novedad y el año fuera de rango se revalida sin tocar la base de datos. El estado tras la última revisión: build **0/0**, **67/67** tests, **82/82** comprobaciones de humo (41 por visión) y **0** caracteres no latinos en 67 ficheros.

El CSS y el partial del formulario son idénticos byte a byte en los dos proyectos. El partial pinta las etiquetas con el prefijo que le da el servidor (`Etiquetas[0]` en MVC, `Formulario.Etiquetas[0]` en Razor Pages), así que el mismo JavaScript sirve para las dos visiones.

## Errores frecuentes al tocarlo

| Error | Causa |
|-------|-------|
| El `input type=file` llega vacío al servidor | Falta `enctype="multipart/form-data"` en el formulario |
| Un campo de texto vacío provoca un `NullReferenceException` en el alta | El binder entrega `null` en los textos vacíos; `ToEntity` los recibe con `?? string.Empty` |
| Se guarda un fichero aunque el formulario tenga errores | El orden de la acción: primero el `ModelState`, después convertir y tocar el disco |
| El POST de la subida devuelve `400` | El token anti-forgery viaja como otro campo del multipart; sin él el servidor lo rechaza |
| Las etiquetas se escriben y al recargar desaparecen | La entidad no tiene el campo, el mapper no lo copia o falta la conversión de valor en el DbContext |
| EF Core avisa del `10620` al arrancar | Una colección con conversión necesita su `ValueComparer`; sin él no se comparan las listas |
| El fichero se guarda pero la imagen no se ve | La ruta pública debe empezar por `/uploads/` para que el middleware de ficheros estáticos la sirva |
| `GetPhysicalPath` devuelve una ruta inexistente | La ruta pública se resuelve contra `wwwroot`, no contra `uploads`; ahí está la red de seguridad contra el traversal |
| `413` en un envío grande | El techo es `[RequestSizeLimit]`, no el `MaxFileSize` del servicio: los dos hacen falta |
| La subida de un `.exe` renombrado pasa | La extensión no basta: la firma de los primeros bytes es la que decide |
| `uploads/` aparece en el `git` | La carpeta está gitignorada salvo su `.gitkeep`; si aparece, se versionó algo que no debe |
| 404 en la raíz de Razor Pages | No hay página en `/`; el índice está en `/Productos` |

## Qué mirar después

- **05-ProductosEstado** — la misma capa de negocio con estado del cliente: cookies, sesión y TempData bajo la lupa.
- **12-mvc-vs-razorpages** — el tema que compara ambas arquitecturas para decidir cuándo usar cada una.
