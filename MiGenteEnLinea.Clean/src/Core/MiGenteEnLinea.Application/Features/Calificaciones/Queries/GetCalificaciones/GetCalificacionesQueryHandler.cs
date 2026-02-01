using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

namespace MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificaciones;

public class GetCalificacionesQueryHandler : IRequestHandler<GetCalificacionesQuery, List<CalificacionVistaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetCalificacionesQueryHandler> _logger;

    public GetCalificacionesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetCalificacionesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CalificacionVistaDto>> Handle(GetCalificacionesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo calificaciones - Identificacion: {Identificacion}, UserId: {UserId}",
            request.Identificacion,
            request.UserId);

        // Query base: filtrar por identificación del contratista
        var query = _context.Calificaciones
            .AsNoTracking()
            .Where(c => c.ContratistaIdentificacion == request.Identificacion);

        // Si se provee UserId, filtrar también por empleador
        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(c => c.EmpleadorUserId == request.UserId);
        }

        // Obtener calificaciones con join a Perfiles para nombre del calificador
        var calificaciones = await query
            .OrderByDescending(c => c.Id)
            .Select(c => new CalificacionVistaDto
            {
                CalificacionId = c.Id,
                UserId = c.EmpleadorUserId,
                Identificacion = c.ContratistaIdentificacion,
                Puntuacion = (c.Puntualidad + c.Cumplimiento + c.Conocimientos + c.Recomendacion) / 4,
                Comentario = null, // Legacy no tiene campo de comentario
                FechaCreacion = c.Fecha,
                NombreCalificador = c.ContratistaNombre, // Por ahora usamos el nombre del contratista
                ApellidoCalificador = string.Empty
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Se encontraron {Count} calificaciones", calificaciones.Count);
        
        return calificaciones;
    }
}
