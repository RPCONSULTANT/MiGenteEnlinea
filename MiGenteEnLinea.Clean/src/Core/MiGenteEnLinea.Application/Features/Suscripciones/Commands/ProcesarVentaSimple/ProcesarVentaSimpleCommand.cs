using MediatR;

namespace MiGenteEnLinea.Application.Features.Suscripciones.Commands.ProcesarVentaSimple;

/// <summary>
/// Procesa una venta en modo simple/fake sin datos de tarjeta.
/// </summary>
public record ProcesarVentaSimpleCommand : IRequest<int>
{
    public string UserId { get; init; } = string.Empty;
    public int PlanId { get; init; }
    public string? Motivo { get; init; }
}
