using Microsoft.AspNetCore.Http;

namespace ProductosWeb.Shared.Services.Storage;

/// <summary>
/// Almacenamiento de los ficheros subidos en la carpeta <c>wwwroot/uploads</c>.
/// Es el único sitio que escribe en el disco del almacén: los formularios solo pasan el
/// <see cref="IFormFile" /> y se llevan un <see cref="StorageResult" /> con la ruta pública.
/// Los límites y la ruta viven en la sección Storage de la configuración, no en el código.
/// </summary>
/// <param name="environment">Entorno web: aporta la raíz de ficheros estáticos (wwwroot).</param>
/// <param name="configuration">Configuración: sección Storage (ruta, techo y extensiones).</param>
/// <param name="logger">Logger del servicio.</param>
public class StorageService(
    IWebHostEnvironment environment,
    IConfiguration configuration,
    ILogger<StorageService> logger) : IStorageService
{
    /// <summary>Carpeta de subidas dentro de wwwroot.</summary>
    private readonly string _uploadPath = configuration["Storage:UploadPath"] ?? "uploads";

    /// <summary>Techo máximo por fichero, en bytes.</summary>
    private readonly long _maxFileSize = configuration.GetValue<long>("Storage:MaxFileSize", 5242880);

    /// <summary>Lista blanca de extensiones admitidas.</summary>
    private readonly string[] _allowedExtensions =
        configuration.GetSection("Storage:AllowedExtensions").Get<string[]>()
        ?? [".jpg", ".jpeg", ".png", ".gif"];

    /// <summary>Raíz de ficheros estáticos (wwwroot): es contra lo que se resuelve una URL.</summary>
    private readonly string _webRootPath = environment.WebRootPath
        ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    /// <summary>Ruta absoluta del almacén: wwwroot + carpeta de subidas.</summary>
    private readonly string _rootPath = Path.Combine(
        environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
        configuration["Storage:UploadPath"] ?? "uploads");

    /// <inheritdoc />
    public async Task<StorageResult> SaveFileAsync(IFormFile? file, string folder)
    {
        // 1. El fichero tiene que existir y traer algo dentro.
        if (file is null || file.Length == 0)
        {
            return StorageResult.Fallo("No has elegido ningún fichero");
        }

        // 2. Techo por fichero: el del endpoint corta la petición, este corta el archivo.
        if (file.Length > _maxFileSize)
        {
            var megas = _maxFileSize / 1024 / 1024;
            return StorageResult.Fallo($"El fichero es demasiado grande: el máximo es {megas} MB");
        }

        // 3. Lista blanca de extensiones: decidida por la configuración.
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return StorageResult.Fallo(
                $"La extensión {extension} no está permitida; se admiten {string.Join(", ", _allowedExtensions)}");
        }

        // 4. Las firmas delatan el tipo real: un .exe renombrado a .jpg no empieza por FF D8 FF.
        if (!await FirmaValidaAsync(file, extension))
        {
            return StorageResult.Fallo($"El contenido del fichero no corresponde a una imagen {extension}");
        }

        // 5. La subcarpeta la pone el código de la aplicación, nunca el cliente.
        var carpeta = Path.GetFileName(folder);
        if (string.IsNullOrWhiteSpace(carpeta))
        {
            return StorageResult.Fallo("La carpeta de destino no es válida");
        }

        var destino = Path.Combine(_rootPath, carpeta);
        Directory.CreateDirectory(destino);

        // 6. Nombre único: marca temporal, guion corto y nombre saneado. El nombre que
        //    llegó del cliente solo aporta la parte legible; la ruta nunca se forma con él.
        var nombreUnico = $"{DateTime.UtcNow:yyyyMMddHHmmss}" +
                          $"_{Guid.NewGuid().ToString("N")[..8]}" +
                          $"_{NombreBase(file.FileName)}{extension}";

        await using (var stream = new FileStream(Path.Combine(destino, nombreUnico), FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var rutaPublica = $"/{_uploadPath}/{carpeta}/{nombreUnico}";
        logger.LogInformation("Fichero {Original} guardado como {Ruta}", file.FileName, rutaPublica);
        return StorageResult.Exito(rutaPublica);
    }

    /// <inheritdoc />
    public bool FileExists(string publicPath) =>
        GetPhysicalPath(publicPath) is { } fisica && File.Exists(fisica);

    /// <inheritdoc />
    public string? GetPhysicalPath(string publicPath)
    {
        if (string.IsNullOrWhiteSpace(publicPath))
        {
            return null;
        }

        // La ruta pública es una URL que se sirve desde wwwroot, así que primero se resuelve
        // contra wwwroot. Después se comprueba que el resultado siga DENTRO de wwwroot: una
        // ruta que escapa con ../ o una ruta absoluta del sistema no es un fichero de la web.
        // Con esta red se admiten también las imágenes que no son subidas, como la de por
        // defecto de wwwroot/images, que si no no tendría enlace de descarga.
        var raiz = Path.GetFullPath(_webRootPath);
        var relativo = publicPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var completo = Path.GetFullPath(Path.Combine(_webRootPath, relativo));

        return completo.StartsWith(raiz + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            ? completo
            : null;
    }

    /// <summary>
    /// Comprueba las primeras bytes del fichero (los magic numbers) contra la firma de su extensión.
    /// </summary>
    /// <param name="file">Fichero subido.</param>
    /// <param name="extension">Extensión declarada, ya en minúsculas.</param>
    /// <returns>true si el contenido corresponde a esa extensión.</returns>
    private static async Task<bool> FirmaValidaAsync(IFormFile file, string extension)
    {
        await using var stream = file.OpenReadStream();

        var cabecera = new byte[8];
        var leidos = await stream.ReadAsync(cabecera);
        if (leidos < 3)
        {
            return false;
        }

        if (extension is ".jpg" or ".jpeg")
        {
            // JPEG: FF D8 FF
            ReadOnlySpan<byte> firmaJpeg = [0xFF, 0xD8, 0xFF];
            return cabecera.AsSpan(0, 3).SequenceEqual(firmaJpeg);
        }

        if (extension is ".png")
        {
            // PNG: firma de 8 bytes
            ReadOnlySpan<byte> firmaPng = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
            return cabecera.AsSpan(0, 8).SequenceEqual(firmaPng);
        }

        if (extension is ".gif")
        {
            // GIF: cabecera ASCII "GIF87a" o "GIF89a"
            var firmaGif = "GIF87a"u8;
            var firmaGif89 = "GIF89a"u8;
            return cabecera.AsSpan(0, 6).SequenceEqual(firmaGif)
                   || cabecera.AsSpan(0, 6).SequenceEqual(firmaGif89);
        }

        return false;
    }

    /// <summary>
    /// Se queda con la parte final del nombre del cliente y la deja inofensiva:
    /// sin rutas, sin puntos dobles y solo con letras, dígitos, guiones y guiones bajos.
    /// </summary>
    /// <param name="fileName">Nombre original, tal cual llega del navegador.</param>
    /// <returns>La parte legible del nombre, lista para componer el nombre único.</returns>
    private static string NombreBase(string fileName)
    {
        // GetFileName se queda con lo que va tras la última barra: ../../appsettings.json -> appsettings.json
        var nombre = Path.GetFileNameWithoutExtension(fileName);

        var saneado = new string(nombre.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_').ToArray());
        if (saneado.Length == 0)
        {
            return "imagen";
        }

        return saneado.Length > 40 ? saneado[..40] : saneado;
    }
}
