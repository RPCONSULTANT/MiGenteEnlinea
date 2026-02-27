namespace MiGenteEnLinea.Infrastructure.Options;

/// <summary>
/// Configuración centralizada para almacenamiento de archivos en filesystem.
/// </summary>
public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>
    /// Ruta raíz absoluta opcional. Si está vacía se usa wwwroot.
    /// </summary>
    public string RootFolder { get; set; } = string.Empty;

    /// <summary>
    /// URL pública base opcional para construir URLs absolutas.
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Extensiones permitidas para upload.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    /// <summary>
    /// Content-Types permitidos para upload.
    /// </summary>
    public string[] AllowedMimeTypes { get; set; } = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    /// <summary>
    /// Tamaño máximo por archivo en megabytes.
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 5;

    /// <summary>
    /// Activar re-encode/sanitización de imagen (pendiente de implementación).
    /// </summary>
    public bool EnableImageReencode { get; set; }

    /// <summary>
    /// Carpetas lógicas permitidas para uploads.
    /// </summary>
    public string[] AllowedFolders { get; set; } = ["contratistas-fotos", "empleadores-fotos"];
}
