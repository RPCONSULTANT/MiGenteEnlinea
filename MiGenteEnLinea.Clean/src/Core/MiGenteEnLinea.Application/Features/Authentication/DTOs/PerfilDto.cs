using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Authentication.DTOs;

/// <summary>
/// DTO para el perfil de usuario (basado en VPerfiles view)
/// </summary>
public class PerfilDto
{
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("emailUsuario")]
    public string? EmailUsuario { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }
    [JsonPropertyName("telefono1")]
    public string? Telefono1 { get; set; }
    [JsonPropertyName("telefono2")]
    public string? Telefono2 { get; set; }
    [JsonPropertyName("fechaCreacion")]
    public DateTime? FechaCreacion { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("cuentaId")]
    public int? CuentaId { get; set; }
    [JsonPropertyName("perfilId")]
    public int? PerfilId { get; set; }
    [JsonPropertyName("sexo")]
    public string? Sexo { get; set; }
    [JsonPropertyName("fechaNacimiento")]
    public DateTime? FechaNacimiento { get; set; }
    [JsonPropertyName("estadoCivil")]
    public string? EstadoCivil { get; set; }
    [JsonPropertyName("provinciaId")]
    public int? ProvinciaId { get; set; }
    [JsonPropertyName("provinciaStr")]
    public string? ProvinciaStr { get; set; }
    [JsonPropertyName("sector")]
    public string? Sector { get; set; }
    [JsonPropertyName("calle")]
    public string? Calle { get; set; }
    [JsonPropertyName("numeroCasa")]
    public string? NumeroCasa { get; set; }
}
