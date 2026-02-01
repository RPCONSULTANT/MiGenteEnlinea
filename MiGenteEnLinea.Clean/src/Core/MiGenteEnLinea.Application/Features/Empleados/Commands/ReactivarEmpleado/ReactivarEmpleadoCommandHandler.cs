using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using DomainEmpleado = MiGenteEnLinea.Domain.Entities.Empleados.Empleado;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.ReactivarEmpleado;

/// <summary>
/// Handler para reactivar un empleado previamente dado de baja.
/// Usa el método Reactivar() de la entidad de dominio.
/// </summary>
public class ReactivarEmpleadoCommandHandler : IRequestHandler<ReactivarEmpleadoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReactivarEmpleadoCommandHandler> _logger;

    public ReactivarEmpleadoCommandHandler(
        IApplicationDbContext context,
        ILogger<ReactivarEmpleadoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(ReactivarEmpleadoCommand request, CancellationToken cancellationToken)
    {
        // Buscar en la tabla de empleados de dominio
        var empleado = await _context.Set<DomainEmpleado>()
            .FirstOrDefaultAsync(e => e.EmpleadoId == request.EmpleadoId, cancellationToken);

        if (empleado == null)
        {
            _logger.LogWarning("Empleado no encontrado: {EmpleadoId}", request.EmpleadoId);
            throw new Exception($"Empleado con ID {request.EmpleadoId} no encontrado");
        }

        // Usar el método de dominio para reactivar
        empleado.Reactivar();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Empleado reactivado exitosamente: {EmpleadoId}, Usuario: {UserId}",
            request.EmpleadoId,
            request.UserId);

        return true;
    }
}
