using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Contrataciones;
using MiGenteEnLinea.Domain.Entities.Empleados;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;

public class CreateEmpleadoTemporalCommandHandler : IRequestHandler<CreateEmpleadoTemporalCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateEmpleadoTemporalCommandHandler> _logger;

    public CreateEmpleadoTemporalCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateEmpleadoTemporalCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(CreateEmpleadoTemporalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating EmpleadoTemporal for UserId: {UserId}, Name: {Nombre} {Apellido}",
            request.UserId,
            request.Nombre,
            request.Apellido);

        var executionStrategy = _context.Database.CreateExecutionStrategy();
        var contratacionId = 0;

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            var tipo = request.Tipo ?? 1;
            EmpleadoTemporal temporal;

            if (tipo == 2)
            {
                var nombreRepresentante = string.Join(" ", new[] { request.Nombre, request.Apellido }
                    .Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
                if (string.IsNullOrWhiteSpace(nombreRepresentante))
                {
                    nombreRepresentante = request.NombreComercial ?? "Representante";
                }

                temporal = EmpleadoTemporal.CreatePersonaJuridica(
                    userId: request.UserId,
                    nombreComercial: request.NombreComercial ?? string.Empty,
                    rnc: request.Rnc ?? string.Empty,
                    nombreRepresentante: nombreRepresentante,
                    cedulaRepresentante: request.Identificacion ?? request.Rnc ?? "N/A",
                    telefono1: request.Telefono);
            }
            else
            {
                temporal = EmpleadoTemporal.CreatePersonaFisica(
                    userId: request.UserId,
                    identificacion: request.Identificacion ?? string.Empty,
                    nombre: request.Nombre ?? string.Empty,
                    apellido: request.Apellido ?? string.Empty,
                    telefono1: request.Telefono);
            }

            temporal.ActualizarDireccion(request.Direccion, null, null);

            await _context.Set<EmpleadoTemporal>().AddAsync(temporal, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var detalle = DetalleContratacion.Crear(
                descripcionCorta: request.Servicio ?? string.Empty,
                fechaInicio: DateOnly.FromDateTime(request.FechaInicio!.Value),
                fechaFinal: DateOnly.FromDateTime(request.FechaFinal!.Value),
                montoAcordado: request.Pago ?? 0m,
                descripcionAmpliada: request.LugarTrabajo,
                esquemaPagos: request.HorarioTrabajo,
                contratacionId: temporal.ContratacionId);

            await _context.DetalleContrataciones.AddAsync(detalle, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.Estatus.HasValue && request.Estatus.Value != 1)
            {
                await _context.DetalleContrataciones
                    .Where(x => x.DetalleId == detalle.DetalleId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Estatus, request.Estatus.Value), cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);
            contratacionId = temporal.ContratacionId;
        });

        _logger.LogInformation(
            "EmpleadoTemporal created successfully. ContratacionId: {ContratacionId}",
            contratacionId);

        return contratacionId;
    }
}
