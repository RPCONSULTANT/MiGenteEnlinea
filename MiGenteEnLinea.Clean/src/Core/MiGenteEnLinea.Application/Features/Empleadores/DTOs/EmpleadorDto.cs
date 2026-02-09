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
