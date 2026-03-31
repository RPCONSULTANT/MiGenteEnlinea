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

    /// <summary>
    /// Indica si el correo de activación fue enviado exitosamente.
    /// </summary>
    [JsonPropertyName("activationEmailSent")]
    public bool ActivationEmailSent { get; set; }

    /// <summary>
    /// Mensaje de diagnóstico del correo de activación cuando falle.
    /// </summary>
    [JsonPropertyName("activationEmailMessage")]
    public string? ActivationEmailMessage { get; set; }
}
