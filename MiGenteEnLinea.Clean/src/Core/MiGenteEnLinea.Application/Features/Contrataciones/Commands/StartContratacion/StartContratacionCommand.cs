using MediatR;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.StartContratacion;

/// <summary>
/// Command para iniciar el trabajo de una contratación aceptada.
/// 
/// CONTEXTO:
/// - Estado cambia de: Aceptada → En Progreso
/// - Se registra la fecha real de inicio
/// - Porcentaje de avance se inicializa en 0%
/// </summary>
public record StartContratacionCommand : IRequest<Unit>
{
    public int DetalleId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
