using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para remuneraciones adicionales de empleado
/// Migrado desde: Remuneraciones entity (tabla Legacy)
/// 
/// Representa ingresos adicionales al salario base como:
/// - Bonos
/// - Comisiones
/// - Horas extras
/// - Incentivos
/// </summary>
public class RemuneracionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
    [JsonPropertyName("monto")]
    public decimal Monto { get; set; }
}
