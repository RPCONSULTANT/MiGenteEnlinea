using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.EliminarEmpleadoTemporal;

/// <summary>
/// Handler para eliminar un empleado temporal con todos sus recibos asociados.
/// Migrado desde: EmpleadosService.eliminarEmpleadoTemporal(int contratacionID) - line 298
/// </summary>
public class EliminarEmpleadoTemporalCommandHandler 
    : IRequestHandler<EliminarEmpleadoTemporalCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EliminarEmpleadoTemporalCommandHandler> _logger;

    public EliminarEmpleadoTemporalCommandHandler(
        IApplicationDbContext context,
        ILogger<EliminarEmpleadoTemporalCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(
        EliminarEmpleadoTemporalCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Eliminando empleado temporal y sus recibos: ContratacionId={ContratacionId}",
            request.ContratacionId);

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        var pagoIds = await _context.EmpleadorRecibosHeaderContrataciones
            .Where(r => r.ContratacionId == request.ContratacionId)
            .Select(r => r.PagoId)
            .ToListAsync(cancellationToken);

        if (pagoIds.Count > 0)
        {
            await _context.EmpleadorRecibosDetalleContrataciones
                .Where(d => d.PagoId.HasValue && pagoIds.Contains(d.PagoId.Value))
                .ExecuteDeleteAsync(cancellationToken);

            await _context.EmpleadorRecibosHeaderContrataciones
                .Where(h => pagoIds.Contains(h.PagoId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        await _context.DetalleContrataciones
            .Where(d => d.ContratacionId == request.ContratacionId)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.Set<Domain.Entities.Empleados.EmpleadoTemporal>()
            .Where(e => e.ContratacionId == request.ContratacionId)
            .ExecuteDeleteAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        var result = true;

        _logger.LogInformation(
            "Empleado temporal eliminado (recibos + empleado): ContratacionId={ContratacionId}",
            request.ContratacionId);

        return result;
    }
}
