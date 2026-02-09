using System;
using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO resumido para listado de empleados.
/// Optimizado para grids y listas con paginación.
/// </summary>
public class EmpleadoListDto
{
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
    [JsonPropertyName("identificacion")]
    public string Identificacion { get; set; } = string.Empty;
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;
    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
    [JsonPropertyName("posicion")]
    public string? Posicion { get; set; }
    [JsonPropertyName("salario")]
    public decimal Salario { get; set; }
    [JsonPropertyName("periodoPago")]
    public int PeriodoPago { get; set; }
    [JsonPropertyName("periodoPagoDescripcion")]
    public string PeriodoPagoDescripcion => PeriodoPago switch
    {
        1 => "Semanal",
        2 => "Quincenal",
        3 => "Mensual",
        _ => "N/A"
    };
    [JsonPropertyName("diasPago")]
    public int? DiasPago { get; set; }
    [JsonPropertyName("fechaInicio")]
    public DateOnly? FechaInicio { get; set; }
    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
    [JsonPropertyName("foto")]
    public string? Foto { get; set; }
    
    // Solo para inactivos
    [JsonPropertyName("fechaSalida")]
    public DateTime? FechaSalida { get; set; }
}
