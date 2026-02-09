using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// DTO for VistaContratacionTemporal view
/// Migrado de: VContratacionesTemporales (Legacy)
/// </summary>
public class VistaContratacionTemporalDto
{
    // Contratación
    [JsonPropertyName("contratacionId")]
    public int ContratacionId { get; set; }
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    [JsonPropertyName("fechaRegistro")]
    public DateTime? FechaRegistro { get; set; }
    [JsonPropertyName("tipo")]
    public int? Tipo { get; set; }
    
    // Información del Contratista
    [JsonPropertyName("nombreComercial")]
    public string? NombreComercial { get; set; }
    [JsonPropertyName("rnc")]
    public string? Rnc { get; set; }
    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
    
    // Ubicación
    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }
    [JsonPropertyName("provincia")]
    public string? Provincia { get; set; }
    [JsonPropertyName("municipio")]
    public string? Municipio { get; set; }
    [JsonPropertyName("telefono1")]
    public string? Telefono1 { get; set; }
    [JsonPropertyName("telefono2")]
    public string? Telefono2 { get; set; }
    
    // Detalle de Contratación
    [JsonPropertyName("detalleId")]
    public int DetalleId { get; set; }
    [JsonPropertyName("expr1")]
    public int? Expr1 { get; set; }
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
    
    // Composiciones
    [JsonPropertyName("composicionNombre")]
    public string? ComposicionNombre { get; set; }
    [JsonPropertyName("composicionId")]
    public string? ComposicionId { get; set; }
    
    // Calificaciones
    [JsonPropertyName("conocimientos")]
    public int? Conocimientos { get; set; }
    [JsonPropertyName("puntualidad")]
    public int? Puntualidad { get; set; }
    [JsonPropertyName("recomendacion")]
    public int? Recomendacion { get; set; }
    [JsonPropertyName("cumplimiento")]
    public int? Cumplimiento { get; set; }
}
