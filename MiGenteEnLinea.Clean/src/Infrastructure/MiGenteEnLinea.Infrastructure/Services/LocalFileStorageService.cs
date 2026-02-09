using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Implementación de almacenamiento de archivos local en wwwroot
/// Guarda los archivos en el servidor y devuelve URLs relativas
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;
    private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Guarda un archivo en wwwroot/uploads/{folder}/ y devuelve la URL relativa
    /// </summary>
    public async Task<string> SaveFileAsync(
        Stream file,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar extensión
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException(
                    $"Extensión de archivo no permitida: {fileExtension}. " +
                    $"Permitidas: {string.Join(", ", AllowedExtensions)}");
            }

            // Validar tamaño
            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException(
                    $"El archivo es demasiado grande. Máximo permitido: 5MB, " +
                    $"archivo actual: {file.Length / 1024 / 1024}MB");
            }

            // Crear directorio si no existe
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsDir);

            // Generar nombre único
            var uniqueFileName = GenerateUniqueFileName(fileName);
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            // Guardar archivo
            using (var fileStream = System.IO.File.Create(filePath))
            {
                file.Position = 0;
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            // Devolver URL relativa
            var relativePath = Path.Combine("uploads", folder, uniqueFileName)
                .Replace("\\", "/");
            var urlPath = $"/{relativePath}";

            _logger.LogInformation(
                "Archivo guardado exitosamente. Folder: {Folder}, NombreArchivo: {FileName}, URL: {Url}",
                folder, uniqueFileName, urlPath);

            return urlPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar archivo. FileName: {FileName}, Folder: {Folder}",
                fileName, folder);
            throw;
        }
    }

    /// <summary>
    /// Recupera un archivo desde wwwroot/uploads/ como stream
    /// </summary>
    public async Task<Stream?> GetFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar la ruta (remover leading /)
            var normalizedPath = filePath.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("Archivo no encontrado: {FilePath}", filePath);
                return null;
            }

            // Retornar stream del archivo
            return System.IO.File.OpenRead(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recuperar archivo: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// Elimina un archivo de wwwroot/uploads/
    /// </summary>
    public async Task<bool> DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar la ruta
            var normalizedPath = filePath.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("Archivo a eliminar no encontrado: {FilePath}", filePath);
                return false;
            }

            System.IO.File.Delete(fullPath);
            _logger.LogInformation("Archivo eliminado exitosamente: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo: {FilePath}", filePath);
            return false;
        }
    }

    /// <summary>
    /// Verifica si un archivo existe
    /// </summary>
    public bool FileExists(string filePath)
    {
        try
        {
            var normalizedPath = filePath.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);
            return System.IO.File.Exists(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de archivo: {FilePath}", filePath);
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
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var guid = Guid.NewGuid().ToString("N");
        return $"{timestamp}_{guid}{extension}";
    }
}
