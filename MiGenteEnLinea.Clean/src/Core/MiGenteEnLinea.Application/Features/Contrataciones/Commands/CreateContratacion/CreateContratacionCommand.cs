using MediatR;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.CreateContratacion;

/// <summary>
/// Command para crear una nueva propuesta de contratación.
/// 
/// CONTEXTO DE NEGOCIO:
/// - Un empleador crea una propuesta para contratar a un contratista para un servicio específico
/// - La contratación inicia en estado "Pendiente" esperando aceptación del contratista
/// - Se debe especificar descripción del trabajo, fechas, monto acordado y forma de pago
/// 
/// FLUJO:
/// 1. Empleador selecciona contratista desde directorio
/// 2. Llena formulario con detalles del trabajo
/// 3. Sistema crea DetalleContratacion en estado Pendiente
/// 4. Sistema notifica al contratista (email/notificación)
/// 5. Contratista puede ver propuesta y decidir aceptar/rechazar
/// </summary>
public record CreateContratacionCommand : IRequest<int>
{
    /// <summary>
    /// UserId del empleador autenticado (inyectado por API desde JWT).
    /// No debe venir confiado desde el cliente.
    /// </summary>
    public string EmpleadorUserId { get; init; } = string.Empty;

    /// <summary>
    /// ID de la contratación padre (EmpleadoTemporal).
    /// Opcional: puede ser null para contrataciones independientes.
    /// </summary>
    public int? ContratacionId { get; init; }

    /// <summary>
    /// ID del contratista seleccionado por el empleador.
    /// Se usa para validar vínculo y trazabilidad.
    /// </summary>
    public int ContratistaId { get; init; }

    /// <summary>
    /// ID del servicio seleccionado para la contratación.
    /// Opcional cuando no aplica catálogo.
    /// </summary>
    public int? ServicioId { get; init; }

    /// <summary>
    /// Descripción breve del trabajo a realizar (máx 60 caracteres).
    /// Ejemplo: "Reparación de plomería - Baño principal"
    /// </summary>
    public string DescripcionCorta { get; init; } = string.Empty;

    /// <summary>
    /// Descripción detallada del trabajo, alcance, materiales, etc. (máx 250 caracteres).
    /// Opcional.
    /// Ejemplo: "Reparar tubería rota en baño principal, reemplazar llave mezcladora..."
    /// </summary>
    public string? DescripcionAmpliada { get; init; }

    /// <summary>
    /// Fecha de inicio acordada para el trabajo.
    /// Debe ser una fecha futura o actual.
    /// </summary>
    public DateOnly FechaInicio { get; init; }

    /// <summary>
    /// Fecha estimada de finalización.
    /// Debe ser posterior a FechaInicio.
    /// </summary>
    public DateOnly FechaFinal { get; init; }

    /// <summary>
    /// Monto total acordado para el trabajo en pesos dominicanos.
    /// Debe ser mayor a 0.
    /// </summary>
    public decimal MontoAcordado { get; init; }

    /// <summary>
    /// Esquema de pagos acordado (máx 50 caracteres).
    /// Opcional.
    /// Ejemplos: "50% adelanto, 50% al finalizar", "Pago único al completar", "Semanal"
    /// </summary>
    public string? EsquemaPagos { get; init; }

    /// <summary>
    /// Notas adicionales sobre la contratación (máx 500 caracteres).
    /// Opcional.
    /// </summary>
    public string? Notas { get; init; }
}
