using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para recibos de empleados desde la vista VRecibosEmpleados.
/// </summary>
public class ReciboEmpleadoDto
{
    [JsonPropertyName("total")]
    public decimal? Total { get; set; }
    [JsonPropertyName("pagoId")]
    public int PagoId { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("empleadoId")]
    public int? EmpleadoId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("fechaPago")]
    public DateTime? FechaPago { get; set; }
    [JsonPropertyName("conceptoPago")]
    public string ConceptoPago { get; set; } = string.Empty;
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;
}
