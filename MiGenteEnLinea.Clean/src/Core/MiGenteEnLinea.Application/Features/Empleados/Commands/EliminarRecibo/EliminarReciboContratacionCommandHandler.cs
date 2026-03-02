using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.EliminarRecibo;

public class EliminarReciboContratacionCommandHandler : IRequestHandler<EliminarReciboContratacionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EliminarReciboContratacionCommandHandler> _logger;

    public EliminarReciboContratacionCommandHandler(
        IApplicationDbContext context,
        ILogger<EliminarReciboContratacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(EliminarReciboContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Eliminando recibo de contratación: PagoId={PagoId}", request.PagoId);

        var executionStrategy = _context.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.EmpleadorRecibosDetalleContrataciones
                .Where(d => d.PagoId == request.PagoId)
                .ExecuteDeleteAsync(cancellationToken);

            await _context.EmpleadorRecibosHeaderContrataciones
                .Where(h => h.PagoId == request.PagoId)
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });

        _logger.LogInformation("Recibo de contratación eliminado (Header + Detalle): PagoId={PagoId}", request.PagoId);
        
        return true;
    }
}
