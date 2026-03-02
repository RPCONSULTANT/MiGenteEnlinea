using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Domain.Entities.Empleados;
using MiGenteEnLinea.Domain.Entities.Contrataciones;
using MiGenteEnLinea.Domain.Interfaces;
using MiGenteEnLinea.Domain.Interfaces.Repositories;
using System;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.AcceptContratacion;

/// <summary>
/// Handler para aceptar una propuesta de contratación.
/// 
/// LÓGICA:
/// 1. Buscar DetalleContratacion por ID
/// 2. Validar que existe
/// 3. Llamar al método Aceptar() del Domain (valida estado Pendiente)
/// 4. Guardar cambios (UnitOfWork commit)
/// 5. Domain Event se dispara automáticamente
/// </summary>
public class AcceptContratacionCommandHandler : IRequestHandler<AcceptContratacionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AcceptContratacionCommandHandler> _logger;

    public AcceptContratacionCommandHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ILogger<AcceptContratacionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(AcceptContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Accepting contratacion with ID: {DetalleId}", request.DetalleId);

        // Buscar contratación
        var contratacion = await _unitOfWork.DetallesContrataciones
            .GetByIdAsync(request.DetalleId, cancellationToken);

        if (contratacion == null)
        {
            _logger.LogWarning("Contratacion not found with ID: {DetalleId}", request.DetalleId);
            throw new NotFoundException(nameof(DetalleContratacion), request.DetalleId);
        }

        if (!contratacion.ContratacionId.HasValue)
        {
            throw new InvalidOperationException("La contratación no está vinculada a una contratación temporal válida");
        }

        var empleadoTemporal = await _context.Set<EmpleadoTemporal>()
            .AsNoTracking()
            .FirstOrDefaultAsync(et => et.ContratacionId == contratacion.ContratacionId.Value, cancellationToken);

        if (empleadoTemporal == null || !string.Equals(empleadoTemporal.UserId, request.UserId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Unauthorized accept attempt for detalle {DetalleId} by user {UserId}",
                request.DetalleId,
                request.UserId);
            throw new UnauthorizedAccessException("No autorizado para aceptar esta contratación");
        }

        try
        {
            // Llamar método del Domain (valida estado)
            contratacion.Aceptar();

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Contratacion {DetalleId} accepted successfully. Amount: {Monto}",
                contratacion.DetalleId,
                contratacion.MontoAcordado);

            return Unit.Value;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Cannot accept contratacion {DetalleId}. Current status: {Status}",
                request.DetalleId,
                contratacion.ObtenerNombreEstado());
            throw;
        }
    }
}
