using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using ProductosWeb.Mvc.Controllers;
using ProductosWeb.Shared.Dtos;
using ProductosWeb.Shared.Models;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.Services;
using ProductosWeb.Shared.Services.Storage;
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Controllers;

/// <summary>
/// Tests del controlador de formularios.
/// Se usa el validador REAL (no un mock): así se comprueba de verdad que las dos capas
/// de validación llegan al mismo ModelState antes de decidir entre vista y redirección.
/// El almacenamiento sí se simula: lo que se prueba aquí es cómo reacciona el controlador
/// al resultado de guardar un fichero, no cómo lo guarda.
/// </summary>
[TestFixture]
public class ProductosControllerTests
{
    private Mock<IProductService> _servicio = null!;
    private Mock<IStorageService> _almacen = null!;
    private ProductosController _controlador = null!;

    [SetUp]
    public void SetUp()
    {
        _servicio = new Mock<IProductService>();
        _almacen = new Mock<IStorageService>();

        _controlador = new ProductosController(
            _servicio.Object, _almacen.Object, new ProductFormValidator())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        // TempData necesita un proveedor; con uno simulado basta para leer y escribir mensajes.
        _controlador.TempData = new TempDataDictionary(
            new DefaultHttpContext(), new Mock<ITempDataProvider>().Object);
    }

    [TearDown]
    public void TearDown() => _controlador?.Dispose();

    /// <summary>
    /// Fórmula un formulario válido para partir de él en los tests de alta.
    /// </summary>
    /// <returns>Formulario que pasa todas las reglas.</returns>
    private static ProductFormViewModel FormularioValido() => new()
    {
        Nombre = "Smartphone Aurora X5",
        Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
        Precio = 449.99m,
        Stock = 10,
        Anio = 2026,
        Categoria = ProductCategory.Smartphones
    };

    [Test]
    public async Task Crear_FormularioValido_RedirigeAlListadoConTempData()
    {
        // Arrange
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Smartphone Aurora X5", "desc", 449.99m, 10, 2026,
                ProductCategory.Smartphones, true, "/images/producto-sin-imagen.svg", true));

        // Act
        var resultado = await _controlador.Crear(FormularioValido());

        // Assert: el patrón Post-Redirect-Get devuelve un 302, no la vista.
        var redirect = resultado.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(ProductosController.Index));
        _controlador.TempData["Exito"].Should().BeOfType<string>()
            .Which.Should().Contain("creado correctamente");
    }

    [Test]
    public async Task Crear_FormularioInvalido_DevuelveLaVistaConLosErrores()
    {
        // Arrange: el nombre es demasiado corto y la descripción también.
        var formulario = FormularioValido();
        formulario.Nombre = "X";
        formulario.Descripcion = "corta";

        // Act
        var resultado = await _controlador.Crear(formulario);

        // Assert: sin redirección, para que el usuario pueda corregir.
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.Model.Should().BeSameAs(formulario);
        _controlador.ModelState.IsValid.Should().BeFalse();
        _controlador.ModelState.ErrorCount.Should().BeGreaterThan(0);
        _controlador.ModelState["Nombre"]!.Errors.Should().NotBeEmpty();
        _controlador.ModelState["Descripcion"]!.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task Crear_CampoDeTextoVacio_DevuelveLaVistaSinExcepcion()
    {
        // Arrange: el binder entrega null en los campos de texto vacíos (esa es su regla
        // ConvertEmptyStringToNull). El controlador tiene que devolver la vista con los
        // errores, nunca lanzar NullReferenceException por el mapeo.
        var formulario = FormularioValido();
        formulario.Nombre = null!;
        formulario.Descripcion = null!;

        // Act
        var resultado = await _controlador.Crear(formulario);

        // Assert: vuelve al formulario con errores y no llega a tocar el servicio.
        resultado.Should().BeOfType<ViewResult>();
        _controlador.ModelState.IsValid.Should().BeFalse();
        _servicio.Verify(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Crear_NombreRepetido_DevuelveLaVistaConElAviso()
    {
        // Arrange: el servicio devuelve null porque el índice único ha detectado la repetición.
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var resultado = await _controlador.Crear(FormularioValido());

        // Assert
        resultado.Should().BeOfType<ViewResult>();
        _controlador.ModelState["Nombre"]!.Errors
            .Should().Contain(e => e.ErrorMessage.Contains("Ya existe un producto con ese nombre"));
    }

    [Test]
    public async Task Crear_PasaElFormularioAlServicioConElNombreLimpio()
    {
        // Arrange
        Product? capturado = null;
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => capturado = p)
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, 2026, ProductCategory.Audio, false, "i", true));

        var formulario = FormularioValido();
        formulario.Nombre = "  Smartphone Aurora X5  ";

        // Act
        await _controlador.Crear(formulario);

        // Assert
        capturado.Should().NotBeNull();
        capturado!.Nombre.Should().Be("Smartphone Aurora X5");
        capturado.Precio.Should().Be(449.99m);
        capturado.Anio.Should().Be(2026);
    }

    [Test]
    public async Task Crear_SinFichero_NoTocaElAlmacenamiento()
    {
        // Arrange: el formulario no trae imagen; el servicio no debe ni llamar al almacén.
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, 2026, ProductCategory.Audio, false, "i", true));

        // Act
        await _controlador.Crear(FormularioValido());

        // Assert
        _almacen.Verify(a => a.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Crear_FicheroRechazado_DevuelveLaVistaConElMotivo()
    {
        // Arrange: el servicio de almacenamiento rechaza el fichero por extensión.
        var formulario = FormularioValido();
        formulario.ImagenFile = new Mock<IFormFile>().Object;

        _almacen.Setup(a => a.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(StorageResult.Fallo("La extensión .exe no está permitida"));

        // Act
        var resultado = await _controlador.Crear(formulario);

        // Assert: se vuelve a la vista y el motivo queda en la clave del campo.
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.Model.Should().BeSameAs(formulario);
        _controlador.ModelState["ImagenFile"]!.Errors
            .Should().Contain(e => e.ErrorMessage.Contains(".exe"));
        _servicio.Verify(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Crear_FicheroAceptado_GuardaLaRutaEnElProducto()
    {
        // Arrange
        Product? capturado = null;
        var formulario = FormularioValido();
        formulario.ImagenFile = new Mock<IFormFile>().Object;

        _almacen.Setup(a => a.SaveFileAsync(It.IsAny<IFormFile>(), "productos"))
            .ReturnsAsync(StorageResult.Exito("/uploads/productos/20261101101010_ab12cd34_foto.jpg"));

        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => capturado = p)
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, 2026, ProductCategory.Audio, false, "i", true));

        // Act
        var resultado = await _controlador.Crear(formulario);

        // Assert: la ruta que devolvió el almacén es la que se persiste.
        resultado.Should().BeOfType<RedirectToActionResult>();
        capturado!.Imagen.Should().Be("/uploads/productos/20261101101010_ab12cd34_foto.jpg");
    }

    [Test]
    public async Task Editar_IdExistente_DevuelveElFormularioPrecargado()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Auriculares Aurora", "Cancelación de ruido.", 89.90m, 24, 2024,
                ProductCategory.Audio, false, "/images/producto-sin-imagen.svg", true));

        // Act
        var resultado = await _controlador.Editar(1);

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<ProductFormViewModel>().Subject;
        modelo.Id.Should().Be(1);
        modelo.Nombre.Should().Be("Auriculares Aurora");
        modelo.Precio.Should().Be(89.90m);
        modelo.Anio.Should().Be(2024);
    }

    [Test]
    public async Task Editar_IdInexistente_RedirigeConError()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(9999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var resultado = await _controlador.Editar(9999);

        // Assert
        resultado.Should().BeOfType<RedirectToActionResult>();
        _controlador.TempData["Error"].Should().BeOfType<string>()
            .Which.Should().Contain("no existe");
    }

    [Test]
    public async Task Editar_FormularioValido_RedirigeConTempDataDeExito()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, 2026, ProductCategory.Audio, false, "i", true));
        _servicio.Setup(s => s.UpdateAsync(1, It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Editado", "desc", 10m, 1, 2026,
                ProductCategory.Audio, false, "i", true));

        var formulario = FormularioValido();
        formulario.Id = 1;
        formulario.Nombre = "Editado";

        // Act
        var resultado = await _controlador.Editar(1, formulario);

        // Assert
        resultado.Should().BeOfType<RedirectToActionResult>();
        _controlador.TempData["Exito"].Should().BeOfType<string>()
            .Which.Should().Contain("actualizado correctamente");
    }

    [Test]
    public async Task Editar_ProductoYaBorrado_RedirigeConError()
    {
        // Arrange: el producto existía al cargar la vista, pero alguien lo borró antes del POST.
        _servicio.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        var formulario = FormularioValido();
        formulario.Id = 1;

        // Act
        var resultado = await _controlador.Editar(1, formulario);

        // Assert
        resultado.Should().BeOfType<RedirectToActionResult>();
        _controlador.TempData["Error"].Should().BeOfType<string>()
            .Which.Should().Contain("ya no existe");
    }

    [Test]
    public async Task Eliminar_ProductoExistente_RedirigeConTempDataDeExito()
    {
        // Arrange
        _servicio.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var resultado = await _controlador.Eliminar(1);

        // Assert
        resultado.Should().BeOfType<RedirectToActionResult>();
        _controlador.TempData["Exito"].Should().BeOfType<string>()
            .Which.Should().Contain("eliminado correctamente");
    }

    [Test]
    public async Task Eliminar_ProductoInexistente_RedirigeConError()
    {
        // Arrange
        _servicio.Setup(s => s.DeleteAsync(9999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var resultado = await _controlador.Eliminar(9999);

        // Assert
        resultado.Should().BeOfType<RedirectToActionResult>();
        _controlador.TempData["Error"].Should().BeOfType<string>()
            .Which.Should().Contain("ya no existe");
    }

    [Test]
    public async Task Index_DevuelveLaListaDeProductos()
    {
        // Arrange
        _servicio.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>
            {
                new(1, "Auriculares Aurora", "Cancelación de ruido.", 89.90m, 24, 2024,
                    ProductCategory.Audio, false, "/images/producto-sin-imagen.svg", true)
            });

        // Act
        var resultado = await _controlador.Index();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.Model.Should().BeAssignableTo<IReadOnlyList<ProductDto>>().Which.Should().ContainSingle();
    }

    // --- Subida y descarga de ficheros ------------------------------------------

    [Test]
    public async Task Subir_FicheroValido_RedirigeConLaRutaEnTempData()
    {
        // Arrange
        _almacen.Setup(a => a.SaveFileAsync(It.IsAny<IFormFile>(), "productos"))
            .ReturnsAsync(StorageResult.Exito("/uploads/productos/20261101101010_ab12cd34_foto.jpg"));

        // Act
        var resultado = await _controlador.Subir(new Mock<IFormFile>().Object);

        // Assert
        var redirect = resultado.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(ProductosController.Index));
        _controlador.TempData["Exito"].Should().BeOfType<string>()
            .Which.Should().Contain("/uploads/productos/");
    }

    [Test]
    public async Task Subir_FicheroRechazado_DevuelveLaVistaConElMotivo()
    {
        // Arrange
        _almacen.Setup(a => a.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(StorageResult.Fallo("El fichero es demasiado grande: el máximo es 5 MB"));

        // Act
        var resultado = await _controlador.Subir(new Mock<IFormFile>().Object);

        // Assert: el motivo va al resumen, que es la clave vacía.
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.ViewName.Should().BeNull();
        _controlador.ModelState[string.Empty]!.Errors
            .Should().Contain(e => e.ErrorMessage.Contains("demasiado grande"));
    }

    [Test]
    public void Descargar_FicheroExistente_DevuelveElArchivoConNombreDeDescarga()
    {
        // Arrange
        const string ruta = "/uploads/productos/20261101101010_ab12cd34_foto.jpg";
        _almacen.Setup(a => a.FileExists(ruta)).Returns(true);
        _almacen.Setup(a => a.GetPhysicalPath(ruta)).Returns(@"C:\app\wwwroot\uploads\productos\20261101101010_ab12cd34_foto.jpg");

        // Act
        var resultado = _controlador.Descargar(ruta);

        // Assert: PhysicalFileResult escribe el Content-Disposition con el nombre del fichero.
        var archivo = resultado.Should().BeOfType<PhysicalFileResult>().Subject;
        archivo.FileDownloadName.Should().Be("20261101101010_ab12cd34_foto.jpg");
        archivo.ContentType.Should().Be("application/octet-stream");
    }

    [Test]
    public void Descargar_FicheroInexistente_DevuelveNotFound()
    {
        // Arrange: la ruta está dentro del almacén pero no hay nada escrito.
        _almacen.Setup(a => a.FileExists(It.IsAny<string>())).Returns(false);

        // Act
        var resultado = _controlador.Descargar("/uploads/productos/no-existe.jpg");

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public void Descargar_RutaQueEscapa_DevuelveNotFound()
    {
        // Arrange: una ruta con ../ contra el almacén no resuelve a ningún fichero.
        _almacen.Setup(a => a.FileExists(It.IsAny<string>())).Returns(false);

        // Act
        var resultado = _controlador.Descargar("/uploads/../../appsettings.json");

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }
}
