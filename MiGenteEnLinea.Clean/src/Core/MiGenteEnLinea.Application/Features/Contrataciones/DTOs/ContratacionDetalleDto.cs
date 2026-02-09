using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Contrataciones.DTOs;

/// <summary>
/// DTO completo con todos los detalles de una contratación.
/// Usado para vistas de detalle.
/// </summary>
public class ContratacionDetalleDto
{
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; set; }
    [JsonPropertyName("contratacionId")]
    public int? ContratacionId { get; set; }
    [JsonPropertyName("descripcionCorta")]
    public string DescripcionCorta { get; set; } = string.Empty;
    [JsonPropertyName("descripcionAmpliada")]
    public string? DescripcionAmpliada { get; set; }
    [JsonPropertyName("fechaInicio")]
    public DateOnly FechaInicio { get; set; }
    [JsonPropertyName("fechaFinal")]
    public DateOnly FechaFinal { get; set; }
    [JsonPropertyName("montoAcordado")]
    public decimal MontoAcordado { get; set; }
    [JsonPropertyName("esquemaPagos")]
    public string? EsquemaPagos { get; set; }
    [JsonPropertyName("estatus")]
    public int Estatus { get; set; }
    [JsonPropertyName("nombreEstado")]
    public string NombreEstado { get; set; } = string.Empty;
    [JsonPropertyName("calificado")]
    public bool Calificado { get; set; }
    [JsonPropertyName("calificacionId")]
    public int? CalificacionId { get; set; }
    [JsonPropertyName("notas")]
    public string? Notas { get; set; }
    [JsonPropertyName("motivoCancelacion")]
    public string? MotivoCancelacion { get; set; }
    [JsonPropertyName("fechaInicioReal")]
    public DateTime? FechaInicioReal { get; set; }
    [JsonPropertyName("fechaFinalizacionReal")]
    public DateTime? FechaFinalizacionReal { get; set; }
    [JsonPropertyName("porcentajeAvance")]
    public int PorcentajeAvance { get; set; }

    // Propiedades calculadas
    [JsonPropertyName("duracionEstimadaDias")]
    public int DuracionEstimadaDias { get; set; }
    [JsonPropertyName("duracionRealDias")]
    public int? DuracionRealDias { get; set; }
    [JsonPropertyName("estaRetrasada")]
    public bool EstaRetrasada { get; set; }
    [JsonPropertyName("puedeSerCalificada")]
    public bool PuedeSerCalificada { get; set; }
    [JsonPropertyName("puedeSerCancelada")]
    public bool PuedeSerCancelada { get; set; }
    [JsonPropertyName("puedeSerModificada")]
    public bool PuedeSerModificada { get; set; }
}
