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
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Controllers;

/// <summary>
/// Tests del controlador de formularios.
/// Se usa el validador REAL (no un mock): así se comprueba de verdad que las dos capas
/// de validación llegan al mismo ModelState antes de decidir entre vista y redirección.
/// </summary>
[TestFixture]
public class ProductosControllerTests
{
    private Mock<IProductService> _servicio = null!;
    private ProductosController _controlador = null!;

    [SetUp]
    public void SetUp()
    {
        _servicio = new Mock<IProductService>();

        _controlador = new ProductosController(
            _servicio.Object, new ProductFormValidator(), new BindingFormValidator())
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
        Categoria = ProductCategory.Smartphones
    };

    [Test]
    public async Task Crear_FormularioValido_RedirigeAlListadoConTempData()
    {
        // Arrange
        _servicio.Setup(s => s.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Smartphone Aurora X5", "desc", 449.99m, 10,
                ProductCategory.Smartphones, "/images/producto-sin-imagen.svg", true));

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
            .ReturnsAsync(new ProductDto(1, "x", "y", 1, 1, ProductCategory.Audio, "i", true));

        var formulario = FormularioValido();
        formulario.Nombre = "  Smartphone Aurora X5  ";

        // Act
        await _controlador.Crear(formulario);

        // Assert
        capturado.Should().NotBeNull();
        capturado!.Nombre.Should().Be("Smartphone Aurora X5");
        capturado.Precio.Should().Be(449.99m);
    }

    [Test]
    public async Task Editar_IdExistente_DevuelveElFormularioPrecargado()
    {
        // Arrange
        _servicio.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Auriculares Aurora", "Cancelación de ruido.", 89.90m, 24,
                ProductCategory.Audio, "/images/producto-sin-imagen.svg", true));

        // Act
        var resultado = await _controlador.Editar(1);

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<ProductFormViewModel>().Subject;
        modelo.Id.Should().Be(1);
        modelo.Nombre.Should().Be("Auriculares Aurora");
        modelo.Precio.Should().Be(89.90m);
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
        _servicio.Setup(s => s.UpdateAsync(1, It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(1, "Editado", "desc", 10m, 1,
                ProductCategory.Audio, "i", true));

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
    public async Task Editar_IdDeLaRutaDistintoDelFormulario_NoActualizaYDevuelveError()
    {
        // Arrange: el formulario dice 1 (así lo pintó el servidor), pero en la ruta llega 99.
        // Es lo que queda cuando alguien abre F12 y se cambia el campo oculto para que el POST
        // edite otro registro.
        var formulario = FormularioValido();
        formulario.Id = 1;

        // Act
        var resultado = await _controlador.Editar(99, formulario);

        // Assert: se devuelve la vista con el error y el servicio no se llega a tocar.
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.Model.Should().BeSameAs(formulario);
        var errorGlobal = _controlador.ModelState[string.Empty];
        errorGlobal.Should().NotBeNull();
        errorGlobal!.Errors.Should().Contain(e => e.ErrorMessage.Contains("no coincide"));
        _servicio.Verify(
            s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Editar_ProductoYaBorrado_RedirigeConError()
    {
        // Arrange: el producto existía al cargar la vista, pero alguien lo borró antes del POST.
        _servicio.Setup(s => s.UpdateAsync(1, It.IsAny<Product>(), It.IsAny<CancellationToken>()))
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
                new(1, "Auriculares Aurora", "Cancelación de ruido.", 89.90m, 24,
                    ProductCategory.Audio, "/images/producto-sin-imagen.svg", true)
            });

        // Act
        var resultado = await _controlador.Index();

        // Assert
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        vista.Model.Should().BeAssignableTo<IReadOnlyList<ProductDto>>().Which.Should().ContainSingle();
    }

    // --- Página de Binding avanzado ------------------------------------------------

    [Test]
    public void Binding_Get_DevuelveElModeloDeEjemplo()
    {
        // Act
        var resultado = _controlador.Binding();

        // Assert: la cabecera, una etiqueta y un recargo, con el índice 0 ya visible.
        var vista = resultado.Should().BeOfType<ViewResult>().Subject;
        var modelo = vista.Model.Should().BeOfType<BindingFormViewModel>().Subject;
        modelo.Cabecera.Nombre.Should().Be("Smartphone Aurora X5");
        modelo.Etiquetas.Should().ContainSingle().Which.Should().Be("oferta");
        modelo.Recargos.Should().ContainSingle();
    }

    [Test]
    public async Task Binding_PostProbar_ColeccionesRecogidas_EscribeElResumen()
    {
        // Arrange: dos etiquetas y dos recargos, tal y como los mandaría el formulario.
        var modelo = new BindingFormViewModel
        {
            Cabecera = new ProductFormViewModel
            {
                Nombre = "Smartphone Aurora X5",
                Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
                Precio = 12.75m,
                Stock = 3,
                Categoria = ProductCategory.Smartphones
            },
            Etiquetas = ["oferta", "nuevo"],
            Recargos =
            [
                new RecargoViewModel { Concepto = "Envío urgente", Importe = 4.95m },
                new RecargoViewModel { Concepto = "Grabado", Importe = 12m }
            ]
        };

        // Act
        var resultado = await _controlador.Binding(modelo, "probar");

        // Assert: el resumen nombra cada campo con su prefijo y su índice.
        resultado.Should().BeOfType<ViewResult>();
        var resumen = _controlador.ViewData["Resumen"].Should().BeOfType<string>().Subject;
        resumen.Should().Contain("Cabecera.Nombre = Smartphone Aurora X5");
        resumen.Should().Contain("Etiquetas[1] = nuevo");
        resumen.Should().Contain("Recargos[1].Concepto = Grabado");
    }

    [Test]
    public async Task Binding_PostProbar_CabeceraInvalida_DevuelveLaVistaConLosErrores()
    {
        // Arrange: la cabecera no pasa las reglas del producto.
        var modelo = new BindingFormViewModel { Cabecera = new ProductFormViewModel { Nombre = "X" } };
        modelo.Cabecera.Descripcion = "corta";

        // Act
        var resultado = await _controlador.Binding(modelo, "probar");

        // Assert: sin resumen y con el error en la clave con prefijo.
        resultado.Should().BeOfType<ViewResult>();
        _controlador.ModelState.IsValid.Should().BeFalse();
        _controlador.ModelState["Cabecera.Nombre"]!.Errors.Should().NotBeEmpty();
        _controlador.ViewData["Resumen"].Should().BeNull();
    }

    [Test]
    public async Task Binding_PostAnadirEtiqueta_AnadeUnaFilaSinValidar()
    {
        // Arrange: una fila de más que además viene inválida; añadir no debe validar.
        var modelo = new BindingFormViewModel();
        modelo.Etiquetas.Add("   ");

        // Act
        var resultado = await _controlador.Binding(modelo, "anadir-etiqueta");

        // Assert
        resultado.Should().BeOfType<ViewResult>();
        modelo.Etiquetas.Should().HaveCount(2);
        _controlador.ModelState.IsValid.Should().BeTrue();
        _controlador.ViewData["Resumen"].Should().BeNull();
    }
}
