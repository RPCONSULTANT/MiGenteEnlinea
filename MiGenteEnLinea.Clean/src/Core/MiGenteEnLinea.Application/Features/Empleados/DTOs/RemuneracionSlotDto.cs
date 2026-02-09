using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para remuneraciones extras almacenadas en slots del Empleado (MontoExtra1/2/3)
/// NO confundir con RemuneracionDto (tabla Remuneraciones separada)
/// 
/// Representa uno de los 3 slots fijos de remuneraciones disponibles en el entity Empleado:
/// - RemuneracionExtra1 + MontoExtra1
/// - RemuneracionExtra2 + MontoExtra2
/// - RemuneracionExtra3 + MontoExtra3
/// </summary>
public class RemuneracionSlotDto
{
    /// <summary>
    /// Número del slot de remuneración (1, 2 o 3).
    /// </summary>
    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    /// <summary>
    /// Descripción de la remuneración (ej: "Bono productividad", "Comisión ventas").
    /// Puede ser null si el slot está vacío.
    /// </summary>
    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Monto de la remuneración extra.
    /// Puede ser null si el slot está vacío.
    /// </summary>
    [JsonPropertyName("monto")]
    public decimal? Monto { get; set; }

    /// <summary>
    /// Indica si el slot tiene una remuneración activa.
    /// </summary>
    [JsonPropertyName("tieneValor")]
    public bool TieneValor => !string.IsNullOrWhiteSpace(Descripcion) && Monto.HasValue;
}
