using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Infrastructure.Options;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Implementación de almacenamiento de archivos local en wwwroot
/// Guarda los archivos en el servidor y devuelve URLs relativas
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _basePath;
    private readonly FileStorageOptions _options;
    private readonly HashSet<string> _allowedExtensions;
    private readonly HashSet<string> _allowedMimeTypes;
    private readonly HashSet<string> _allowedFolders;
    private readonly int _maxFileSizeBytes;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options,
        ILogger<LocalFileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        var configuredRoot = (_options.RootFolder ?? string.Empty).Trim();
        var defaultRoot = string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
            : environment.WebRootPath;

        _basePath = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.GetFullPath(defaultRoot)
            : Path.GetFullPath(configuredRoot);

        _allowedExtensions = (_options.AllowedExtensions ?? [])
            .Select(x => NormalizeExtension(x))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _allowedMimeTypes = (_options.AllowedMimeTypes ?? [])
            .Select(x => (x ?? string.Empty).Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _allowedFolders = (_options.AllowedFolders ?? [])
            .Select(x => x?.Trim() ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _maxFileSizeBytes = Math.Max(1, _options.MaxFileSizeMB) * 1024 * 1024;

        Directory.CreateDirectory(_basePath);

        _logger.LogInformation(
            "LocalFileStorageService inicializado. BasePath: {BasePath}, MaxFileSizeMB: {MaxFileSizeMB}, AllowedFolders: {AllowedFolders}",
            _basePath,
            _options.MaxFileSizeMB,
            string.Join(", ", _allowedFolders));
    }

    /// <summary>
    /// Guarda un archivo en wwwroot/uploads/{folder}/ y devuelve la URL relativa
    /// </summary>
    public async Task<string> SaveFileAsync(
        Stream file,
        string fileName,
        string folder,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateFolder(folder);
            ValidateStreamLength(file);

            var fileExtension = NormalizeExtension(Path.GetExtension(fileName));
            if (!_allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException($"Extensión no permitida: {fileExtension}");

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                var normalizedContentType = contentType.Trim().ToLowerInvariant();
                if (!_allowedMimeTypes.Contains(normalizedContentType))
                    throw new InvalidOperationException($"Content-Type no permitido: {normalizedContentType}");
            }

            var uploadsDir = GetSafeUploadsFolderPath(folder);
            Directory.CreateDirectory(uploadsDir);

            var uniqueFileName = GenerateUniqueFileName(fileName);
            var filePath = Path.Combine(uploadsDir, uniqueFileName);
            var fullFilePath = EnsurePathInsideRoot(filePath);

            await ValidateFileSignatureAsync(file, fileExtension, cancellationToken);

            await using (var fileStream = System.IO.File.Create(fullFilePath))
            {
                file.Position = 0;
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            var relativePath = Path.Combine("uploads", folder, uniqueFileName)
                .Replace("\\", "/");
            var urlPath = $"/{relativePath}";

            _logger.LogInformation(
                "file.upload.success folder={Folder} file={FileName} url={Url} size={SizeBytes}",
                folder,
                uniqueFileName,
                urlPath,
                file.Length);

            return urlPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "file.upload.fail file={FileName} folder={Folder}",
                fileName, folder);
            throw;
        }
    }

    /// <summary>
    /// Recupera un archivo desde wwwroot/uploads/ como stream
    /// </summary>
    public Task<Stream?> GetFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = ResolveAndValidateRelativeFilePath(filePath);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("file.read.not_found path={FilePath}", filePath);
                return Task.FromResult<Stream?>(null);
            }

            return Task.FromResult<Stream?>(System.IO.File.OpenRead(fullPath));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "file.read.rejected path={FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "file.read.fail path={FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// Elimina un archivo de wwwroot/uploads/
    /// </summary>
    public Task<bool> DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = ResolveAndValidateRelativeFilePath(filePath);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("file.delete.not_found path={FilePath}", filePath);
                return Task.FromResult(false);
            }

            System.IO.File.Delete(fullPath);
            _logger.LogInformation("file.delete.success path={FilePath}", filePath);
            return Task.FromResult(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "file.delete.rejected path={FilePath}", filePath);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "file.delete.fail path={FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Verifica si un archivo existe
    /// </summary>
    public bool FileExists(string filePath)
    {
        try
        {
            var fullPath = ResolveAndValidateRelativeFilePath(filePath);
            return System.IO.File.Exists(fullPath);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "file.exists.rejected path={FilePath}", filePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "file.exists.fail path={FilePath}", filePath);
            return false;
        }
    }

    /// <summary>
    /// Genera un nombre único para evitar colisiones de archivos
    /// Formato: {timestamp}_{guid}{extension}
    /// Ejemplo: 20260209_123456_a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5.jpg
    /// </summary>
    public string GenerateUniqueFileName(string originalFileName)
    {
        var extension = NormalizeExtension(Path.GetExtension(originalFileName));
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var guid = Guid.NewGuid().ToString("N");
        return $"{timestamp}_{guid}{extension}";
    }

    private string GetSafeUploadsFolderPath(string folder)
    {
        var normalizedFolder = NormalizeFolder(folder);
        var uploadsRoot = EnsurePathInsideRoot(Path.Combine(_basePath, "uploads"));
        return EnsurePathInsideRoot(Path.Combine(uploadsRoot, normalizedFolder));
    }

    private string ResolveAndValidateRelativeFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("La ruta del archivo es requerida.");

        var normalizedPath = filePath.Replace('\\', '/').Trim();
        if (normalizedPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            normalizedPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("No se permiten URLs absolutas en operaciones de filesystem.");

        normalizedPath = normalizedPath.TrimStart('/');
        if (normalizedPath.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Ruta inválida.");

        var fullPath = Path.Combine(_basePath, normalizedPath);
        return EnsurePathInsideRoot(fullPath);
    }

    private string EnsurePathInsideRoot(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var normalizedRoot = _basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fullPath, _basePath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Ruta fuera del directorio permitido.");

        return fullPath;
    }

    private void ValidateFolder(string folder)
    {
        var normalizedFolder = NormalizeFolder(folder);
        if (_allowedFolders.Count > 0 && !_allowedFolders.Contains(normalizedFolder))
            throw new InvalidOperationException($"Carpeta no permitida: {normalizedFolder}");
    }

    private static string NormalizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new InvalidOperationException("La carpeta de destino es requerida.");

        var normalized = folder.Trim().Replace('\\', '/').Trim('/');
        if (normalized.Contains("..", StringComparison.Ordinal) || normalized.Contains('/', StringComparison.Ordinal))
            throw new InvalidOperationException("Nombre de carpeta inválido.");

        return normalized;
    }

    private void ValidateStreamLength(Stream file)
    {
        if (!file.CanRead)
            throw new InvalidOperationException("El stream de archivo no es legible.");

        if (!file.CanSeek)
            throw new InvalidOperationException("El stream de archivo debe soportar seek para validaciones.");

        if (file.Length <= 0)
            throw new InvalidOperationException("El archivo está vacío.");

        if (file.Length > _maxFileSizeBytes)
            throw new InvalidOperationException($"Archivo excede el tamaño máximo permitido de {_options.MaxFileSizeMB}MB.");
    }

    private async Task ValidateFileSignatureAsync(Stream file, string extension, CancellationToken cancellationToken)
    {
        file.Position = 0;
        var header = new byte[16];
        var bytesRead = await file.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
        file.Position = 0;

        if (bytesRead == 0)
            throw new InvalidOperationException("Archivo vacío.");

        if (!IsExpectedSignature(header, bytesRead, extension))
            throw new InvalidOperationException("Firma binaria inválida para la extensión proporcionada.");
    }

    private static bool IsExpectedSignature(byte[] header, int bytesRead, string extension)
    {
        if (bytesRead < 4)
            return false;

        return extension switch
        {
            ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
            ".gif" => bytesRead >= 6 &&
                      header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 &&
                      header[3] == 0x38 && (header[4] == 0x37 || header[4] == 0x39) && header[5] == 0x61,
            ".webp" => bytesRead >= 12 &&
                       header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                       header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
            _ => false
        };
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        var normalized = extension.Trim().ToLowerInvariant();
        return normalized.StartsWith('.') ? normalized : $".{normalized}";
    }
}
