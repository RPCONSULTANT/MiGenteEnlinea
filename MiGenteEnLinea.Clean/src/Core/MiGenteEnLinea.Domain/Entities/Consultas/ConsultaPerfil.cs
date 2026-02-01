using MiGenteEnLinea.Domain.Common;

namespace MiGenteEnLinea.Domain.Entities.Consultas;

/// <summary>
/// Entidad ConsultaPerfil - Registra las visitas a perfiles de contratistas por empleadores
/// 
/// CONTEXTO DE NEGOCIO:
/// - Cuando un empleador visita el perfil de un contratista, se registra la consulta
/// - Permite trackear cuántos perfiles ha consultado un empleador
/// - Permite a los contratistas ver cuántas visitas tiene su perfil
/// 
/// TABLA: ConsultasPerfil (nueva tabla - no existe en Legacy)
/// </summary>
public sealed class ConsultaPerfil : AuditableEntity
{
    /// <summary>
    /// Identificador único de la consulta
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// ID del usuario que consultó el perfil (empleador)
    /// FK a Credencial.UserId
    /// </summary>
    public string EmpleadorUserId { get; private set; } = string.Empty;

    /// <summary>
    /// Identificación (cédula) del contratista consultado
    /// </summary>
    public string ContratistaIdentificacion { get; private set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de la consulta
    /// </summary>
    public DateTime FechaConsulta { get; private set; }

    /// <summary>
    /// IP desde la cual se realizó la consulta (para auditoría)
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private ConsultaPerfil() { }

    /// <summary>
    /// Factory method para crear una nueva consulta de perfil
    /// </summary>
    public static ConsultaPerfil Crear(
        string empleadorUserId,
        string contratistaIdentificacion,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(empleadorUserId))
            throw new ArgumentException("El UserId del empleador es requerido", nameof(empleadorUserId));
        
        if (string.IsNullOrWhiteSpace(contratistaIdentificacion))
            throw new ArgumentException("La identificación del contratista es requerida", nameof(contratistaIdentificacion));

        return new ConsultaPerfil
        {
            EmpleadorUserId = empleadorUserId,
            ContratistaIdentificacion = contratistaIdentificacion,
            FechaConsulta = DateTime.UtcNow,
            IpAddress = ipAddress
        };
    }
}
