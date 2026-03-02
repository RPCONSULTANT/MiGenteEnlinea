using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Empleados;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.DeleteRemuneracion;

/// <summary>
/// Handler para eliminar remuneración
/// Migrado desde: EmpleadosService.quitarRemuneracion(string userID, int id)
/// 
/// Legacy: 
/// var toDelete = db.Remuneraciones.Where(x => x.userID == userID && x.id == id).FirstOrDefault();
/// if (toDelete!=null) {
///     db.Remuneraciones.Remove(toDelete);
///     db.SaveChanges();
/// }
/// </summary>
public class DeleteRemuneracionCommandHandler : IRequestHandler<DeleteRemuneracionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteRemuneracionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteRemuneracionCommand request, CancellationToken cancellationToken)
    {
        var remuneracion = await _context.Set<Remuneracion>()
            .FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.RemuneracionId,
                cancellationToken);

        if (remuneracion is null)
        {
            return Unit.Value;
        }

        _context.Set<Remuneracion>().Remove(remuneracion);
        await _context.SaveChangesAsync(cancellationToken);

        // Legacy no lanza error si no encuentra (sólo valida != null)
        // Mantenemos comportamiento idéntico

        return Unit.Value;
    }
}
