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

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.StartContratacion;

public class StartContratacionCommandHandler : IRequestHandler<StartContratacionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<StartContratacionCommandHandler> _logger;

    public StartContratacionCommandHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ILogger<StartContratacionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(StartContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting contratacion {DetalleId}", request.DetalleId);

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
                "Unauthorized start attempt for detalle {DetalleId} by user {UserId}",
                request.DetalleId,
                request.UserId);
            throw new UnauthorizedAccessException("No autorizado para iniciar esta contratación");
        }

        try
        {
            contratacion.IniciarTrabajo();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Contratacion {DetalleId} started successfully at {FechaInicio}",
                contratacion.DetalleId,
                DateTime.Now);

            return Unit.Value;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot start contratacion {DetalleId}", request.DetalleId);
            throw;
        }
    }
}
