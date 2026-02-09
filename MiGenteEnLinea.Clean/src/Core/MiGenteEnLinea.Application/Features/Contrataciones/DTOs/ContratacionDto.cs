using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Contrataciones.DTOs;

/// <summary>
/// DTO simplificado para listados de contrataciones.
/// </summary>
public class ContratacionDto
{
    /// <summary>
    /// ID del detalle de contratación
    /// </summary>
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; set; }

    /// <summary>
    /// ID de la contratación padre (EmpleadoTemporal)
    /// </summary>
    [JsonPropertyName("contratacionId")]
    public int? ContratacionId { get; set; }

    /// <summary>
    /// Descripción breve del trabajo
    /// </summary>
    [JsonPropertyName("descripcionCorta")]
    public string DescripcionCorta { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de inicio acordada
    /// </summary>
    [JsonPropertyName("fechaInicio")]
    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización acordada
    /// </summary>
    [JsonPropertyName("fechaFinal")]
    public DateOnly FechaFinal { get; set; }

    /// <summary>
    /// Monto total acordado
    /// </summary>
    [JsonPropertyName("montoAcordado")]
    public decimal MontoAcordado { get; set; }

    /// <summary>
    /// Estado actual (1=Pendiente, 2=Aceptada, 3=En Progreso, 4=Completada, 5=Cancelada, 6=Rechazada)
    /// </summary>
    [JsonPropertyName("estatus")]
    public int Estatus { get; set; }

    /// <summary>
    /// Nombre del estado en texto
    /// </summary>
    [JsonPropertyName("nombreEstado")]
    public string NombreEstado { get; set; } = string.Empty;

    /// <summary>
    /// Indica si ya fue calificada
    /// </summary>
    [JsonPropertyName("calificado")]
    public bool Calificado { get; set; }

    /// <summary>
    /// Porcentaje de avance (0-100)
    /// </summary>
    [JsonPropertyName("porcentajeAvance")]
    public int PorcentajeAvance { get; set; }

    /// <summary>
    /// Fecha real de inicio (si ya comenzó)
    /// </summary>
    [JsonPropertyName("fechaInicioReal")]
    public DateTime? FechaInicioReal { get; set; }

    /// <summary>
    /// Fecha real de finalización (si ya terminó)
    /// </summary>
    [JsonPropertyName("fechaFinalizacionReal")]
    public DateTime? FechaFinalizacionReal { get; set; }

    /// <summary>
    /// Identificación del contratista (cédula/RNC)
    /// </summary>
    [JsonPropertyName("contratistaIdentificacion")]
    public string? ContratistaIdentificacion { get; set; }

    /// <summary>
    /// Nombre completo del contratista
    /// </summary>
    [JsonPropertyName("contratistaCompleteName")]
    public string? ContratistaCompleteName { get; set; }

    /// <summary>
    /// URL de la foto del contratista
    /// </summary>
    [JsonPropertyName("contratistaFotoUrl")]
    public string? ContratistaFotoUrl { get; set; }
}
