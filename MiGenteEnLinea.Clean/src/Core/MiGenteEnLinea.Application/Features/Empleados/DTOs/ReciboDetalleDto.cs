using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para recibo de pago detallado con header y líneas de percepciones/deducciones.
/// Usado por GetReciboByIdQuery.
/// </summary>
public record ReciboDetalleDto
{
    // Header
    [JsonPropertyName("pagoId")]
    public int PagoId { get; init; }
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; init; }
    [JsonPropertyName("empleadoNombre")]
    public string EmpleadoNombre { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("fechaPago")]
    public DateTime? FechaPago { get; init; } // Nullable porque puede estar pendiente de pago
    [JsonPropertyName("fechaRegistro")]
    public DateTime FechaRegistro { get; init; }
    [JsonPropertyName("comentarios")]
    public string? Comentarios { get; init; }
    [JsonPropertyName("estado")]
    public int Estado { get; init; } // 1=Pendiente, 2=Pagado, 3=Anulado
    [JsonPropertyName("motivoAnulacion")]
    public string? MotivoAnulacion { get; init; }

    // Totales
    [JsonPropertyName("totalPercepciones")]
    public decimal TotalPercepciones { get; init; }
    [JsonPropertyName("totalDeducciones")]
    public decimal TotalDeducciones { get; init; }
    [JsonPropertyName("netoPagar")]
    public decimal NetoPagar { get; init; }

    // Detalles
    [JsonPropertyName("percepciones")]
    public List<ReciboLineaDto> Percepciones { get; init; } = new();
    [JsonPropertyName("deducciones")]
    public List<ReciboLineaDto> Deducciones { get; init; } = new();
}

/// <summary>
/// Línea individual de recibo (percepción o deducción).
/// </summary>
public record ReciboLineaDto
{
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; init; }
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; init; } = null!;
    [JsonPropertyName("monto")]
    public decimal Monto { get; init; }
}
