using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO for VPagosContrataciones view
/// Represents payment records for contractor services
/// </summary>
public class PagoContratacionDto
{
    [JsonPropertyName("pagoId")]
    public int PagoId { get; set; }
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("fechaPago")]
    public DateTime? FechaPago { get; set; }
    [JsonPropertyName("expr1")]
    public string? Expr1 { get; set; } // Expression from view
    [JsonPropertyName("monto")]
    public decimal? Monto { get; set; }
    [JsonPropertyName("contratacionId")]
    public int? ContratacionId { get; set; }
    [JsonPropertyName("detalleId")]
    public int? DetalleId { get; set; }
}
