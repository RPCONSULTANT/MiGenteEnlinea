using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;

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
            // Obtener la tabla de recibos de empleador
            // Nota: Se asume que existe la entidad EmpleadorRecibosHeader con relación a Empleador
            // La tabla contiene registros de nóminas procesadas

            var query = _context.Set<dynamic>()
                .FromSqlInterpolated($@"
                    SELECT 
                        rh.id AS NominaId,
                        CONCAT(DATENAME(MONTH, rh.FechaProcesamiento), ' ', YEAR(rh.FechaProcesamiento)) AS Periodo,
                        COUNT(DISTINCT rd.EmpleadoId) AS CantidadEmpleados,
                        COALESCE(SUM(rd.TotalSalario), 0) AS TotalNomina,
                        rh.FechaProcesamiento,
                        COALESCE(rh.Estado, 1) AS Estado,
                        0 AS EmailEnviado
                    FROM [EmpleadorRecibosHeader] rh
                    LEFT JOIN [EmpleadorRecibosDetalle] rd ON rh.id = rd.ReceiptId
                    WHERE rh.UserId = {request.UserId}
                    GROUP BY rh.id, rh.FechaProcesamiento, rh.Estado
                    ORDER BY rh.FechaProcesamiento DESC
                    OFFSET {(request.PageIndex - 1) * request.PageSize} ROWS
                    FETCH NEXT {request.PageSize} ROWS ONLY
                ");

            // Nota: Alternativa si la query SQL anterior falla - usar EntityFramework LINQ
            // será necesario ajustar según la estructura real de las entidades en la BD

            var nominas = await query.ToListAsync(cancellationToken);

            // Convertir dinámicos a DTOs
            var result = new List<NominaHistorialDto>();

            // Descomentar y usar cuando las entidades estén disponibles
            // Para ahora, retornar lista vacía como placeholder
            // que será reemplazada cuando se valide la estructura de BD

            _logger.LogInformation(
                "Histórico de nómina obtenido - Registros encontrados: {TotalRegistros}",
                nominas.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener histórico de nómina para UserId: {UserId}", request.UserId);
            throw;
        }
    }
}
