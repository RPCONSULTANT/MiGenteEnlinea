using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;
using MiGenteEnLinea.Domain.Entities.Empleados;

namespace MiGenteEnLinea.Application.Features.Nominas.Queries.GetHistorialNominaUnificado;

public class GetHistorialNominaUnificadoQueryHandler : IRequestHandler<GetHistorialNominaUnificadoQuery, List<NominaHistorialUnificadoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHistorialNominaUnificadoQueryHandler> _logger;

    public GetHistorialNominaUnificadoQueryHandler(
        IApplicationDbContext context,
        ILogger<GetHistorialNominaUnificadoQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<NominaHistorialUnificadoDto>> Handle(
        GetHistorialNominaUnificadoQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 200);
        var empleadosTemporales = _context.Set<EmpleadoTemporal>().AsNoTracking();

        var fixedRows = await _context.RecibosHeader
            .AsNoTracking()
            .Where(rh => rh.UserId == request.UserId)
            .Select(rh => new
            {
                rh.PagoId,
                FechaPago = rh.FechaPago ?? rh.FechaRegistro,
                Nombre = _context.Empleados
                    .Where(e => e.EmpleadoId == rh.EmpleadoId)
                    .Select(e => e.Nombre)
                    .FirstOrDefault(),
                Apellido = _context.Empleados
                    .Where(e => e.EmpleadoId == rh.EmpleadoId)
                    .Select(e => e.Apellido)
                    .FirstOrDefault(),
                Concepto = rh.ConceptoPago,
                TotalBruto = _context.RecibosDetalle
                    .Where(rd => rd.PagoId == rh.PagoId && rd.Monto > 0)
                    .Select(rd => (decimal?)rd.Monto)
                    .Sum() ?? 0m,
                TotalDeducciones = _context.RecibosDetalle
                    .Where(rd => rd.PagoId == rh.PagoId && rd.Monto < 0)
                    .Select(rd => (decimal?)(-rd.Monto))
                    .Sum() ?? 0m,
                TotalNeto = _context.RecibosDetalle
                    .Where(rd => rd.PagoId == rh.PagoId)
                    .Select(rd => (decimal?)rd.Monto)
                    .Sum() ?? 0m,
                ReferenciaId = rh.EmpleadoId,
                Estado = rh.Tipo
            })
            .ToListAsync(cancellationToken);

        var temporalRows = await _context.EmpleadorRecibosHeaderContrataciones
            .AsNoTracking()
            .Where(rh => rh.UserId == request.UserId)
            .Select(rh => new
            {
                rh.PagoId,
                FechaPago = rh.FechaPago ?? rh.FechaRegistro,
                Nombre = empleadosTemporales
                    .Where(et => et.ContratacionId == rh.ContratacionId)
                    .Select(et => et.Nombre)
                    .FirstOrDefault(),
                Apellido = empleadosTemporales
                    .Where(et => et.ContratacionId == rh.ContratacionId)
                    .Select(et => et.Apellido)
                    .FirstOrDefault(),
                NombreComercial = empleadosTemporales
                    .Where(et => et.ContratacionId == rh.ContratacionId)
                    .Select(et => et.NombreComercial)
                    .FirstOrDefault(),
                Concepto = rh.ConceptoPago,
                TotalBruto = _context.EmpleadorRecibosDetalleContrataciones
                    .Where(rd => rd.PagoId == rh.PagoId)
                    .Select(rd => rd.Monto ?? 0m)
                    .Sum(),
                ReferenciaId = rh.ContratacionId ?? 0,
                Estado = rh.Tipo ?? 1
            })
            .ToListAsync(cancellationToken);

        var query = fixedRows
            .Select(x => new NominaHistorialUnificadoDto
            {
                PagoId = x.PagoId,
                FechaPago = x.FechaPago,
                Beneficiario = BuildFixedBeneficiario(x.Nombre, x.Apellido),
                Concepto = string.IsNullOrWhiteSpace(x.Concepto) ? "Pago de nómina" : x.Concepto,
                TotalBruto = x.TotalBruto,
                TotalDeducciones = x.TotalDeducciones,
                TotalNeto = x.TotalNeto,
                TipoRegistro = "Fijo",
                ReferenciaId = x.ReferenciaId,
                Estado = x.Estado
            })
            .Concat(temporalRows.Select(x => new NominaHistorialUnificadoDto
            {
                PagoId = x.PagoId,
                FechaPago = x.FechaPago ?? DateTime.MinValue,
                Beneficiario = BuildTemporalBeneficiario(x.Nombre, x.Apellido, x.NombreComercial),
                Concepto = string.IsNullOrWhiteSpace(x.Concepto) ? "Pago de contratación temporal" : x.Concepto,
                TotalBruto = x.TotalBruto,
                TotalDeducciones = 0m,
                TotalNeto = x.TotalBruto,
                TipoRegistro = "Temporal",
                ReferenciaId = x.ReferenciaId,
                Estado = x.Estado
            }))
            .AsQueryable();

        if (request.FechaDesde.HasValue)
        {
            query = query.Where(x => x.FechaPago >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(x => x.FechaPago <= request.FechaHasta.Value);
        }

        var result = query
            .OrderByDescending(x => x.FechaPago)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        _logger.LogInformation(
            "Historial unificado obtenido para user {UserId}. Registros: {Count}",
            request.UserId,
            result.Count);

        return result;
    }

    private static string BuildFixedBeneficiario(string? nombre, string? apellido)
    {
        var nombreCompleto = string.Join(" ", new[] { nombre, apellido }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
        return string.IsNullOrWhiteSpace(nombreCompleto) ? "Empleado" : nombreCompleto;
    }

    private static string BuildTemporalBeneficiario(string? nombre, string? apellido, string? nombreComercial)
    {
        var nombrePersona = string.Join(" ", new[] { nombre, apellido }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
        if (!string.IsNullOrWhiteSpace(nombrePersona))
        {
            return nombrePersona;
        }

        return string.IsNullOrWhiteSpace(nombreComercial) ? "Contratista temporal" : nombreComercial.Trim();
    }
}
