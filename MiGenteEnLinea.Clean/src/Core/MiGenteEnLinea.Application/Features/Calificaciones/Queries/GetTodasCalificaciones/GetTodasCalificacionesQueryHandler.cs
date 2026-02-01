using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

namespace MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetTodasCalificaciones;

public class GetTodasCalificacionesQueryHandler : IRequestHandler<GetTodasCalificacionesQuery, List<CalificacionVistaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTodasCalificacionesQueryHandler> _logger;

    public GetTodasCalificacionesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTodasCalificacionesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las calificaciones
    /// Replica CalificacionesService.getTodas()
    /// </summary>
    public async Task<List<CalificacionVistaDto>> Handle(GetTodasCalificacionesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo todas las calificaciones");

        var calificaciones = await _context.Calificaciones
            .AsNoTracking()
            .OrderByDescending(c => c.Id)
            .Take(100) // Limitar para performance
            .Select(c => new CalificacionVistaDto
            {
                CalificacionId = c.Id,
                UserId = c.EmpleadorUserId,
                Identificacion = c.ContratistaIdentificacion,
                Puntuacion = (c.Puntualidad + c.Cumplimiento + c.Conocimientos + c.Recomendacion) / 4,
                Comentario = null,
                FechaCreacion = c.Fecha,
                NombreCalificador = c.ContratistaNombre,
                ApellidoCalificador = string.Empty
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Se encontraron {Count} calificaciones", calificaciones.Count);
        
        return calificaciones;
    }
}
