using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.ModificarCalificacion;

public class ModificarCalificacionCommandHandler : IRequestHandler<ModificarCalificacionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ModificarCalificacionCommandHandler> _logger;

    public ModificarCalificacionCommandHandler(
        IApplicationDbContext context,
        ILogger<ModificarCalificacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(ModificarCalificacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Modificando calificación {CalificacionId}",
            request.CalificacionId);

        var result = await _context.Calificaciones
            .Where(x => x.Id == request.CalificacionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.ContratistaIdentificacion, x => request.Identificacion ?? x.ContratistaIdentificacion)
                .SetProperty(x => x.Conocimientos, x => request.Conocimientos ?? x.Conocimientos)
                .SetProperty(x => x.Cumplimiento, x => request.Cumplimiento ?? x.Cumplimiento)
                .SetProperty(x => x.Fecha, x => request.Fecha ?? x.Fecha)
                .SetProperty(x => x.ContratistaNombre, x => request.Nombre ?? x.ContratistaNombre)
                .SetProperty(x => x.Puntualidad, x => request.Puntualidad ?? x.Puntualidad)
                .SetProperty(x => x.Recomendacion, x => request.Recomendacion ?? x.Recomendacion)
                .SetProperty(x => x.Tipo, x => request.Tipo ?? x.Tipo)
                .SetProperty(x => x.EmpleadorUserId, x => request.UserId ?? x.EmpleadorUserId), cancellationToken) > 0;

        if (result)
        {
            _logger.LogInformation(
                "Calificación {CalificacionId} modificada exitosamente",
                request.CalificacionId);
        }
        else
        {
            _logger.LogWarning(
                "No se encontró la calificación {CalificacionId} para modificar",
                request.CalificacionId);
        }

        return result;
    }
}
