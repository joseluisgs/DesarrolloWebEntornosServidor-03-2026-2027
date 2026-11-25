using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ProductosWeb.Shared.Services.Storage;

namespace ProductosWeb.Test.Services;

/// <summary>
/// Tests del servicio de almacenamiento.
/// Se usa un wwwroot temporal por test, así que se escribe en disco de verdad y se borra al
/// terminar: no hay que simular el sistema de ficheros para comprobar que el archivo existe.
/// </summary>
[TestFixture]
public class StorageServiceTests
{
    private string _carpetaRaiz = null!;
    private StorageService _almacen = null!;

    [SetUp]
    public void SetUp()
    {
        // Carpeta temporal que hace de wwwroot; se borra en el TearDown.
        _carpetaRaiz = Path.Combine(Path.GetTempPath(), "almacen-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_carpetaRaiz);

        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Storage:UploadPath"] = "uploads",
                ["Storage:MaxFileSize"] = "1024",
                ["Storage:AllowedExtensions:0"] = ".jpg",
                ["Storage:AllowedExtensions:1"] = ".png"
            })
            .Build();

        var entorno = new Mock<IWebHostEnvironment>();
        entorno.Setup(e => e.WebRootPath).Returns(_carpetaRaiz);

        _almacen = new StorageService(
            entorno.Object, configuracion, NullLogger<StorageService>.Instance);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_carpetaRaiz))
        {
            Directory.Delete(_carpetaRaiz, true);
        }
    }

    /// <summary>
    /// Monta un IFormFile con los bytes dados, igual que lo haría el binder con una subida real.
    /// </summary>
    /// <param name="nombre">Nombre del fichero tal y como lo escribiría el navegador.</param>
    /// <param name="contenido">Bytes del fichero.</param>
    /// <returns>Fichero de formulario listo para pasar al servicio.</returns>
    private static IFormFile Fichero(string nombre, byte[] contenido)
    {
        var flujo = new MemoryStream(contenido);
        return new FormFile(flujo, 0, contenido.Length, "fichero", nombre);
    }

    /// <summary>
    /// Cabecera JPEG auténtica: FF D8 FF seguido de relleno, que es todo lo que mira el servicio.
    /// </summary>
    /// <param name="total">Tamaño final del fichero de prueba.</param>
    /// <returns>Contenido con firma JPEG correcta.</returns>
    private static byte[] ContenidoJpeg(int total = 64)
    {
        var bytes = new byte[total];
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        bytes[2] = 0xFF;
        return bytes;
    }

    /// <summary>
    /// Cabecera PNG auténtica: los 8 bytes de la firma, seguidos de relleno.
    /// </summary>
    /// <param name="total">Tamaño final del fichero de prueba.</param>
    /// <returns>Contenido con firma PNG correcta.</returns>
    private static byte[] ContenidoPng(int total = 64)
    {
        var bytes = new byte[total];
        byte[] firma = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        firma.CopyTo(bytes, 0);
        return bytes;
    }

    [Test]
    public async Task SaveFileAsync_FicheroValido_EscribeElArchivoYDevuelveLaRuta()
    {
        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "productos");

        // Assert
        resultado.Ok.Should().BeTrue();
        resultado.Ruta.Should().StartWith("/uploads/productos/");
        resultado.Ruta.Should().EndWith(".jpg");

        // El fichero existe de verdad en el wwwroot temporal.
        var fisica = _almacen.GetPhysicalPath(resultado.Ruta);
        fisica.Should().NotBeNull();
        File.Exists(fisica).Should().BeTrue();
    }

    [Test]
    public async Task SaveFileAsync_FicheroValido_ContieneLosBytesOriginales()
    {
        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg(128)), "productos");

        // Assert: lo que se escribe en disco es exactamente lo que llegó.
        var fisica = _almacen.GetPhysicalPath(resultado.Ruta)!;
        new FileInfo(fisica).Length.Should().Be(128);
    }

    [Test]
    public async Task SaveFileAsync_SinFichero_DevuelveElFallo()
    {
        // Act
        var resultado = await _almacen.SaveFileAsync(null, "productos");

        // Assert
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain("ningún fichero");
        resultado.Ruta.Should().BeEmpty();
    }

    [Test]
    public async Task SaveFileAsync_FicheroVacio_DevuelveElFallo()
    {
        // Act: un fichero de cero bytes, que es lo que llega si el navegador no eligió nada.
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", []), "productos");

        // Assert
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain("ningún fichero");
    }

    [Test]
    public async Task SaveFileAsync_ExtensionNoPermitida_DevuelveElFallo()
    {
        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("programa.exe", ContenidoJpeg()), "productos");

        // Assert: el mensaje nombra la extensión y las que sí se admiten.
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain(".exe");
        resultado.Mensaje.Should().Contain(".jpg");
    }

    [Test]
    public async Task SaveFileAsync_PorEncimaDelTecho_DevuelveElFallo()
    {
        // Arrange: el techo de la configuración son 1024 bytes.
        var contenido = ContenidoJpeg(2048);

        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", contenido), "productos");

        // Assert
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain("demasiado grande");
    }

    [Test]
    public async Task SaveFileAsync_FirmaQueNoCorresponde_DevuelveElFallo()
    {
        // Arrange: texto plano que el navegador envía como .jpg.
        var contenido = "esto no es una imagen"u8.ToArray();

        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", contenido), "productos");

        // Assert: la extensión pasa la lista blanca, pero la firma no.
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain("no corresponde");
    }

    [Test]
    public async Task SaveFileAsync_PngValido_LoAcepta()
    {
        // Act
        var resultado = await _almacen.SaveFileAsync(Fichero("logotipo.png", ContenidoPng()), "productos");

        // Assert
        resultado.Ok.Should().BeTrue();
        resultado.Ruta.Should().EndWith(".png");
    }

    [Test]
    public async Task SaveFileAsync_DosSubidasSeguidas_NoPisanLaRuta()
    {
        // Act: dos ficheros con el mismo nombre, uno detrás de otro.
        var primero = await _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "productos");
        var segundo = await _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "productos");

        // Assert: el nombre único del servidor evita que el segundo tape al primero.
        primero.Ruta.Should().NotBe(segundo.Ruta);
        File.Exists(_almacen.GetPhysicalPath(primero.Ruta)).Should().BeTrue();
        File.Exists(_almacen.GetPhysicalPath(segundo.Ruta)).Should().BeTrue();
    }

    [Test]
    public async Task SaveFileAsync_NombreConRuta_NoEscribeFueraDelAlmacen()
    {
        // Act: un nombre con .. que intentaría salirse de la carpeta.
        var resultado = await _almacen.SaveFileAsync(
            Fichero("../../fuera.jpg", ContenidoJpeg()), "productos");

        // Assert: la ruta pública sigue dentro de uploads y el archivo está bajo la raíz.
        resultado.Ok.Should().BeTrue();
        resultado.Ruta.Should().StartWith("/uploads/productos/");
        resultado.Ruta.Should().NotContain("..");

        var fisica = _almacen.GetPhysicalPath(resultado.Ruta)!;
        Path.GetFullPath(fisica).Should().StartWith(Path.GetFullPath(_carpetaRaiz));
    }

    [Test]
    public async Task SaveFileAsync_NombreSoloDePuntos_LlegaAlServidorConNombrePropio()
    {
        // Act: un nombre sin ningún carácter útil tras sanear.
        var resultado = await _almacen.SaveFileAsync(Fichero("....jpg", ContenidoJpeg()), "productos");

        // Assert: el servidor pone su propio nombre en lugar de quedarse con uno vacío.
        resultado.Ok.Should().BeTrue();
        resultado.Ruta.Should().NotContain("....");
    }

    [Test]
    public async Task SaveFileAsync_CarpetaVacia_DevuelveElFallo()
    {
        // Act: la carpeta la pone la aplicación, y una cadena en blanco no sirve.
        var resultado = await _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "   ");

        // Assert
        resultado.Ok.Should().BeFalse();
        resultado.Mensaje.Should().Contain("carpeta");
    }

    [Test]
    public async Task SaveFileAsync_CarpetaConRuta_NoEscribeFueraDelAlmacen()
    {
        // Act: una carpeta con .. que intentaría saltarse el almacén.
        var resultado = await _almacen.SaveFileAsync(
            Fichero("foto.jpg", ContenidoJpeg()), "../../carpeta");

        // Assert: Path.GetFileName se queda con el último tramo, que es inofensivo.
        resultado.Ok.Should().BeTrue();
        resultado.Ruta.Should().StartWith("/uploads/");
        resultado.Ruta.Should().NotContain("..");
    }

    [Test]
    public void FileExists_FicheroGuardado_DevuelveTrue()
    {
        // Arrange
        var ruta = _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "productos")
            .GetAwaiter().GetResult().Ruta;

        // Act & Assert
        _almacen.FileExists(ruta).Should().BeTrue();
    }

    [Test]
    public void FileExists_RutaInventada_DevuelveFalse()
    {
        // Act & Assert
        _almacen.FileExists("/uploads/productos/no-existe.jpg").Should().BeFalse();
    }

    [Test]
    public void FileExists_RutaVacia_DevuelveFalse()
    {
        // Act & Assert
        _almacen.FileExists("   ").Should().BeFalse();
    }

    [Test]
    public void FileExists_RutaQueEscapaDelAlmacen_DevuelveFalse()
    {
        // Act: ../../ recorre hacia arriba desde uploads.
        _almacen.FileExists("/uploads/../../appsettings.json").Should().BeFalse();
    }

    [Test]
    public void GetPhysicalPath_RutaValida_DevuelveLaRutaCompleta()
    {
        // Arrange
        var ruta = _almacen.SaveFileAsync(Fichero("foto.jpg", ContenidoJpeg()), "productos")
            .GetAwaiter().GetResult().Ruta;

        // Act
        var fisica = _almacen.GetPhysicalPath(ruta);

        // Assert
        fisica.Should().NotBeNull();
        Path.GetFullPath(fisica).Should().StartWith(Path.GetFullPath(_carpetaRaiz));
    }

    [Test]
    public void GetPhysicalPath_RutaQueEscapa_DevuelveNull()
    {
        // Act & Assert: no devuelve nada, que es la red de seguridad contra el path traversal.
        _almacen.GetPhysicalPath("/uploads/../../appsettings.json").Should().BeNull();
        _almacen.GetPhysicalPath("C:\\Windows\\system32\\config").Should().BeNull();
    }

    [Test]
    public void GetPhysicalPath_ImagenDePorDefecto_DevuelveLaRutaCompleta()
    {
        // Arrange: la imagen de por defecto vive en wwwroot/images, fuera de la carpeta de
        // subidas. Tiene que resolverse igual, o los productos sin subir no tendrían enlace.
        var carpeta = Path.Combine(_carpetaRaiz, "images");
        Directory.CreateDirectory(carpeta);
        var esperada = Path.GetFullPath(Path.Combine(carpeta, "producto-sin-imagen.svg"));
        File.WriteAllText(esperada, "<svg></svg>");

        // Act
        var fisica = _almacen.GetPhysicalPath("/images/producto-sin-imagen.svg");

        // Assert
        fisica.Should().Be(esperada);
        _almacen.FileExists("/images/producto-sin-imagen.svg").Should().BeTrue();
    }
}
