using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;
using System.Globalization;

namespace MiGenteEnLinea.Application.Features.Nominas.Queries.GetHistorialNominaByUserId;

/// <summary>
/// Handler para obtener el histórico paginado de nóminas procesadas de un empleador.
/// 
/// LÓGICA DE NEGOCIO:
/// 1. Valida que el usuario sea un empleador
/// 2. Obtiene EmpleadorRecibosHeader filtrado por UserId
/// 3. Aplica filtros opcionales (período, estado)
/// 4. Retorna paginado ordenado por fecha descendente
/// 5. Incluye información de cantidad de empleados y total por nómina
/// </summary>
public class GetHistorialNominaByUserIdQueryHandler : IRequestHandler<GetHistorialNominaByUserIdQuery, List<NominaHistorialDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHistorialNominaByUserIdQueryHandler> _logger;

    public GetHistorialNominaByUserIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetHistorialNominaByUserIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<NominaHistorialDto>> Handle(
        GetHistorialNominaByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo histórico de nómina - UserId: {UserId}, PageIndex: {PageIndex}, PageSize: {PageSize}",
            request.UserId,
            request.PageIndex,
            request.PageSize);

        try
        {
            var userIdAsString = request.UserId.ToString(CultureInfo.InvariantCulture);
            var query = _context.RecibosHeader
                .AsNoTracking()
                .Where(x => x.UserId == userIdAsString);

            // Construir filtro de período si se proporciona (formato YYYY-MM)
            if (!string.IsNullOrEmpty(request.Periodo))
            {
                var parts = request.Periodo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int year) && int.TryParse(parts[1], out int month))
                {
                    query = query.Where(rh =>
                        (rh.FechaPago ?? rh.FechaRegistro).Year == year &&
                        (rh.FechaPago ?? rh.FechaRegistro).Month == month);
                }
            }

            if (request.Estado.HasValue)
            {
                query = query.Where(rh => rh.Tipo == request.Estado.Value);
            }

            var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 100);
            var spanishCulture = CultureInfo.GetCultureInfo("es-ES");

            var rows = await query
                .OrderByDescending(rh => rh.FechaPago ?? rh.FechaRegistro)
                .ThenByDescending(rh => rh.FechaRegistro)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(rh => new
                {
                    rh.PagoId,
                    FechaProcesamiento = rh.FechaPago ?? rh.FechaRegistro,
                    rh.Tipo,
                    rh.ConceptoPago,
                    CantidadEmpleados = _context.RecibosDetalle
                        .Where(rd => rd.PagoId == rh.PagoId)
                        .Select(rd => rd.DetalleId)
                        .Distinct()
                        .Count(),
                    TotalNomina = _context.RecibosDetalle
                        .Where(rd => rd.PagoId == rh.PagoId)
                        .Select(rd => (decimal?)rd.Monto)
                        .Sum() ?? 0m
                })
                .ToListAsync(cancellationToken);

            var result = rows.Select(row => new NominaHistorialDto
                {
                    NominaId = row.PagoId,
                    Periodo = row.FechaProcesamiento.ToString("MMMM yyyy", spanishCulture),
                    CantidadEmpleados = row.CantidadEmpleados,
                    TotalNomina = row.TotalNomina,
                    FechaProcesamiento = row.FechaProcesamiento,
                    Estado = row.Tipo,
                    EstadoTexto = MapEstadoTexto(row.Tipo),
                    EmailEnviado = false,
                    FechaEnvioEmail = null,
                    Notas = row.ConceptoPago
                })
                .ToList();

            _logger.LogInformation(
                "Histórico de nómina obtenido - Registros encontrados: {TotalRegistros}",
                result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener histórico de nómina para UserId: {UserId}", request.UserId);
            throw;
        }
    }

    private static string MapEstadoTexto(int estado)
    {
        return estado switch
        {
            1 => "Procesado",
            2 => "Parcial",
            3 => "Error",
            _ => "Desconocido"
        };
    }
}
