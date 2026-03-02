using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Empleados;
using MiGenteEnLinea.Domain.Entities.Contrataciones;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.CreateContratacion;

/// <summary>
/// Handler para crear una nueva propuesta de contratación.
/// 
/// LÓGICA DE NEGOCIO:
/// 1. Validar datos de entrada (FluentValidation se ejecuta antes)
/// 2. Crear entidad DetalleContratacion usando factory method
/// 3. Guardar en base de datos vía DbContext
/// 4. Domain Event ContratacionCreadaEvent se dispara automáticamente
/// 5. Retornar ID del detalle creado
/// </summary>
public class CreateContratacionCommandHandler : IRequestHandler<CreateContratacionCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateContratacionCommandHandler> _logger;

    public CreateContratacionCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateContratacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(CreateContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating new contratacion for user {UserId}. Contratista: {ContratistaId}, ContratacionPadre: {ContratacionId}, Amount: {MontoAcordado}",
            request.EmpleadorUserId,
            request.ContratistaId,
            request.ContratacionId,
            request.MontoAcordado);

        try
        {
            if (!request.ContratacionId.HasValue)
            {
                throw new ArgumentException("Debe seleccionar un contratista antes de crear la contratación");
            }

            var contratista = await _context.Contratistas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ContratistaId, cancellationToken);

            if (contratista == null || !contratista.Activo)
            {
                throw new ArgumentException("El contratista seleccionado no existe o no está activo");
            }

            var empleadoTemporal = await _context.Set<EmpleadoTemporal>()
                .AsNoTracking()
                .FirstOrDefaultAsync(et => et.ContratacionId == request.ContratacionId.Value, cancellationToken);

            if (empleadoTemporal == null)
            {
                throw new ArgumentException("La contratación temporal base no existe");
            }

            if (!string.Equals(empleadoTemporal.UserId, request.EmpleadorUserId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("La contratación temporal seleccionada no pertenece al usuario autenticado");
            }

            if (!empleadoTemporal.Activo)
            {
                throw new ArgumentException("La contratación temporal seleccionada está inactiva");
            }

            _logger.LogInformation(
            "Creating detalle contratacion: {DescripcionCorta}, Amount: {MontoAcordado}, Start: {FechaInicio}",
            request.DescripcionCorta,
            request.MontoAcordado,
            request.FechaInicio);
            // Crear entidad usando factory method del Domain
            var contratacion = DetalleContratacion.Crear(
                descripcionCorta: request.DescripcionCorta,
                fechaInicio: request.FechaInicio,
                fechaFinal: request.FechaFinal,
                montoAcordado: request.MontoAcordado,
                descripcionAmpliada: request.DescripcionAmpliada,
                esquemaPagos: request.EsquemaPagos,
                contratacionId: request.ContratacionId
            );

            // Agregar notas si se especificaron
            if (!string.IsNullOrWhiteSpace(request.Notas))
            {
                contratacion.ActualizarNotas(request.Notas);
            }

            // Guardar en base de datos
            await _context.DetalleContrataciones.AddAsync(contratacion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Contratacion created successfully with ID: {DetalleId}",
                contratacion.DetalleId);

            return contratacion.DetalleId;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating contratacion: {Message}", ex.Message);
            throw; // FluentValidation debería haber capturado esto, pero por si acaso
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contratacion");
            throw;
        }
    }
}
