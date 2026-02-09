namespace MiGenteEnLinea.Application.Common.Interfaces;

/// <summary>
/// Interface para manejar el almacenamiento de archivos en el servidor
/// Permite guardar, recuperar y eliminar archivos de forma agnóstica
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Guarda un archivo en el servidor y devuelve la URL relativa
    /// </summary>
    /// <param name="file">El archivo a guardar (stream)</param>
    /// <param name="fileName">Nombre original del archivo</param>
    /// <param name="folder">Carpeta de destino (ej: "contratistas-fotos")</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Ruta relativa al servidor (ej: "/uploads/contratistas-fotos/abc123.jpg")</returns>
    Task<string> SaveFileAsync(Stream file, string fileName, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera un archivo del servidor como stream
    /// </summary>
    /// <param name="filePath">Ruta relativa del archivo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Stream del archivo, o null si no existe</returns>
    Task<Stream?> GetFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un archivo del servidor
    /// </summary>
    /// <param name="filePath">Ruta relativa del archivo a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>true si se eliminó, false si no existía o error</returns>
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un archivo existe
    /// </summary>
    /// <param name="filePath">Ruta relativa del archivo</param>
    /// <returns>true si el archivo existe</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Genera un nombre único para el archivo para evitar colisiones
    /// </summary>
    /// <param name="originalFileName">Nombre original del archivo</param>
    /// <returns>Nombre único con timestamp y GUID</returns>
    string GenerateUniqueFileName(string originalFileName);
}
