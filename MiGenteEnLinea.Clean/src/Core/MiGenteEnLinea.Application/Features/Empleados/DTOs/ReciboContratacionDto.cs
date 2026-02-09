using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para recibo de contratación con su detalle y empleado temporal.
/// Representa Empleador_Recibos_Header_Contrataciones + Detalle + EmpleadosTemporales
/// </summary>
public class ReciboContratacionDto
{
    // Header fields
    [JsonPropertyName("pagoId")]
    public int PagoId { get; set; }
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("contratacionId")]
    public int? ContratacionId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("fechaPago")]
    public DateTime? FechaPago { get; set; }
    [JsonPropertyName("conceptoPago")]
    public string? ConceptoPago { get; set; }
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }

    // Detalle
    [JsonPropertyName("detalles")]
    public List<ReciboContratacionDetalleDto> Detalles { get; set; } = new();

    // EmpleadoTemporal (from Include)
    [JsonPropertyName("empleadoTemporal")]
    public EmpleadoTemporalSimpleDto? EmpleadoTemporal { get; set; }

    // Calculated total from detalles
    [JsonPropertyName("total")]
    public decimal Total => Detalles.Sum(d => d.Monto ?? 0);
}

/// <summary>
/// DTO para el detalle del recibo de contratación
/// </summary>
public class ReciboContratacionDetalleDto
{
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; set; }
    [JsonPropertyName("pagoId")]
    public int? PagoId { get; set; }
    [JsonPropertyName("concepto")]
    public string? Concepto { get; set; }
    [JsonPropertyName("monto")]
    public decimal? Monto { get; set; }
}

/// <summary>
/// DTO simplificado de empleado temporal para incluir en el recibo
/// </summary>
public class EmpleadoTemporalSimpleDto
{
    [JsonPropertyName("contratacionId")]
    public int ContratacionId { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }
    [JsonPropertyName("cedula")]
    public string? Cedula { get; set; }
    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}
