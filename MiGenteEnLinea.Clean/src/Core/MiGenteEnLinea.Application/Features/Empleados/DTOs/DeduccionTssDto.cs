using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para deducción de TSS (Tesorería de la Seguridad Social).
/// Representa un catálogo de deducciones con su porcentaje correspondiente.
/// </summary>
public class DeduccionTssDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
    [JsonPropertyName("porcentaje")]
    public decimal Porcentaje { get; set; }
}
