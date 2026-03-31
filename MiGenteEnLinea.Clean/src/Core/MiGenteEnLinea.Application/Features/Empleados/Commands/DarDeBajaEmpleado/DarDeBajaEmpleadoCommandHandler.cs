using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.DarDeBajaEmpleado;

public class DarDeBajaEmpleadoCommandHandler : IRequestHandler<DarDeBajaEmpleadoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DarDeBajaEmpleadoCommandHandler> _logger;

    public DarDeBajaEmpleadoCommandHandler(
        IApplicationDbContext context,
        ILogger<DarDeBajaEmpleadoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DarDeBajaEmpleadoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Dando de baja empleado: {EmpleadoId}, Fecha: {FechaBaja}, Motivo: {Motivo}",
            request.EmpleadoId,
            request.FechaBaja,
            request.Motivo);

        var empleado = await _context.Empleados
            .AsNoTracking()
            .Where(e => e.EmpleadoId == request.EmpleadoId && e.UserId == request.UserId)
            .Select(e => new { e.EmpleadoId, e.Activo })
            .FirstOrDefaultAsync(cancellationToken);

        if (empleado is null)
        {
            throw new InvalidOperationException(
                $"Empleado {request.EmpleadoId} no encontrado o no pertenece al usuario {request.UserId}");
        }

        if (!empleado.Activo)
        {
            throw new InvalidOperationException($"Empleado {request.EmpleadoId} ya está dado de baja");
        }

        if (request.Prestaciones <= 0)
        {
            throw new InvalidOperationException("Debe calcular y registrar el monto de prestaciones antes de dar de baja al colaborador");
        }

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new InvalidOperationException("Debe indicar el motivo de la baja del colaborador");
        }

        var rowsAffected = await _context.Empleados
            .Where(e => e.EmpleadoId == request.EmpleadoId && e.UserId == request.UserId && e.Activo)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.Activo, false)
                .SetProperty(e => e.FechaSalida, request.FechaBaja.Date)
                .SetProperty(e => e.MotivoBaja, request.Motivo)
                .SetProperty(e => e.Prestaciones, request.Prestaciones), cancellationToken);

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"No se pudo dar de baja al empleado {request.EmpleadoId}");
        }

        _logger.LogInformation("Empleado dado de baja exitosamente: {EmpleadoId}", request.EmpleadoId);
        
        return true;
    }
}
