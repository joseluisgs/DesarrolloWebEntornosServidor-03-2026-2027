using Microsoft.AspNetCore.Http;

namespace ProductosWeb.Shared.Services.Storage;

/// <summary>
/// Almacenamiento de ficheros subidos por los formularios.
/// Las dos visiones llaman a este interfaz y ninguna de las dos sabe dónde se guarda nada:
/// el guardado, el nombre del fichero y sus límites son responsabilidad del servicio.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Valida un fichero subido y lo escribe en disco con un nombre que decide el servidor.
    /// </summary>
    /// <param name="file">Fichero recibido en el formulario; null si el usuario no eligió ninguno.</param>
    /// <param name="folder">Subcarpeta del almacén, por ejemplo "productos".</param>
    /// <returns>Resultado con la ruta pública si se guardó, o el motivo del rechazo.</returns>
    Task<StorageResult> SaveFileAsync(IFormFile? file, string folder);

    /// <summary>
    /// Comprueba si una ruta pública corresponde a un fichero que existe bajo wwwroot.
    /// </summary>
    /// <param name="publicPath">Ruta pública, por ejemplo /uploads/productos/foto.jpg.</param>
    /// <returns>true si el fichero está en disco.</returns>
    bool FileExists(string publicPath);

    /// <summary>
    /// Devuelve la ruta absoluta en disco de una ruta pública servida desde wwwroot.
    /// Solo se aceptan rutas que caen dentro de wwwroot: una ruta que escapa con
    /// <c>..</c> o una ruta absoluta del sistema no resuelve a nada.
    /// </summary>
    /// <param name="publicPath">Ruta pública, por ejemplo /uploads/productos/foto.jpg.</param>
    /// <returns>La ruta absoluta, o null si no está dentro de wwwroot.</returns>
    string? GetPhysicalPath(string publicPath);
}
