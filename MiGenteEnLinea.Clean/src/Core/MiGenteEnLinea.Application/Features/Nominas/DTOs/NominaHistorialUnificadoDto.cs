using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Nominas.DTOs;

public class NominaHistorialUnificadoDto
{
    [JsonPropertyName("pagoId")]
    public int PagoId { get; set; }

    [JsonPropertyName("fechaPago")]
    public DateTime FechaPago { get; set; }

    [JsonPropertyName("beneficiario")]
    public string Beneficiario { get; set; } = string.Empty;

    [JsonPropertyName("concepto")]
    public string Concepto { get; set; } = string.Empty;

    [JsonPropertyName("totalBruto")]
    public decimal TotalBruto { get; set; }

    [JsonPropertyName("totalDeducciones")]
    public decimal TotalDeducciones { get; set; }

    [JsonPropertyName("totalNeto")]
    public decimal TotalNeto { get; set; }

    [JsonPropertyName("tipoRegistro")]
    public string TipoRegistro { get; set; } = "Fijo";

    [JsonPropertyName("referenciaId")]
    public int ReferenciaId { get; set; }

    [JsonPropertyName("estado")]
    public int Estado { get; set; }
}
