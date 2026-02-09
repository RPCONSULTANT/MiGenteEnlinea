using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleadores.DTOs;

/// <summary>
/// DTO: Representa un Empleador completo
/// </summary>
/// <remarks>
/// Se usa para:
/// - Respuesta de queries (GetEmpleadorByUserId, GetEmpleadorById, SearchEmpleadores)
/// - NO incluye byte[] Foto (muy grande), se obtiene en endpoint separado
/// </remarks>
public sealed class EmpleadorDto
{
    /// <summary>
    /// ID del empleador (ofertanteID en legacy)
    /// </summary>
    [JsonPropertyName("empleadorId")]
    public int EmpleadorId { get; init; }

    /// <summary>
    /// ID del usuario (FK a Credencial.UserId)
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de publicación/creación del perfil
    /// </summary>
    [JsonPropertyName("fechaPublicacion")]
    public DateTime? FechaPublicacion { get; init; }

    /// <summary>
    /// Habilidades de la empresa empleadora (max 200 caracteres)
    /// </summary>
    [JsonPropertyName("habilidades")]
    public string? Habilidades { get; init; }

    /// <summary>
    /// Experiencia o trayectoria de la empresa (max 200 caracteres)
    /// </summary>
    [JsonPropertyName("experiencia")]
    public string? Experiencia { get; init; }

    /// <summary>
    /// Descripción general del empleador/empresa (max 500 caracteres)
    /// </summary>
    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; init; }

    /// <summary>
    /// Nombre del empleador (perfil)
    /// </summary>
    [JsonPropertyName("nombre")]
    public string? Nombre { get; init; }

    /// <summary>
    /// Apellido del empleador (perfil)
    /// </summary>
    [JsonPropertyName("apellido")]
    public string? Apellido { get; init; }

    /// <summary>
    /// Nombre completo calculado (perfil)
    /// </summary>
    [JsonPropertyName("nombreCompleto")]
    public string? NombreCompleto { get; init; }

    /// <summary>
    /// Nombre comercial (si aplica)
    /// </summary>
    [JsonPropertyName("nombreComercial")]
    public string? NombreComercial { get; init; }

    /// <summary>
    /// Identificacion (cedula o RNC)
    /// </summary>
    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; init; }

    /// <summary>
    /// RNC (cuando aplica)
    /// </summary>
    [JsonPropertyName("rnc")]
    public string? Rnc { get; init; }

    /// <summary>
    /// Cedula (cuando aplica)
    /// </summary>
    [JsonPropertyName("cedula")]
    public string? Cedula { get; init; }

    /// <summary>
    /// Email de contacto (perfil)
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Telefono principal (perfil)
    /// </summary>
    [JsonPropertyName("telefono1")]
    public string? Telefono1 { get; init; }

    /// <summary>
    /// Telefono secundario (perfil)
    /// </summary>
    [JsonPropertyName("telefono2")]
    public string? Telefono2 { get; init; }

    /// <summary>
    /// WhatsApp asociado al telefono1 (si aplica)
    /// </summary>
    [JsonPropertyName("whatsapp1")]
    public bool Whatsapp1 { get; init; }

    /// <summary>
    /// WhatsApp asociado al telefono2 (si aplica)
    /// </summary>
    [JsonPropertyName("whatsapp2")]
    public bool Whatsapp2 { get; init; }

    /// <summary>
    /// Direccion (perfil extendido)
    /// </summary>
    [JsonPropertyName("direccion")]
    public string? Direccion { get; init; }

    /// <summary>
    /// Provincia (si aplica)
    /// </summary>
    [JsonPropertyName("provincia")]
    public string? Provincia { get; init; }

    /// <summary>
    /// Ciudad (si aplica)
    /// </summary>
    [JsonPropertyName("ciudad")]
    public string? Ciudad { get; init; }

    /// <summary>
    /// Sector economico (si aplica)
    /// </summary>
    [JsonPropertyName("sector")]
    public string? Sector { get; init; }

    /// <summary>
    /// Calificacion promedio (si aplica)
    /// </summary>
    [JsonPropertyName("promedioCalificaciones")]
    public decimal? PromedioCalificaciones { get; init; }

    /// <summary>
    /// Total de contrataciones (si aplica)
    /// </summary>
    [JsonPropertyName("totalContrataciones")]
    public int TotalContrataciones { get; init; }

    /// <summary>
    /// Fecha de ingreso al sistema (perfil)
    /// </summary>
    [JsonPropertyName("fechaIngreso")]
    public DateTime? FechaIngreso { get; init; }

    /// <summary>
    /// Indica si el perfil esta activo
    /// </summary>
    [JsonPropertyName("activo")]
    public bool Activo { get; init; }

    /// <summary>
    /// Indica si el empleador tiene foto/logo cargado
    /// </summary>
    [JsonPropertyName("tieneFoto")]
    public bool TieneFoto { get; init; }

    /// <summary>
    /// Fecha de creación (auditoría)
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Fecha de última modificación (auditoría)
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; init; }
}
