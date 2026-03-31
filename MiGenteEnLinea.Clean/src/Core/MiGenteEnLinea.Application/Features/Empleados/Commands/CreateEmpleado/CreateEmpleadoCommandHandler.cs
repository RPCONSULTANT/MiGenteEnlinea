using MediatR;
using MiGenteEnLinea.Application.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Empleados;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleado;

/// <summary>
/// Handler para CreateEmpleadoCommand.
/// Legacy: EmpleadosService.guardarEmpleado(empleado)
/// </summary>
public class CreateEmpleadoCommandHandler : IRequestHandler<CreateEmpleadoCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IPadronService _padronService;

    public CreateEmpleadoCommandHandler(IApplicationDbContext context, IPadronService padronService)
    {
        _context = context;
        _padronService = padronService;
    }

    public async Task<int> Handle(CreateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        // PASO 1: Validar que el empleador existe
        var empleadorExists = await _context.Credenciales
            .AnyAsync(c => c.UserId == request.UserId, cancellationToken);

        if (!empleadorExists)
        {
            throw new NotFoundException("Credencial", request.UserId);
        }

        // PASO 2: Validar límite de empleados según el plan del empleador
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var suscripcionActiva = await _context.Suscripciones
            .Where(s => s.UserId == request.UserId && s.Vencimiento > hoy)
            .OrderByDescending(s => s.Vencimiento)
            .FirstOrDefaultAsync(cancellationToken);

        if (suscripcionActiva == null)
        {
            throw new ValidationException("No tiene una suscripción activa. Por favor adquiera un plan para continuar.");
        }

        var plan = await _context.PlanesEmpleadores
            .FirstOrDefaultAsync(p => p.PlanId == suscripcionActiva.PlanId, cancellationToken);

        if (plan != null && plan.LimiteEmpleados > 0)
        {
            var empleadosActuales = await _context.Empleados
                .CountAsync(e => e.UserId == request.UserId && e.Activo, cancellationToken);

            if (empleadosActuales >= plan.LimiteEmpleados)
            {
                throw new ValidationException(
                    $"Ha alcanzado el límite de {plan.LimiteEmpleados} empleado(s) permitidos en su plan '{plan.Nombre}'. " +
                    "Actualice su plan para agregar más empleados.");
            }
        }

        // PASO 3: Validar que la identificación no esté duplicada para este empleador
        var duplicateExists = await _context.Empleados
            .AnyAsync(e => e.UserId == request.UserId && 
                           e.Identificacion == request.Identificacion,
                     cancellationToken);

        if (duplicateExists)
        {
            throw new ValidationException(
                $"Ya existe un empleado con la identificación {request.Identificacion}");
        }

        // PASO 4: Validar cédula contra padrón nacional
        var padronData = await _padronService.ConsultarCedulaAsync(request.Identificacion, cancellationToken);
        if (padronData == null)
        {
            throw new ValidationException(
                "No se pudo validar la cédula en el padrón nacional. Verifique el número ingresado antes de registrar el colaborador.");
        }

        if (!PadronIdentityValidator.Matches(request.Nombre, request.Apellido, request.Nacimiento, padronData, out var padronMessage))
        {
            throw new ValidationException(padronMessage);
        }

        // PASO 5: Crear empleado usando factory method del dominio
        var empleado = Empleado.Create(
            userId: request.UserId,
            identificacion: request.Identificacion,
            nombre: request.Nombre,
            apellido: request.Apellido,
            salario: request.Salario,
            periodoPago: request.PeriodoPago
        );

        // PASO 6: Actualizar fecha de inicio
        var fechaInicioDateOnly = DateOnly.FromDateTime(request.FechaInicio);
        empleado.ActualizarFechaInicio(fechaInicioDateOnly);

        // PASO 7: Actualizar información personal si se proporciona
        if (request.EstadoCivil.HasValue || request.Nacimiento.HasValue || !string.IsNullOrEmpty(request.Alias))
        {
            var nacimientoDateOnly = request.Nacimiento.HasValue 
                ? DateOnly.FromDateTime(request.Nacimiento.Value)
                : (DateOnly?)null;

            empleado.ActualizarInformacionPersonal(
                nombre: request.Nombre,
                apellido: request.Apellido,
                nacimiento: nacimientoDateOnly,
                estadoCivil: request.EstadoCivil,
                alias: request.Alias
            );
        }

        // PASO 8: Actualizar información de contacto
        if (!string.IsNullOrEmpty(request.Telefono1) || 
            !string.IsNullOrEmpty(request.Telefono2))
        {
            empleado.ActualizarContacto(
                telefono1: request.Telefono1,
                telefono2: request.Telefono2,
                contactoEmergencia: request.ContactoEmergencia,
                telefonoEmergencia: request.TelefonoEmergencia
            );
        }

        // PASO 9: Actualizar dirección
        if (!string.IsNullOrEmpty(request.Direccion) ||
            !string.IsNullOrEmpty(request.Provincia) ||
            !string.IsNullOrEmpty(request.Municipio))
        {
            empleado.ActualizarDireccion(
                direccion: request.Direccion,
                provincia: request.Provincia,
                municipio: request.Municipio
            );
        }

        // PASO 10: Actualizar posición si se proporciona
        if (!string.IsNullOrEmpty(request.Posicion))
        {
            empleado.ActualizarPosicion(
                posicion: request.Posicion,
                salario: request.Salario,
                periodoPago: request.PeriodoPago
            );
        }

        // PASO 11: Configurar TSS
        if (request.Tss)
        {
            empleado.ActualizarInscripcionTss(true);
        }

        // PASO 12: Configurar días de pago si se proporciona
        if (request.DiasPago.HasValue)
        {
            // Acceso directo a propiedad por ahora
            // empleado.DiasPago = request.DiasPago.Value;
        }

        // PASO 13: Asignar foto manual o la foto validada del padrón
        var fotoFinal = !string.IsNullOrWhiteSpace(request.Foto) ? request.Foto : padronData.Photo;
        if (!string.IsNullOrWhiteSpace(fotoFinal))
        {
            empleado.ActualizarFoto(fotoFinal);
        }

        // PASO 14: Guardar en base de datos
        await _context.Empleados.AddAsync(empleado, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return empleado.EmpleadoId;
    }
}
