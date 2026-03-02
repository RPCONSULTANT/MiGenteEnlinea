using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Contrataciones;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;

public class CreateDetalleContratacionCommandHandler : IRequestHandler<CreateDetalleContratacionCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateDetalleContratacionCommandHandler> _logger;

    public CreateDetalleContratacionCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateDetalleContratacionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(CreateDetalleContratacionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating DetalleContratacion for ContratacionId: {ContratacionId}",
            request.ContratacionId);

        var detalle = DetalleContratacion.Crear(
            descripcionCorta: request.DescripcionCorta ?? string.Empty,
            fechaInicio: DateOnly.FromDateTime(request.FechaInicio!.Value),
            fechaFinal: DateOnly.FromDateTime(request.FechaFinal!.Value),
            montoAcordado: request.MontoAcordado ?? 0m,
            descripcionAmpliada: request.DescripcionAmpliada,
            esquemaPagos: request.EsquemaPagos,
            contratacionId: request.ContratacionId);

        await _context.DetalleContrataciones.AddAsync(detalle, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (request.Estatus.HasValue && request.Estatus.Value != 1)
        {
            await _context.DetalleContrataciones
                .Where(x => x.DetalleId == detalle.DetalleId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Estatus, request.Estatus.Value), cancellationToken);
        }

        var detalleId = detalle.DetalleId;

        _logger.LogInformation(
            "DetalleContratacion created successfully. DetalleId: {DetalleId}",
            detalleId);

        return detalleId;
    }
}
