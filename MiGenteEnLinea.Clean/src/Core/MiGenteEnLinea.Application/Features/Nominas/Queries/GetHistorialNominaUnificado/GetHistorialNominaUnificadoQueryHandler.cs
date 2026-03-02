using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;

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

        const string sql = @"
            SELECT
                rh.pagoID AS PagoId,
                COALESCE(rh.fechaPago, rh.fechaRegistro) AS FechaPago,
                COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre, ' ', e.apellido))), ''), 'Empleado') AS Beneficiario,
                COALESCE(rh.conceptoPago, 'Pago de nómina') AS Concepto,
                COALESCE(SUM(CASE WHEN COALESCE(rd.Monto, 0) > 0 THEN rd.Monto ELSE 0 END), 0) AS TotalBruto,
                ABS(COALESCE(SUM(CASE WHEN COALESCE(rd.Monto, 0) < 0 THEN rd.Monto ELSE 0 END), 0)) AS TotalDeducciones,
                COALESCE(SUM(COALESCE(rd.Monto, 0)), 0) AS TotalNeto,
                CAST('Fijo' AS nvarchar(20)) AS TipoRegistro,
                COALESCE(rh.empleadoID, 0) AS ReferenciaId,
                COALESCE(rh.tipo, 1) AS Estado
            FROM Empleador_Recibos_Header rh
            LEFT JOIN Empleador_Recibos_Detalle rd ON rd.pagoID = rh.pagoID
            LEFT JOIN Empleados e ON e.empleadoID = rh.empleadoID
            WHERE rh.userID = {0}
            GROUP BY rh.pagoID, rh.fechaPago, rh.fechaRegistro, rh.conceptoPago, rh.empleadoID, rh.tipo, e.nombre, e.apellido

            UNION ALL

            SELECT
                rhc.pagoID AS PagoId,
                COALESCE(rhc.fechaPago, rhc.fechaRegistro) AS FechaPago,
                COALESCE(
                    NULLIF(LTRIM(RTRIM(CONCAT(et.nombre, ' ', et.apellido))), ''),
                    NULLIF(LTRIM(RTRIM(et.nombreComercial)), ''),
                    'Contratista temporal'
                ) AS Beneficiario,
                COALESCE(rhc.conceptoPago, 'Pago de contratación temporal') AS Concepto,
                COALESCE(SUM(COALESCE(rdc.Monto, 0)), 0) AS TotalBruto,
                CAST(0 AS decimal(18,2)) AS TotalDeducciones,
                COALESCE(SUM(COALESCE(rdc.Monto, 0)), 0) AS TotalNeto,
                CAST('Temporal' AS nvarchar(20)) AS TipoRegistro,
                COALESCE(rhc.contratacionID, 0) AS ReferenciaId,
                COALESCE(rhc.tipo, 1) AS Estado
            FROM Empleador_Recibos_Header_Contrataciones rhc
            LEFT JOIN Empleador_Recibos_Detalle_Contrataciones rdc ON rdc.pagoID = rhc.pagoID
            LEFT JOIN Empleados_Temporales et ON et.contratacionID = rhc.contratacionID
            WHERE rhc.userID = {0}
            GROUP BY rhc.pagoID, rhc.fechaPago, rhc.fechaRegistro, rhc.conceptoPago, rhc.contratacionID, rhc.tipo, et.nombre, et.apellido, et.nombreComercial
        ";

        var rows = await _context.Database
            .SqlQueryRaw<NominaHistorialUnificadoDto>(sql, request.UserId)
            .ToListAsync(cancellationToken);

        var query = rows.AsQueryable();
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
}
