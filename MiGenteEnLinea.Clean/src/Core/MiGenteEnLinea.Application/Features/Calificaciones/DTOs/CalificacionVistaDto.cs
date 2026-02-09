using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

/// <summary>
/// DTO para vista de calificaciones (equivalente a VCalificaciones del Legacy)
/// </summary>
public class CalificacionVistaDto
{
    [JsonPropertyName("calificacionId")]
    public int CalificacionId { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("identificacion")]
    public string Identificacion { get; set; } = string.Empty;
    [JsonPropertyName("puntuacion")]
    public int Puntuacion { get; set; }
    [JsonPropertyName("comentario")]
    public string? Comentario { get; set; }
    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }
    [JsonPropertyName("nombreCalificador")]
    public string NombreCalificador { get; set; } = string.Empty;
    [JsonPropertyName("apellidoCalificador")]
    public string ApellidoCalificador { get; set; } = string.Empty;
}
