using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Authentication.DTOs;

/// <summary>
/// Resultado de la operación de registro
/// </summary>
public class RegisterResult
{
    /// <summary>
    /// Indica si el registro fue exitoso
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje de resultado
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID del usuario creado (GUID)
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Email del usuario creado
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
