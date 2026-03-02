using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetDeduccionesTss;

/// <summary>
/// Handler para GetDeduccionesTssQuery.
/// Migrado de: EmpleadosService.deducciones() - Line 680
/// Retorna el catálogo completo de deducciones TSS sin filtros.
/// </summary>
public class GetDeduccionesTssQueryHandler : IRequestHandler<GetDeduccionesTssQuery, List<DeduccionTssDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDeduccionesTssQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeduccionTssDto>> Handle(GetDeduccionesTssQuery request, CancellationToken cancellationToken)
    {
        return await _context.DeduccionesTss
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new DeduccionTssDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Porcentaje = x.Porcentaje
            })
            .ToListAsync(cancellationToken);
    }
}
