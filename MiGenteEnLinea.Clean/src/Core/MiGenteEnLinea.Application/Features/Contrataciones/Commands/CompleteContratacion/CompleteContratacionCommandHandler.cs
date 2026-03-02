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

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.CompleteContratacion;

public class CompleteContratacionCommandHandler : IRequestHandler<CompleteContratacionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CompleteContratacionCommandHandler> _logger;

    public CompleteContratacionCommandHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ILogger<CompleteContratacionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(CompleteContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing contratacion {DetalleId}", request.DetalleId);

        var contratacion = await _unitOfWork.DetallesContrataciones
            .GetByIdAsync(request.DetalleId, cancellationToken);

        if (contratacion == null)
            throw new NotFoundException(nameof(DetalleContratacion), request.DetalleId);

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
                "Unauthorized complete attempt for detalle {DetalleId} by user {UserId}",
                request.DetalleId,
                request.UserId);
            throw new UnauthorizedAccessException("No autorizado para completar esta contratación");
        }

        try
        {
            contratacion.Completar();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Contratacion {DetalleId} completed successfully. Amount: {Monto}",
                contratacion.DetalleId,
                contratacion.MontoAcordado);

            return Unit.Value;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot complete contratacion {DetalleId}", request.DetalleId);
            throw;
        }
    }
}
