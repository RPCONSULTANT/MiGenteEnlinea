using MediatR;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratistaFoto;

/// <summary>
/// Command: Actualizar la URL de foto de perfil del Contratista
/// Se ejecuta después que el archivo ha sido guardado en el servidor (por IFileStorageService)
/// </summary>
/// <remarks>
/// FLUJO:
/// 1. Controller recibe archivo
/// 2. Controller usa IFileStorageService para guardar en wwwroot/uploads/
/// 3. IFileStorageService devuelve URL relativa (/uploads/contratistas-fotos/abc123.jpg)
/// 4. Controller envía UpdateContratistaFotoCommand con esa URL
/// 5. Handler busca el contratista y actualiza su ImagenUrl
/// 6. Se persiste en BD
/// </remarks>
public record UpdateContratistaFotoCommand(
    string UserId,
    string FotoUrl  // URL relativa: /uploads/contratistas-fotos/20260209_abc123.jpg
) : IRequest<UpdateContratistaFotoResult>;

/// <summary>
/// Resultado de la operación de actualizar foto
/// </summary>
public record UpdateContratistaFotoResult
{
    /// <summary>
    /// true si la foto se actualizó exitosamente
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Mensaje descriptivo (éxito o error)
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// URL de la foto actualizada
    /// </summary>
    public string? FotoUrl { get; init; }

    public static UpdateContratistaFotoResult SuccessResult(string fotoUrl) =>
        new() { Success = true, Message = "Foto actualizada exitosamente", FotoUrl = fotoUrl };

    public static UpdateContratistaFotoResult FailureResult(string message) =>
        new() { Success = false, Message = message, FotoUrl = null };
}
