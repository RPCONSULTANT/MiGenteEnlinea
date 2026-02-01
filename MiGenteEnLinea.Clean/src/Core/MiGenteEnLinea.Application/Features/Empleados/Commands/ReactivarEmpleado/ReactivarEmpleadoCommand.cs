using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.ReactivarEmpleado;

/// <summary>
/// Comando para reactivar un empleado previamente dado de baja.
/// </summary>
public record ReactivarEmpleadoCommand(
    int EmpleadoId,
    string UserId) : IRequest<bool>;
