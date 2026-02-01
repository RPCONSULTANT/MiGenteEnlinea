using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

namespace MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificacionesByEmpleador;

/// <summary>
/// Handler: Obtener calificaciones realizadas por un empleador
/// </summary>
public class GetCalificacionesByEmpleadorQueryHandler 
    : IRequestHandler<GetCalificacionesByEmpleadorQuery, List<CalificacionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCalificacionesByEmpleadorQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CalificacionDto>> Handle(
        GetCalificacionesByEmpleadorQuery request, 
        CancellationToken cancellationToken)
    {
        var calificaciones = await _context.Calificaciones
            .AsNoTracking()
            .Where(c => c.EmpleadorUserId == request.UserId)
            .OrderByDescending(c => c.Fecha)
            .ThenByDescending(c => c.Id)
            .ProjectTo<CalificacionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return calificaciones;
    }
}
