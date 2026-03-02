using MiGenteEnLinea.Domain.Common;

namespace MiGenteEnLinea.Domain.Entities.Contactos;

/// <summary>
/// Solicitud de contacto desde un contratista hacia un empleador.
/// </summary>
public sealed class ContactoSolicitud : AuditableEntity
{
    public int SolicitudId { get; private set; }
    public string ContratistaUserId { get; private set; } = string.Empty;
    public int EmpleadorId { get; private set; }
    public string? Mensaje { get; private set; }
    public string? CanalPreferido { get; private set; }
    public string Estatus { get; private set; } = "Pendiente";

    private ContactoSolicitud() { }

    public static ContactoSolicitud Crear(
        string contratistaUserId,
        int empleadorId,
        string? mensaje,
        string? canalPreferido)
    {
        if (string.IsNullOrWhiteSpace(contratistaUserId))
        {
            throw new ArgumentException("El usuario contratista es requerido.", nameof(contratistaUserId));
        }

        if (empleadorId <= 0)
        {
            throw new ArgumentException("El empleadorId es requerido.", nameof(empleadorId));
        }

        return new ContactoSolicitud
        {
            ContratistaUserId = contratistaUserId.Trim(),
            EmpleadorId = empleadorId,
            Mensaje = string.IsNullOrWhiteSpace(mensaje) ? null : mensaje.Trim(),
            CanalPreferido = string.IsNullOrWhiteSpace(canalPreferido) ? null : canalPreferido.Trim().ToLowerInvariant(),
            Estatus = "Pendiente",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
