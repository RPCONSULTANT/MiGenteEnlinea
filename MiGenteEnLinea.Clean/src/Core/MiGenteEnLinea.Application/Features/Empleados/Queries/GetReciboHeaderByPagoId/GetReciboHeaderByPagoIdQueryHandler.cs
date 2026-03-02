using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetReciboHeaderByPagoId;

/// <summary>
/// Handler para GetReciboHeaderByPagoIdQuery
/// Obtiene recibo header con detalle y empleado por PagoID
/// </summary>
public class GetReciboHeaderByPagoIdQueryHandler : IRequestHandler<GetReciboHeaderByPagoIdQuery, ReciboHeaderCompletoDto?>
{
    private readonly IApplicationDbContext _context;

    public GetReciboHeaderByPagoIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReciboHeaderCompletoDto?> Handle(
        GetReciboHeaderByPagoIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.RecibosHeader
            .AsNoTracking()
            .Where(h => h.PagoId == request.PagoId)
            .Select(h => new ReciboHeaderCompletoDto
            {
                PagoId = h.PagoId,
                UserId = h.UserId,
                EmpleadoId = h.EmpleadoId,
                FechaRegistro = h.FechaRegistro,
                FechaPago = h.FechaPago,
                ConceptoPago = h.ConceptoPago,
                Tipo = h.Tipo,
                Detalles = _context.RecibosDetalle
                    .Where(d => d.PagoId == h.PagoId)
                    .Select(d => new EmpleadorReciboDetalleDto
                    {
                        DetalleId = d.DetalleId,
                        PagoId = d.PagoId,
                        Concepto = d.Concepto,
                        Monto = d.Monto
                    })
                    .ToList(),
                Empleado = _context.Empleados
                    .Where(e => e.EmpleadoId == h.EmpleadoId)
                    .Select(e => new EmpleadoBasicoDto
                    {
                        EmpleadoId = e.EmpleadoId,
                        Nombre = e.Nombre,
                        Apellido = e.Apellido,
                        Identificacion = e.Identificacion
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
