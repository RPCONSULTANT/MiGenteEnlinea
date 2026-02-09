using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Authentication.DTOs;

/// <summary>
/// DTO para credenciales de usuario
/// </summary>
public class CredencialDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
    [JsonPropertyName("fechaCreacion")]
    public DateTime? FechaCreacion { get; set; }
    [JsonPropertyName("ultimoAcceso")]
    public DateTime? UltimoAcceso { get; set; }
}
