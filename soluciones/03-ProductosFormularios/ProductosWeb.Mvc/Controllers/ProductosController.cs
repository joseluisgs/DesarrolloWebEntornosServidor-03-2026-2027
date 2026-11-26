using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Extensions;
using ProductosWeb.Shared.Mappers;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Mvc.Controllers;

/// <summary>
/// Controlador de formularios de producto.
/// Cada POST cierra con el patrón Post-Redirect-Get: si todo va bien se redirige con un
/// código 302 y el mensaje viaja en TempData; si hay errores de validación se devuelve la
/// misma vista con el ModelState marcado, sin redirigir, para que el usuario corrija.
/// </summary>
/// <param name="servicio">Catálogo de productos.</param>
/// <param name="validador">Validador FluentValidation del formulario.</param>
/// <param name="validadorBinding">Validador del formulario de Binding avanzado.</param>
public class ProductosController(
    IProductService servicio,
    IValidator<ProductFormViewModel> validador,
    IValidator<BindingFormViewModel> validadorBinding)
    : Controller
{
    /// <summary>Lista completa del catálogo, con el mensaje de TempData si lo hay.</summary>
    /// <returns>Vista con la tabla de productos.</returns>
    public async Task<IActionResult> Index()
    {
        var productos = await servicio.GetAllAsync();
        return View(productos);
    }

    /// <summary>
    /// Formulario de alta en blanco.
    /// El GET no recibe datos, así que solo hay que devolver la vista con el modelo vacío.
    /// </summary>
    /// <returns>Vista del formulario vacío.</returns>
    public IActionResult Crear() => View(new ProductFormViewModel());

    /// <summary>
    /// Recibe el alta. ValidateAntiForgeryToken comprueba el token que el form Tag Helper emitió.
    /// </summary>
    /// <param name="model">Modelo que el binder ha rellenado desde el formulario.</param>
    /// <returns>Redirect con TempData si va bien; la misma vista con los errores si no.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ProductFormViewModel model)
    {
        // Dos capas de validación: DataAnnotations ya se ejecutaron solas al enlazar el modelo,
        // y aquí sumamos las reglas FluentValidation al mismo ModelState.
        var resultado = await validador.ValidateAsync(model);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var creado = await servicio.CreateAsync(ToEntity(model));

        if (creado is null)
        {
            // El nombre ya existía: el índice único de la base de datos ha decidido.
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe un producto con ese nombre");
            return View(model);
        }

        TempData["Exito"] = $"Producto \"{creado.Nombre}\" creado correctamente";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Formulario de edición precargado. Si el Id no existe, se redirige al listado con aviso.
    /// </summary>
    /// <param name="id">Identificador del producto a editar.</param>
    /// <returns>Vista con el formulario relleno, o redirect si no existe.</returns>
    public async Task<IActionResult> Editar(long id)
    {
        var producto = await servicio.GetByIdAsync(id);

        if (producto is null)
        {
            TempData["Error"] = "El producto solicitado no existe";
            return RedirectToAction(nameof(Index));
        }

        return View(ToForm(producto));
    }

    /// <summary>
    /// Recibe la edición. El identificador llega dos veces: en la ruta, que es la que enlazó el
    /// catálogo y que por eso es fiable, y en el campo oculto del formulario, que lo pinta el
    /// servidor pero que el cliente puede cambiar. Los dos tienen que decir lo mismo.
    /// </summary>
    /// <param name="id">Identificador de la ruta, el que enlazó el catálogo.</param>
    /// <param name="model">Modelo rellenado por el binder, con el Id del formulario.</param>
    /// <returns>Redirect con TempData si va bien; la misma vista con los errores si no.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(long id, ProductFormViewModel model)
    {
        var resultado = await validador.ValidateAsync(model);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Alternativa A, la que usamos: el Id de la ruta y el del formulario tienen que ser el
        // mismo. Si alguien abre F12 y cambia el campo oculto para que el POST edite otro
        // registro, aquí se queda: la petición solo puede tocar lo que enlazó el catálogo.
        if (id != model.Id)
        {
            ModelState.AddModelError(string.Empty, "El Id del formulario no coincide con el de la ruta");
            return View(model);
        }

        // Alternativa B, sin campo oculto: quita el <input type="hidden"> de la vista y pon aquí
        // model.Id = id; el identificador solo lo manda la ruta y el formulario llega con 0.
        //
        // Alternativa C, ni siquiera la ruta manda: carga el producto con el id de la ruta y
        // aplica solo los campos que el usuario tiene permiso de cambiar, de modo que el Id del
        // formulario ni se mira. Es la más dura y la que mejor aguanta el paso del tiempo:
        //     var original = await servicio.GetByIdAsync(id);
        //     if (original is null) { TempData["Error"] = "El producto solicitado no existe"; ... }
        //     var aGuardar = ToEntity(model);
        //     aGuardar.Id = original.Id;   // el registro lo decide el servidor
        //
        // Ninguna de las tres sustituye a la comprobación de existencia (más abajo) ni a la de
        // permisos cuando la aplicación tenga usuarios: el identificador no es un secreto.

        var actualizado = await servicio.UpdateAsync(model.Id, ToEntity(model));

        if (actualizado is null)
        {
            TempData["Error"] = "El producto que intentabas editar ya no existe";
            return RedirectToAction(nameof(Index));
        }

        TempData["Exito"] = $"Producto \"{actualizado.Nombre}\" actualizado correctamente";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Elimina un producto. Es un POST porque cambia estado, y lleva token anti-forgery
    /// porque un enlace malicioso en otra página podría intentar dispararlo.
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
    /// Página de Binding avanzado: cabecera con prefijo y colecciones por índice.
    /// El GET muestra el modelo de partida; el POST recoge lo que el formulario haya mandado
    /// y lo describe línea a línea, que es la manera más directa de ver qué hizo el binder.
    /// </summary>
    /// <returns>La página con el modelo de partida.</returns>
    [HttpGet]
    public IActionResult Binding() => View(BindingMapper.CrearDeEjemplo());

    /// <summary>
    /// Recibe la página de Binding. Añadir una fila no valida: es repintado, no negocio.
    /// El botón de probar sí valida las dos capas y, si todo cuadra, pinta el resumen
    /// de lo que el binder ha recogido.
    /// </summary>
    /// <param name="model">Formulario enlazado desde los campos con prefijo e índices.</param>
    /// <param name="accion">Botón pulsado.</param>
    /// <returns>La propia página con el repintado o con el resumen.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Binding(BindingFormViewModel model, string? accion)
    {
        if (accion == "anadir-etiqueta")
        {
            model.Etiquetas.Add(string.Empty);
            return View(model);
        }

        if (accion == "anadir-recargo")
        {
            model.Recargos.Add(new RecargoViewModel());
            return View(model);
        }

        var resultado = await validadorBinding.ValidateAsync(model);
        resultado.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ViewData["Resumen"] = model.DescripcionBinding();
        return View(model);
    }

    /// <summary>
    /// Pasa del DTO del servicio al formulario. El mapeo vive en el proyecto compartido
    /// para que las dos visiones del ejemplo lo hagan exactamente igual.
    /// </summary>
    /// <param name="producto">DTO devuelto por el servicio.</param>
    /// <returns>Formulario listo para la vista de edición.</returns>
    private static ProductFormViewModel ToForm(ProductDto producto) =>
        new ProductFormViewModel().ToFormViewModel(producto);

    /// <summary>
    /// Pasa del formulario al modelo que persiste el servicio. El Id solo se usa al editar.
    /// </summary>
    /// <param name="model">Formulario validado.</param>
    /// <returns>Entidad lista para la base de datos.</returns>
    private static ProductosWeb.Shared.Models.Product ToEntity(ProductFormViewModel model) => new()
    {
        Id = model.Id,
        Nombre = model.Nombre.Trim(),
        Descripcion = model.Descripcion.Trim(),
        Precio = model.Precio,
        Stock = model.Stock,
        Categoria = model.Categoria,
        Imagen = string.IsNullOrWhiteSpace(model.Imagen) ? null : model.Imagen.Trim()
    };
}
