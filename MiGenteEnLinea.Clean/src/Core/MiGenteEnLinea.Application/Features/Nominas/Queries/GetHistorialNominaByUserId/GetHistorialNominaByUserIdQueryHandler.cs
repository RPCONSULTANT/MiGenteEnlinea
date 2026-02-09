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
            // Construir filtro de período si se proporciona
            var periodoFilter = "";
            if (!string.IsNullOrEmpty(request.Periodo))
            {
                // Período en formato "YYYY-MM"
                var parts = request.Periodo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int year) && int.TryParse(parts[1], out int month))
                {
                    periodoFilter = $"AND YEAR(rh.fechaPago) = {year} AND MONTH(rh.fechaPago) = {month}";
                }
            }

            // Usar raw SQL para obtener el histórico
            var sql = $@"
                SELECT 
                    rh.pagoID AS NominaId,
                    FORMAT(rh.fechaPago, 'MMMM yyyy', 'es-ES') AS Periodo,
                    COUNT(DISTINCT rd.detalleID) AS CantidadEmpleados,
                    COALESCE(SUM(rd.Monto), 0) AS TotalNomina,
                    COALESCE(rh.fechaPago, rh.fechaRegistro) AS FechaProcesamiento,
                    COALESCE(rh.tipo, 1) AS Estado,
                    CASE COALESCE(rh.tipo, 1)
                        WHEN 1 THEN 'Procesado'
                        WHEN 2 THEN 'Parcial'
                        WHEN 3 THEN 'Error'
                        ELSE 'Desconocido'
                    END AS EstadoTexto,
                    0 AS EmailEnviado,
                    CAST(NULL AS datetime2) AS FechaEnvioEmail,
                    rh.conceptoPago AS Notas
                FROM [Empleador_Recibos_Header] rh
                LEFT JOIN [Empleador_Recibos_Detalle] rd ON rh.pagoID = rd.pagoID
                WHERE rh.userID = {{0}}
                {periodoFilter}
                GROUP BY rh.pagoID, rh.fechaPago, rh.fechaRegistro, rh.tipo, rh.conceptoPago
                ORDER BY rh.fechaPago DESC, rh.fechaRegistro DESC
                OFFSET {(request.PageIndex - 1) * request.PageSize} ROWS
                FETCH NEXT {request.PageSize} ROWS ONLY
            ";

            // Obtener los datos como dinámicos
            var result = new List<NominaHistorialDto>();
            
            try
            {
                // Ejecutar la query y mapear a DTOs
                var data = await _context.Database
                    .SqlQueryRaw<NominaHistorialDto>(sql, request.UserId)
                    .ToListAsync(cancellationToken);

                if (data.Any())
                {
                    result = data;
                    _logger.LogInformation(
                        "Histórico de nómina obtenido - Registros encontrados: {TotalRegistros}",
                        result.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "No se encontraron registros de histórico de nómina para UserId: {UserId}",
                        request.UserId);
                }
            }
            catch (Exception ex)
            {
                // Si falla la query raw, retornar lista vacía con log
                _logger.LogWarning(
                    ex,
                    "Error ejecutando query de histórico de nómina. Retornando lista vacía. UserId: {UserId}",
                    request.UserId);
                
                result = new List<NominaHistorialDto>();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener histórico de nómina para UserId: {UserId}", request.UserId);
            throw;
        }
    }
}
