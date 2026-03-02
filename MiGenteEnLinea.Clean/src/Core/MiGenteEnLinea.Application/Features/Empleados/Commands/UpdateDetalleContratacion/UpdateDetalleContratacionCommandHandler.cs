using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateDetalleContratacion;

public class UpdateDetalleContratacionCommandHandler : IRequestHandler<UpdateDetalleContratacionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateDetalleContratacionCommandHandler> _logger;

    public UpdateDetalleContratacionCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateDetalleContratacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateDetalleContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating DetalleContratacion for ContratacionId: {ContratacionId}",
            request.ContratacionId);

        var detalle = await _context.DetalleContrataciones
            .FirstOrDefaultAsync(x => x.ContratacionId == request.ContratacionId, cancellationToken);

        if (detalle is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.DescripcionCorta) || request.DescripcionAmpliada is not null)
        {
            detalle.ActualizarDescripciones(request.DescripcionCorta, request.DescripcionAmpliada);
        }

        if (request.FechaInicio.HasValue || request.FechaFinal.HasValue)
        {
            detalle.ActualizarFechas(
                request.FechaInicio.HasValue ? DateOnly.FromDateTime(request.FechaInicio.Value) : null,
                request.FechaFinal.HasValue ? DateOnly.FromDateTime(request.FechaFinal.Value) : null);
        }

        if (request.MontoAcordado.HasValue)
        {
            detalle.ActualizarMonto(request.MontoAcordado.Value);
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (request.Estatus.HasValue || request.EsquemaPagos is not null)
        {
            await _context.DetalleContrataciones
                .Where(x => x.DetalleId == detalle.DetalleId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Estatus, x => request.Estatus ?? x.Estatus)
                    .SetProperty(x => x.EsquemaPagos, x => request.EsquemaPagos ?? x.EsquemaPagos), cancellationToken);
        }

        var result = true;

        _logger.LogInformation(
            "DetalleContratacion updated successfully for ContratacionId: {ContratacionId}",
            request.ContratacionId);

        return result;
    }
}
