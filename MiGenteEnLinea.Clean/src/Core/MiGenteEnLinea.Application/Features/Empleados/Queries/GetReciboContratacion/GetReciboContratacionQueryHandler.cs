using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetReciboContratacion;

/// <summary>
/// Handler para obtener un recibo de contratación con su detalle y empleado temporal.
/// Migrado desde: EmpleadosService.GetContratacion_ReciboByPagoID(int pagoID) - line 222
/// </summary>
public class GetReciboContratacionQueryHandler 
    : IRequestHandler<GetReciboContratacionQuery, ReciboContratacionDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetReciboContratacionQueryHandler> _logger;

    public GetReciboContratacionQueryHandler(
        IApplicationDbContext context,
        ILogger<GetReciboContratacionQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReciboContratacionDto?> Handle(
        GetReciboContratacionQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo recibo de contratación: PagoId={PagoId}",
            request.PagoId);

        var recibo = await _context.EmpleadorRecibosHeaderContrataciones
            .AsNoTracking()
            .Where(x => x.PagoId == request.PagoId)
            .Select(h => new ReciboContratacionDto
            {
                PagoId = h.PagoId,
                UserId = h.UserId,
                ContratacionId = h.ContratacionId,
                FechaRegistro = h.FechaRegistro,
                FechaPago = h.FechaPago,
                ConceptoPago = h.ConceptoPago,
                Tipo = h.Tipo,
                Detalles = _context.EmpleadorRecibosDetalleContrataciones
                    .Where(d => d.PagoId == h.PagoId)
                    .Select(d => new ReciboContratacionDetalleDto
                    {
                        DetalleId = d.DetalleId,
                        PagoId = d.PagoId,
                        Concepto = d.Concepto,
                        Monto = d.Monto
                    })
                    .ToList(),
                EmpleadoTemporal = h.ContratacionId.HasValue
                    ? _context.Set<Domain.Entities.Empleados.EmpleadoTemporal>()
                        .Where(e => e.ContratacionId == h.ContratacionId.Value)
                        .Select(e => new EmpleadoTemporalSimpleDto
                        {
                            ContratacionId = e.ContratacionId,
                            Nombre = e.Nombre,
                            Apellido = e.Apellido,
                            Cedula = e.Identificacion
                        })
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (recibo == null)
        {
            _logger.LogWarning(
                "Recibo de contratación no encontrado: PagoId={PagoId}",
                request.PagoId);
            return null;
        }

        _logger.LogInformation(
            "Recibo de contratación obtenido: PagoId={PagoId}, Detalles={DetalleCount}, Total={Total}",
            recibo.PagoId,
            recibo.Detalles.Count,
            recibo.Total);

        return recibo;
    }
}
