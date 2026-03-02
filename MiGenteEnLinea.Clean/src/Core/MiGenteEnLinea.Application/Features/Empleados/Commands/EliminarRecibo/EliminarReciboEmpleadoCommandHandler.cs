using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.EliminarRecibo;

public class EliminarReciboEmpleadoCommandHandler : IRequestHandler<EliminarReciboEmpleadoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EliminarReciboEmpleadoCommandHandler> _logger;

    public EliminarReciboEmpleadoCommandHandler(
        IApplicationDbContext context,
        ILogger<EliminarReciboEmpleadoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(EliminarReciboEmpleadoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Eliminando recibo de empleado: PagoId={PagoId}", request.PagoId);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.RecibosDetalle
            .Where(d => d.PagoId == request.PagoId)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.RecibosHeader
            .Where(h => h.PagoId == request.PagoId)
            .ExecuteDeleteAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Recibo de empleado eliminado (Header + Detalle): PagoId={PagoId}", request.PagoId);
        
        return true;
    }
}
