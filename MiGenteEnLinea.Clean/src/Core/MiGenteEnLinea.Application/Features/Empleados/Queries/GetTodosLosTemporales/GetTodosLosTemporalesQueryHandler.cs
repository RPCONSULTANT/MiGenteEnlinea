using MediatR;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetTodosLosTemporales;

/// <summary>
/// Handler para GetTodosLosTemporalesQuery
/// Obtiene todos los empleados temporales con transformación de nombres según tipo
/// </summary>
public class GetTodosLosTemporalesQueryHandler : IRequestHandler<GetTodosLosTemporalesQuery, List<EmpleadoTemporalDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodosLosTemporalesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpleadoTemporalDto>> Handle(
        GetTodosLosTemporalesQuery request,
        CancellationToken cancellationToken)
    {
        var empleadosTemporales = await _context.Set<Domain.Entities.Empleados.EmpleadoTemporal>()
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
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
            .ToListAsync(cancellationToken);

        foreach (var empleado in empleadosTemporales)
        {
            if (empleado.Tipo == 1)
            {
                empleado.Nombre = $"{empleado.Nombre} {empleado.Apellido}".Trim();
            }
            else if (empleado.Tipo == 2)
            {
                empleado.Nombre = empleado.NombreComercial;
                empleado.Identificacion = empleado.Rnc;
            }
        }

        return empleadosTemporales;
    }
}
