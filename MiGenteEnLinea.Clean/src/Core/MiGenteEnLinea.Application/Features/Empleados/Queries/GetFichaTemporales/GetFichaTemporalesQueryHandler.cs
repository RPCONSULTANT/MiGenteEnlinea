using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetFichaTemporales;

public class GetFichaTemporalesQueryHandler : IRequestHandler<GetFichaTemporalesQuery, EmpleadoTemporalDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetFichaTemporalesQueryHandler> _logger;

    public GetFichaTemporalesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetFichaTemporalesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmpleadoTemporalDto?> Handle(GetFichaTemporalesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo ficha temporal: ContratacionId={ContratacionId}, UserId={UserId}",
            request.ContratacionId,
            request.UserId);

        var result = await _context.Set<Domain.Entities.Empleados.EmpleadoTemporal>()
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.ContratacionId == request.ContratacionId)
            .Select(e => new EmpleadoTemporalDto
            {
                ContratacionId = e.ContratacionId,
                UserId = e.UserId,
                FechaRegistro = e.FechaRegistro,
                Tipo = e.Tipo,
                NombreComercial = e.NombreComercial,
                Rnc = e.Rnc,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Identificacion = e.Identificacion,
                Telefono1 = e.Telefono1,
                Direccion = e.Direccion,
                Detalle = _context.DetalleContrataciones
                    .Where(d => d.ContratacionId == e.ContratacionId)
                    .Select(d => new DetalleContratacionDto
                    {
                        DetalleId = d.DetalleId,
                        ContratacionId = d.ContratacionId,
                        DescripcionCorta = d.DescripcionCorta,
                        DescripcionAmpliada = d.DescripcionAmpliada,
                        FechaInicio = d.FechaInicio,
                        FechaFinal = d.FechaFinal,
                        MontoAcordado = d.MontoAcordado,
                        EsquemaPagos = d.EsquemaPagos,
                        Estatus = d.Estatus,
                        Calificado = d.Calificado,
                        CalificacionId = d.CalificacionId
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
        {
            _logger.LogWarning(
                "No se encontró ficha temporal con ContratacionId={ContratacionId} y UserId={UserId}",
                request.ContratacionId,
                request.UserId);
        }

        return result;
    }
}
