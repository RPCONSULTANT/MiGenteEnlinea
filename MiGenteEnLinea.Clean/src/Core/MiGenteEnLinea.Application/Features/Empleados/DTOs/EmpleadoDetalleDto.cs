using System;
using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO con información detallada de un empleado permanente.
/// Incluye campos calculados y toda la información necesaria para la vista de detalle.
/// </summary>
public class EmpleadoDetalleDto
{
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("identificacion")]
    public string Identificacion { get; set; } = string.Empty;
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;
    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
    
    // Información Personal
    [JsonPropertyName("estadoCivil")]
    public int? EstadoCivil { get; set; }
    [JsonPropertyName("nacimiento")]
    public DateOnly? Nacimiento { get; set; }
    [JsonPropertyName("edad")]
    public int? Edad
    {
        get
        {
            if (!Nacimiento.HasValue)
                return null;

            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - Nacimiento.Value.Year;
            if (Nacimiento.Value > today.AddYears(-age))
                age--;
            return age;
        }
    }
    
    // Contacto
    [JsonPropertyName("telefono1")]
    public string? Telefono1 { get; set; }
    [JsonPropertyName("telefono2")]
    public string? Telefono2 { get; set; }
    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }
    [JsonPropertyName("provincia")]
    public string? Provincia { get; set; }
    [JsonPropertyName("municipio")]
    public string? Municipio { get; set; }
    
    // Información Laboral
    [JsonPropertyName("fechaRegistro")]
    public DateTime FechaRegistro { get; set; }
    [JsonPropertyName("fechaInicio")]
    public DateOnly? FechaInicio { get; set; }
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
        _ => "No especificado"
    };
    
    [JsonPropertyName("diasPago")]
    public int? DiasPago { get; set; }
    [JsonPropertyName("inscritoTss")]
    public bool InscritoTss { get; set; }
    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
    
    // Baja (si aplica)
    [JsonPropertyName("fechaSalida")]
    public DateTime? FechaSalida { get; set; }
    [JsonPropertyName("motivoBaja")]
    public string? MotivoBaja { get; set; }
    [JsonPropertyName("prestaciones")]
    public decimal? Prestaciones { get; set; }
    
    // Emergencia
    [JsonPropertyName("contactoEmergencia")]
    public string? ContactoEmergencia { get; set; }
    [JsonPropertyName("telefonoEmergencia")]
    public string? TelefonoEmergencia { get; set; }
    
    // Foto
    [JsonPropertyName("foto")]
    public string? Foto { get; set; }
    
    // Campos Calculados
    [JsonPropertyName("salarioMensual")]
    public decimal SalarioMensual { get; set; }
    [JsonPropertyName("antiguedad")]
    public int Antiguedad { get; set; }
    [JsonPropertyName("requiereActualizacionFoto")]
    public bool RequiereActualizacionFoto { get; set; }
    
    // Remuneraciones Extras
    [JsonPropertyName("descripcionExtra1")]
    public string? DescripcionExtra1 { get; set; }
    [JsonPropertyName("montoExtra1")]
    public decimal? MontoExtra1 { get; set; }
    [JsonPropertyName("descripcionExtra2")]
    public string? DescripcionExtra2 { get; set; }
    [JsonPropertyName("montoExtra2")]
    public decimal? MontoExtra2 { get; set; }
    [JsonPropertyName("descripcionExtra3")]
    public string? DescripcionExtra3 { get; set; }
    [JsonPropertyName("montoExtra3")]
    public decimal? MontoExtra3 { get; set; }
    
    // Auditoría
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
