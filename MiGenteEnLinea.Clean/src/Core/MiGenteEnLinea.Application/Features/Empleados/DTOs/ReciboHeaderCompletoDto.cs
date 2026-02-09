using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO completo para Recibo con Header, Detalle y Empleado
/// Migrado de: Empleador_Recibos_Header con Include(Detalle).Include(Empleado)
/// </summary>
public class ReciboHeaderCompletoDto
{
    // Header fields (from Empleador_Recibos_Header)
    [JsonPropertyName("pagoId")]
    public int PagoId { get; set; }
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("empleadoId")]
    public int? EmpleadoId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("fechaPago")]
    public DateTime? FechaPago { get; set; }
    [JsonPropertyName("conceptoPago")]
    public string? ConceptoPago { get; set; }
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }
    
    // Nested relationships
    [JsonPropertyName("detalles")]
    public List<EmpleadorReciboDetalleDto> Detalles { get; set; } = new();
    [JsonPropertyName("empleado")]
    public EmpleadoBasicoDto? Empleado { get; set; }
}

/// <summary>
/// DTO para detalles de recibo (Empleador_Recibos_Detalle)
/// Renombrado de ReciboDetalleDto para evitar conflicto con existente
/// </summary>
public class EmpleadorReciboDetalleDto
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
/// DTO básico de empleado (solo campos esenciales)
/// </summary>
public class EmpleadoBasicoDto
{
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }
    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }
}
