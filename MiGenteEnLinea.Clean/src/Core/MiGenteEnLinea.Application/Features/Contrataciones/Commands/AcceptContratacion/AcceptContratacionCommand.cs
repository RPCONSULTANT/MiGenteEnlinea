using MediatR;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.AcceptContratacion;

/// <summary>
/// Command para que un contratista acepte una propuesta de contratación.
/// 
/// CONTEXTO DE NEGOCIO:
/// - El contratista recibe una notificación de propuesta de trabajo
/// - Revisa los términos (descripción, fechas, monto, forma de pago)
/// - Si está de acuerdo, acepta la propuesta
/// - Estado cambia de: Pendiente → Aceptada
/// - El empleador recibe notificación de aceptación
/// 
/// FLUJO:
/// 1. Contratista accede a propuesta desde su dashboard
/// 2. Revisa detalles del trabajo
/// 3. Click en "Aceptar Propuesta"
/// 4. Sistema valida que esté en estado Pendiente
/// 5. Sistema cambia estado a Aceptada
/// 6. Domain Event ContratacionAceptadaEvent se dispara
/// 7. Sistema notifica al empleador
/// </summary>
public record AcceptContratacionCommand : IRequest<Unit>
{
    /// <summary>
    /// ID del detalle de contratación a aceptar
    /// </summary>
    public int DetalleId { get; init; }

    /// <summary>
    /// UserId del actor autenticado que ejecuta la transición.
    /// </summary>
    public string UserId { get; init; } = string.Empty;
}
