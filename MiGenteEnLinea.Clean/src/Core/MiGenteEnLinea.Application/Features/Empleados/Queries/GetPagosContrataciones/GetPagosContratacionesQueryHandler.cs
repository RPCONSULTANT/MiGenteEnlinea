using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetPagosContrataciones;

public class GetPagosContratacionesQueryHandler : IRequestHandler<GetPagosContratacionesQuery, List<PagoContratacionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetPagosContratacionesQueryHandler> _logger;

    public GetPagosContratacionesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetPagosContratacionesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PagoContratacionDto>> Handle(GetPagosContratacionesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting pagos contrataciones for ContratacionId: {ContratacionId}, DetalleId: {DetalleId}",
            request.ContratacionId,
            request.DetalleId);

        var result = await _context.Set<Domain.ReadModels.VistaPagoContratacion>()
            .AsNoTracking()
            .Where(x => x.ContratacionId == request.ContratacionId && x.DetalleId == request.DetalleId)
            .Select(x => new PagoContratacionDto
            {
                PagoId = x.PagoId,
                UserId = x.UserId,
                FechaRegistro = x.FechaRegistro,
                FechaPago = x.FechaPago,
                Expr1 = x.Expr1,
                Monto = x.Monto,
                ContratacionId = x.ContratacionId,
                DetalleId = x.DetalleId
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Found {Count} pagos contrataciones for ContratacionId: {ContratacionId}, DetalleId: {DetalleId}",
            result.Count,
            request.ContratacionId,
            request.DetalleId);

        return result;
    }
}
