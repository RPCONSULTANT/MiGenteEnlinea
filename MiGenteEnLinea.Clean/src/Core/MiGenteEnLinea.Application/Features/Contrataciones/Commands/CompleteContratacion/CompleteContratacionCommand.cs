using MediatR;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.CompleteContratacion;

/// <summary>
/// Command para marcar una contratación como completada.
/// 
/// CONTEXTO:
/// - Estado cambia de: En Progreso → Completada
/// - Se registra la fecha real de finalización
/// - Porcentaje de avance se establece en 100%
/// - Después de completar, el empleador puede calificar al contratista
/// </summary>
public record CompleteContratacionCommand : IRequest<Unit>
{
    public int DetalleId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
