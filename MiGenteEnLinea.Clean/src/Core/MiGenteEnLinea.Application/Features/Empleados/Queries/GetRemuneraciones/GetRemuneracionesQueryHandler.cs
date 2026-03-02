using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;
using MiGenteEnLinea.Domain.Entities.Empleados;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetRemuneraciones;

public class GetRemuneracionesQueryHandler : IRequestHandler<GetRemuneracionesQuery, List<RemuneracionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRemuneracionesQueryHandler> _logger;

    public GetRemuneracionesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetRemuneracionesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene remuneraciones adicionales de un empleado
    /// Replica EmpleadosService.obtenerRemuneraciones()
    /// </summary>
    public async Task<List<RemuneracionDto>> Handle(GetRemuneracionesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo remuneraciones - UserId: {UserId}, EmpleadoId: {EmpleadoId}",
            request.UserId,
            request.EmpleadoId);

        var remuneraciones = await _context.Set<Remuneracion>()
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.EmpleadoId == request.EmpleadoId)
            .Select(x => new RemuneracionDto
            {
                Id = x.Id,
                UserId = x.UserId,
                EmpleadoId = x.EmpleadoId,
                Descripcion = x.Descripcion,
                Monto = x.Monto
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Remuneraciones encontradas: {Count}", remuneraciones.Count);

        return remuneraciones;
    }
}
