using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CalificarContratacion;

public class CalificarContratacionCommandHandler : IRequestHandler<CalificarContratacionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CalificarContratacionCommandHandler> _logger;

    public CalificarContratacionCommandHandler(
        IApplicationDbContext context,
        ILogger<CalificarContratacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(CalificarContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Calificando contratación {ContratacionId} con calificación {CalificacionId}",
            request.ContratacionId,
            request.CalificacionId);

        var rows = await _context.DetalleContrataciones
            .Where(x => x.ContratacionId == request.ContratacionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Calificado, true)
                .SetProperty(x => x.CalificacionId, request.CalificacionId), cancellationToken);

        var result = rows > 0;

        if (result)
        {
            _logger.LogInformation(
                "Contratación {ContratacionId} calificada exitosamente",
                request.ContratacionId);
        }
        else
        {
            _logger.LogWarning(
                "No se encontró la contratación {ContratacionId} para calificar",
                request.ContratacionId);
        }

        return result;
    }
}
