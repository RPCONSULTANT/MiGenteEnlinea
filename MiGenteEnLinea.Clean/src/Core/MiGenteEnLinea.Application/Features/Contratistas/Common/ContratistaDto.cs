namespace MiGenteEnLinea.Application.Features.Contratistas.Common;

/// <summary>
/// DTO: Contratista - Data Transfer Object para operaciones de lectura
/// </summary>
/// <remarks>
/// Representa la información pública de un contratista
/// Incluye campos calculados para facilitar el frontend
/// </remarks>
public record ContratistaDto
{
    /// <summary>
    /// ID único del contratista
    /// </summary>
    public int ContratistaId { get; init; }

    /// <summary>
    /// ID del usuario asociado (FK a Credenciales)
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de registro en la plataforma
    /// </summary>
    public DateTime? FechaIngreso { get; init; }

    /// <summary>
    /// Título profesional o descripción corta
    /// Ejemplo: "Plomero certificado con 10 años de experiencia"
    /// </summary>
    public string? Titulo { get; init; }

    /// <summary>
    /// Tipo: 1=Persona Física, 2=Empresa
    /// </summary>
    public int Tipo { get; init; }

    /// <summary>
    /// Cédula o RNC
    /// </summary>
    public string? Identificacion { get; init; }

    /// <summary>
    /// Nombre del contratista
    /// </summary>
    public string? Nombre { get; init; }

    /// <summary>
    /// Apellido del contratista
    /// </summary>
    public string? Apellido { get; init; }

    /// <summary>
    /// Nombre completo (calculado en Query)
    /// Formato: "{Nombre} {Apellido}"
    /// </summary>
    public string? NombreCompleto { get; init; }

    /// <summary>
    /// Sector económico principal
    /// Ejemplo: "Construcción", "Reparaciones"
    /// </summary>
    public string? Sector { get; init; }

    /// <summary>
    /// Años de experiencia
    /// </summary>
    public int? Experiencia { get; init; }

    /// <summary>
    /// Presentación o biografía del contratista
    /// </summary>
    public string? Presentacion { get; init; }

    /// <summary>
    /// Teléfono principal
    /// </summary>
    public string? Telefono1 { get; init; }

    /// <summary>
    /// ¿Teléfono1 es WhatsApp?
    /// </summary>
    public bool Whatsapp1 { get; init; }

    /// <summary>
    /// Teléfono secundario
    /// </summary>
    public string? Telefono2 { get; init; }

    /// <summary>
    /// ¿Teléfono2 es WhatsApp?
    /// </summary>
    public bool Whatsapp2 { get; init; }

    /// <summary>
    /// Email de contacto
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// ¿Perfil activo y visible?
    /// </summary>
    public bool Activo { get; init; }

    /// <summary>
    /// Provincia donde opera
    /// </summary>
    public string? Provincia { get; init; }

    /// <summary>
    /// ¿Trabaja a nivel nacional?
    /// </summary>
    public bool NivelNacional { get; init; }

    /// <summary>
    /// URL de la imagen de perfil
    /// </summary>
    public string? ImagenUrl { get; init; }

    /// <summary>
    /// ¿Tiene imagen de perfil? (calculado)
    /// </summary>
    public bool TieneImagen => !string.IsNullOrWhiteSpace(ImagenUrl);

    /// <summary>
    /// ¿Tiene WhatsApp disponible? (calculado en Query)
    /// </summary>
    public bool TieneWhatsApp { get; init; }

    /// <summary>
    /// ¿Perfil completo? (calculado en Query)
    /// </summary>
    public bool PerfilCompleto { get; init; }

    /// <summary>
    /// ¿Puede recibir trabajos? (calculado en Query)
    /// </summary>
    public bool PuedeRecibirTrabajos { get; init; }

    /// <summary>
    /// Calificacion promedio (calculado en Query)
    /// </summary>
    public decimal? PromedioCalificacion { get; init; }
}
