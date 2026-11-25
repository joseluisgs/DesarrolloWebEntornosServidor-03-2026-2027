namespace ProductosWeb.Shared.Services.Storage;

/// <summary>
/// Resultado de una operación de almacenamiento.
/// Un fichero puede fallar por muchos motivos (vacío, tamaño, extensión, firma) y el formulario
/// necesita el motivo para pintarlo: el servicio no lanza excepciones, devuelve el resultado.
/// </summary>
/// <param name="Ok">Si el fichero se guardó (o la operación salió bien).</param>
/// <param name="Mensaje">Motivo del fallo; vacío cuando todo ha ido bien.</param>
/// <param name="Ruta">Ruta pública del fichero guardado, por ejemplo /uploads/productos/foto.jpg.</param>
public record StorageResult(bool Ok, string Mensaje, string Ruta)
{
    /// <summary>
    /// Resultado de una guardada correcta.
    /// </summary>
    /// <param name="ruta">Ruta pública del fichero recién escrito.</param>
    /// <returns>Resultado marcado como correcto.</returns>
    public static StorageResult Exito(string ruta) => new(true, string.Empty, ruta);

    /// <summary>
    /// Resultado de una operación rechazada.
    /// </summary>
    /// <param name="mensaje">Motivo legible para quien usa el formulario.</param>
    /// <returns>Resultado marcado como fallido, sin ruta.</returns>
    public static StorageResult Fallo(string mensaje) => new(false, mensaje, string.Empty);
}
