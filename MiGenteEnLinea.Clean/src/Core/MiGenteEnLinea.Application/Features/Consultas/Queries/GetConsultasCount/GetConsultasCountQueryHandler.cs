using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Consultas.Queries.GetConsultasCount;

/// <summary>
/// Handler para obtener el conteo de consultas de perfil realizadas por un empleador
/// </summary>
public class GetConsultasCountQueryHandler : IRequestHandler<GetConsultasCountQuery, int>
{
    private readonly IApplicationDbContext _context;

    public GetConsultasCountQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetConsultasCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _context.ConsultasPerfil
            .AsNoTracking()
            .Where(c => c.EmpleadorUserId == request.EmpleadorUserId)
            .CountAsync(cancellationToken);

        return count;
    }
}
