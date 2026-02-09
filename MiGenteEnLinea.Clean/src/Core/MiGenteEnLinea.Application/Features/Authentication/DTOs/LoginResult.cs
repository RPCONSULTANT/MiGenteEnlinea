using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Authentication.DTOs;

/// <summary>
/// Resultado de la operación de login
/// </summary>
/// <remarks>
/// StatusCode compatible con Legacy:
/// - 2: Login exitoso
/// - 0: Credenciales inválidas
/// - -1: Cuenta inactiva
/// </remarks>
public class LoginResult
{
    /// <summary>
    /// Código de estado (2=success, 0=invalid, -1=inactive)
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>
    /// ID del usuario (GUID)
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Email del usuario
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    /// <summary>
    /// Tipo de usuario (1=Empleador, 2=Contratista)
    /// </summary>
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }

    /// <summary>
    /// ID del plan activo
    /// </summary>
    [JsonPropertyName("planId")]
    public int? PlanId { get; set; }

    /// <summary>
    /// Fecha de vencimiento del plan
    /// </summary>
    [JsonPropertyName("vencimientoPlan")]
    public DateTime? VencimientoPlan { get; set; }

    /// <summary>
    /// Número de nóminas permitidas en el plan
    /// </summary>
    [JsonPropertyName("nomina")]
    public int? Nomina { get; set; }

    /// <summary>
    /// Número de empleados permitidos en el plan
    /// </summary>
    [JsonPropertyName("empleados")]
    public int? Empleados { get; set; }

    /// <summary>
    /// Indica si el plan incluye histórico
    /// </summary>
    [JsonPropertyName("historico")]
    public bool? Historico { get; set; }

    /// <summary>
    /// Información del perfil del usuario
    /// </summary>
    [JsonPropertyName("perfil")]
    public PerfilDto? Perfil { get; set; }

    /// <summary>
    /// Token JWT (se agregará cuando se implemente JWT)
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token (se agregará cuando se implemente JWT)
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}
