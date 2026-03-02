using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CancelarTrabajo;

public class CancelarTrabajoCommandHandler : IRequestHandler<CancelarTrabajoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CancelarTrabajoCommandHandler> _logger;

    public CancelarTrabajoCommandHandler(
        IApplicationDbContext context,
        ILogger<CancelarTrabajoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelarTrabajoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Cancelando trabajo temporal: ContratacionId={ContratacionId}, DetalleId={DetalleId}",
            request.ContratacionId,
            request.DetalleId);

        await _context.DetalleContrataciones
            .Where(d => d.ContratacionId == request.ContratacionId && d.DetalleId == request.DetalleId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(d => d.Estatus, 3), cancellationToken);

        _logger.LogInformation(
            "Trabajo temporal cancelado (estatus=3): ContratacionId={ContratacionId}, DetalleId={DetalleId}",
            request.ContratacionId,
            request.DetalleId);
        
        return true;
    }
}
