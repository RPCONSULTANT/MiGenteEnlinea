using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO para EmpleadosTemporales con DetalleContrataciones incluido
/// </summary>
public class EmpleadoTemporalDto
{
    [JsonPropertyName("contratacionId")]
    public int ContratacionId { get; set; }
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }
    [JsonPropertyName("nombreComercial")]
    public string? NombreComercial { get; set; }
    [JsonPropertyName("rnc")]
    public string? Rnc { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }
    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }
    [JsonPropertyName("telefono1")]
    public string? Telefono1 { get; set; }
    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }

    // Nested DetalleContrataciones
    [JsonPropertyName("detalle")]
    public DetalleContratacionDto? Detalle { get; set; }
}

public class DetalleContratacionDto
{
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; set; }
    [JsonPropertyName("contratacionId")]
    public int? ContratacionId { get; set; }
    [JsonPropertyName("descripcionCorta")]
    public string? DescripcionCorta { get; set; }
    [JsonPropertyName("descripcionAmpliada")]
    public string? DescripcionAmpliada { get; set; }
    [JsonPropertyName("fechaInicio")]
    public DateOnly? FechaInicio { get; set; }
    [JsonPropertyName("fechaFinal")]
    public DateOnly? FechaFinal { get; set; }
    [JsonPropertyName("montoAcordado")]
    public decimal? MontoAcordado { get; set; }
    [JsonPropertyName("esquemaPagos")]
    public string? EsquemaPagos { get; set; }
    [JsonPropertyName("estatus")]
    public int? Estatus { get; set; }
    [JsonPropertyName("calificado")]
    public bool? Calificado { get; set; }
    [JsonPropertyName("calificacionId")]
    public int? CalificacionId { get; set; }
}
