using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.Services.Storage;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Mvc.Controllers;

/// <summary>
/// Controlador de formularios de producto.
/// Cada POST cierra con el patrón Post-Redirect-Get: si todo va bien se redirige con un
/// código 302 y el mensaje viaja en TempData; si hay errores de validación se devuelve la
/// misma vista con el ModelState marcado, sin redirigir, para que el usuario corrija.
/// El formulario es multipart, así que los POST de alta y edición también pueden traer el
/// fichero de la imagen: el techo del endpoint corta la petición y el servicio de
/// almacenamiento decide si el fichero entra o no.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="almacen">Almacenamiento de los ficheros subidos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
public class ProductosController(
    IProductService servicio,
    IStorageService almacen,
    IValidator<ProductFormViewModel> validador) : Controller
{
    /// <summary>
    /// Listado completo del catálogo.
    /// </summary>
    /// <returns>Vista con la lista de productos en DTO.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productos = await servicio.GetAllAsync();
        return View(productos);
    }

    /// <summary>
    /// Muestra el formulario de alta vacío.
    /// </summary>
    /// <returns>Vista del formulario.</returns>
    [HttpGet]
    public IActionResult Crear() => View(new ProductFormViewModel());

    /// <summary>
    /// Recibe el alta. El fichero de imagen es opcional: si viene, el servicio de
    /// almacenamiento lo valida y devuelve la ruta que se guarda en el producto.
    /// </summary>
    /// <param name="model">Datos del formulario, incluido el fichero subido.</param>
    /// <returns>Redirección al listado si todo va bien; la vista con errores si no.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Crear([Bind("Nombre,Descripcion,Precio,Stock,Anio,Categoria,EsNovedad,Imagen,ImagenFile,Etiquetas")] ProductFormViewModel model)
    {
        var resultadoValidacion = await validador.ValidateAsync(model);
        if (!resultadoValidacion.IsValid)
        {
            resultadoValidacion.AddToModelState(ModelState);
        }

        // Se comprueba el ModelState ANTES de tocar la entidad o el disco: un formulario
        // inválido no se convierte ni guarda ficheros, que así no quedan huérfanos en
        // wwwroot/uploads cuando luego se vuelve a pintar la vista con errores.
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entidad = model.ToEntity();
        if (!await AplicaImagenAsync(entidad, model))
        {
            return View(model);
        }

        var creado = await servicio.CreateAsync(entidad);

        if (creado is null)
        {
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe un producto con ese nombre");
            return View(model);
        }

        TempData["Exito"] = $"Producto \"{creado.Nombre}\" creado correctamente";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Muestra el formulario de edición precargado con los datos del producto.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Vista del formulario o NotFound si el producto no existe.</returns>
    [HttpGet]
    public async Task<IActionResult> Editar(long id)
    {
        var producto = await servicio.GetByIdAsync(id);
        if (producto is null)
        {
            TempData["Error"] = "El producto solicitado no existe";
            return RedirectToAction(nameof(Index));
        }

        return View(new ProductFormViewModel().ToFormViewModel(producto));
    }

    /// <summary>
    /// Recibe la edición. Como el alta, admite un fichero nuevo de imagen que sustituye al anterior.
    /// </summary>
    /// <param name="id">Identificador del producto (de la URL, no del formulario).</param>
    /// <param name="model">Datos del formulario, incluido el fichero subido.</param>
    /// <returns>Redirección al listado si todo va bien; la vista con errores si no.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Editar(long id, [Bind("Id,Nombre,Descripcion,Precio,Stock,Anio,Categoria,EsNovedad,Imagen,ImagenFile,Etiquetas")] ProductFormViewModel model)
    {
        var productoExistente = await servicio.GetByIdAsync(id);
        if (productoExistente is null)
        {
            TempData["Error"] = "El producto que intentabas editar ya no existe";
            return RedirectToAction(nameof(Index));
        }

        var resultadoValidacion = await validador.ValidateAsync(model);
        if (!resultadoValidacion.IsValid)
        {
            resultadoValidacion.AddToModelState(ModelState);
        }

        // Igual que en el alta: primero se decide si el formulario vale y sólo después se
        // convierte y se toca el disco.
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entidad = model.ToEntity();
        if (!await AplicaImagenAsync(entidad, model))
        {
            return View(model);
        }

        var actualizado = await servicio.UpdateAsync(id, entidad);

        if (actualizado is null)
        {
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe otro producto con ese nombre");
            return View(model);
        }

        TempData["Exito"] = $"Producto \"{actualizado.Nombre}\" actualizado correctamente";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Elimina un producto. Es un POST porque cambia estado, y lleva token anti-forgery
    /// porque un enlace malicioso en otra página podría intentar dispararlo.
    /// La confirmación la pinta la vista con <c>confirm()</c> antes de enviar el formulario.
    /// </summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Redirect al listado con el mensaje correspondiente.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(long id)
    {
        var eliminado = await servicio.DeleteAsync(id);

        TempData[eliminado ? "Exito" : "Error"] = eliminado
            ? "Producto eliminado correctamente"
            : "El producto que intentabas eliminar ya no existe";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Formulario de subida independiente del producto, pensado para probar los rechazos
    /// del servicio (extensión, tamaño, firma) sin pasar por el alta completa.
    /// </summary>
    /// <returns>Vista con el formulario de subida.</returns>
    [HttpGet]
    public IActionResult Subir() => View();

    /// <summary>
    /// Recibe el fichero y lo delega en el almacenamiento.
    /// Si el servicio lo rechaza, se devuelve la vista con el motivo en el resumen;
    /// si lo guarda, se redirige con la ruta pública en TempData.
    /// </summary>
    /// <param name="fichero">Fichero elegido en el formulario.</param>
    /// <returns>Redirección al listado si se guardó; la vista con el error si no.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Subir(IFormFile? fichero)
    {
        var resultado = await almacen.SaveFileAsync(fichero, "productos");

        if (!resultado.Ok)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View();
        }

        TempData["Exito"] = $"Fichero guardado en {resultado.Ruta}";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Descarga un fichero de wwwroot.
    /// El nombre que recibe el navegador lo decide el servidor con <c>fileDownloadName</c>,
    /// que escribe el Content-Disposition; la ruta que llega por la query se resuelve dentro
    /// de wwwroot y, si escapa de él, no devuelve nada.
    /// </summary>
    /// <param name="ruta">Ruta pública del fichero, por ejemplo /uploads/productos/foto.jpg.</param>
    /// <returns>El fichero como descarga o NotFound si no está en wwwroot.</returns>
    [HttpGet]
    public IActionResult Descargar([FromQuery] string ruta)
    {
        if (!almacen.FileExists(ruta))
        {
            return NotFound();
        }

        var rutaFisica = almacen.GetPhysicalPath(ruta)!;
        return PhysicalFile(rutaFisica, "application/octet-stream", Path.GetFileName(ruta));
    }

    /// <summary>
    /// Guarda el fichero que haya traído el formulario y escribe su ruta en la entidad.
    /// Sin fichero no se toca la imagen que ya tenía el producto.
    /// </summary>
    /// <param name="entidad">Entidad que se va a persistir.</param>
    /// <param name="model">Formulario, con el fichero opcional.</param>
    /// <returns>true si se puede seguir; false si el servicio de almacenamiento rechazó el fichero.</returns>
    private async Task<bool> AplicaImagenAsync(Product entidad, ProductFormViewModel model)
    {
        if (model.ImagenFile is null)
        {
            return true;
        }

        var resultado = await almacen.SaveFileAsync(model.ImagenFile, "productos");
        if (!resultado.Ok)
        {
            ModelState.AddModelError(nameof(model.ImagenFile), resultado.Mensaje);
            return false;
        }

        entidad.Imagen = resultado.Ruta;
        return true;
    }
}
