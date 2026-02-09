using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Authentication.DTOs;

/// <summary>
/// DTO de respuesta para operaciones de autenticación (Login, Refresh)
/// </summary>
public class AuthenticationResultDto
{
    /// <summary>
    /// Access token JWT (duración: 15 minutos)
    /// Usar en header: Authorization: Bearer {AccessToken}
    /// </summary>
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token para renovar el access token (duración: 7 días)
    /// Almacenar en HttpOnly cookie o localStorage
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de expiración del access token (UTC)
    /// </summary>
    [JsonPropertyName("accessTokenExpires")]
    public DateTime AccessTokenExpires { get; set; }

    /// <summary>
    /// Fecha de expiración del refresh token (UTC)
    /// </summary>
    [JsonPropertyName("refreshTokenExpires")]
    public DateTime RefreshTokenExpires { get; set; }

    /// <summary>
    /// Información del usuario autenticado
    /// </summary>
    [JsonPropertyName("user")]
    public UserInfoDto User { get; set; } = new();
}

/// <summary>
/// Información del usuario autenticado
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// ID del usuario (AspNetUsers.Id)
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario: "1" (Empleador) o "2" (Contratista)
    /// </summary>
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// ID del plan de suscripción (0 = sin plan)
    /// </summary>
    [JsonPropertyName("planId")]
    public int PlanId { get; set; }

    /// <summary>
    /// Fecha de vencimiento del plan (null si no tiene plan)
    /// </summary>
    [JsonPropertyName("vencimientoPlan")]
    public DateTime? VencimientoPlan { get; set; }

    /// <summary>
    /// Indica si el plan está activo (no expirado)
    /// </summary>
    public bool PlanActivo => VencimientoPlan.HasValue && VencimientoPlan.Value > DateTime.UtcNow;

    /// <summary>
    /// Roles del usuario (de Identity)
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
}
