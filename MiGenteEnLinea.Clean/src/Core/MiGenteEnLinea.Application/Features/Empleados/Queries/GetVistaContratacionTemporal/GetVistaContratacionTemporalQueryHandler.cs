using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetVistaContratacionTemporal;

/// <summary>
/// Handler para GetVistaContratacionTemporalQuery
/// Obtiene vista completa de contratación temporal desde base de datos
/// </summary>
public class GetVistaContratacionTemporalQueryHandler : IRequestHandler<GetVistaContratacionTemporalQuery, VistaContratacionTemporalDto?>
{
    private readonly IApplicationDbContext _context;

    public GetVistaContratacionTemporalQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VistaContratacionTemporalDto?> Handle(
        GetVistaContratacionTemporalQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Set<Domain.ReadModels.VistaContratacionTemporal>()
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.ContratacionId == request.ContratacionId)
            .Select(v => new VistaContratacionTemporalDto
            {
                ContratacionId = v.ContratacionId,
                UserId = v.UserId,
                FechaRegistro = v.FechaRegistro,
                Tipo = v.Tipo,
                NombreComercial = v.NombreComercial,
                Rnc = v.Rnc,
                Identificacion = v.Identificacion,
                Nombre = v.Nombre,
                Apellido = v.Apellido,
                Alias = v.Alias,
                Direccion = v.Direccion,
                Provincia = v.Provincia,
                Municipio = v.Municipio,
                Telefono1 = v.Telefono1,
                Telefono2 = v.Telefono2,
                DetalleId = v.DetalleId,
                Expr1 = v.Expr1,
                DescripcionCorta = v.DescripcionCorta,
                DescripcionAmpliada = v.DescripcionAmpliada,
                FechaInicio = v.FechaInicio,
                FechaFinal = v.FechaFinal,
                MontoAcordado = v.MontoAcordado,
                EsquemaPagos = v.EsquemaPagos,
                Estatus = v.Estatus,
                ComposicionNombre = v.ComposicionNombre,
                ComposicionId = v.ComposicionId,
                Conocimientos = v.Conocimientos,
                Puntualidad = v.Puntualidad,
                Recomendacion = v.Recomendacion,
                Cumplimiento = v.Cumplimiento
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
