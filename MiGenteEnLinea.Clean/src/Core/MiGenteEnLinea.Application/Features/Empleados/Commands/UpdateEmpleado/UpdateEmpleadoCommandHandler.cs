using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Helpers;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateEmpleado;

/// <summary>
/// Handler para UpdateEmpleadoCommand.
/// Legacy: EmpleadosService.actualizarEmpleado(empleado) + ActualizarEmpleado(empleado)
/// Implementa actualización parcial: solo actualiza campos no nulos.
/// </summary>
public class UpdateEmpleadoCommandHandler : IRequestHandler<UpdateEmpleadoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPadronService _padronService;

    public UpdateEmpleadoCommandHandler(IApplicationDbContext context, IPadronService padronService)
    {
        _context = context;
        _padronService = padronService;
    }

    public async Task<bool> Handle(UpdateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        // PASO 1: Obtener empleado existente
        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.EmpleadoId == request.EmpleadoId && 
                                     e.UserId == request.UserId,
                                cancellationToken);

        if (empleado == null)
        {
            throw new NotFoundException("Empleado", request.EmpleadoId);
        }

        // PASO 2: Validar datos personales contra padrón cuando se modifican
        if (request.Nombre != null || request.Apellido != null || request.Nacimiento.HasValue)
        {
            var padronData = await _padronService.ConsultarCedulaAsync(empleado.Identificacion, cancellationToken);
            if (padronData == null)
            {
                throw new ValidationException(
                    "No se pudo validar la cédula en el padrón nacional. Intente nuevamente antes de guardar los cambios.");
            }

            var nacimientoActual = request.Nacimiento ?? empleado.Nacimiento?.ToDateTime(TimeOnly.MinValue);
            if (!PadronIdentityValidator.Matches(
                    request.Nombre ?? empleado.Nombre,
                    request.Apellido ?? empleado.Apellido,
                    nacimientoActual,
                    padronData,
                    out var padronMessage))
            {
                throw new ValidationException(padronMessage);
            }

            if (request.Foto == null && string.IsNullOrWhiteSpace(empleado.Foto) && !string.IsNullOrWhiteSpace(padronData.Photo))
            {
                empleado.ActualizarFoto(padronData.Photo);
            }
        }

        // PASO 3: Actualizar información personal (si se proporciona)
        if (request.Nombre != null || request.Apellido != null ||
            request.Alias != null || request.EstadoCivil.HasValue || request.Nacimiento.HasValue)
        {
            var nacimientoDateOnly = request.Nacimiento.HasValue 
                ? DateOnly.FromDateTime(request.Nacimiento.Value)
                : empleado.Nacimiento;

            empleado.ActualizarInformacionPersonal(
                nombre: request.Nombre ?? empleado.Nombre,
                apellido: request.Apellido ?? empleado.Apellido,
                nacimiento: nacimientoDateOnly,
                estadoCivil: request.EstadoCivil ?? empleado.EstadoCivil,
                alias: request.Alias
            );
        }

        // PASO 4: Actualizar información de contacto (si se proporciona)
        if (request.Telefono1 != null || request.Telefono2 != null ||
            request.ContactoEmergencia != null || request.TelefonoEmergencia != null)
        {
            empleado.ActualizarContacto(
                telefono1: request.Telefono1,
                telefono2: request.Telefono2,
                contactoEmergencia: request.ContactoEmergencia,
                telefonoEmergencia: request.TelefonoEmergencia
            );
        }

        // PASO 5: Actualizar dirección (si se proporciona)
        if (request.Direccion != null || request.Provincia != null || request.Municipio != null)
        {
            empleado.ActualizarDireccion(
                direccion: request.Direccion,
                provincia: request.Provincia,
                municipio: request.Municipio
            );
        }

        // PASO 6: Actualizar posición y salario (si se proporciona)
        if (request.Posicion != null || request.Salario.HasValue || request.PeriodoPago.HasValue)
        {
            empleado.ActualizarPosicion(
                posicion: request.Posicion ?? empleado.Posicion,
                salario: request.Salario ?? empleado.Salario,
                periodoPago: request.PeriodoPago ?? empleado.PeriodoPago
            );
        }

        // PASO 7: Actualizar fecha de inicio (si se proporciona)
        if (request.FechaInicio.HasValue)
        {
            var fechaInicioDateOnly = DateOnly.FromDateTime(request.FechaInicio.Value);
            empleado.ActualizarFechaInicio(fechaInicioDateOnly);
        }

        // PASO 8: Actualizar configuraciones adicionales
        if (request.DiasPago.HasValue)
        {
            // empleado.DiasPago = request.DiasPago.Value;
        }

        if (request.Tss.HasValue)
        {
            empleado.ActualizarInscripcionTss(request.Tss.Value);
        }

        // PASO 9: Actualizar foto (si se proporciona)
        if (request.Foto != null)
        {
            empleado.ActualizarFoto(request.Foto);
        }

        // PASO 10: Guardar cambios
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
