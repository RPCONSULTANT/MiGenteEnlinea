using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Contrataciones.DTOs;
using MiGenteEnLinea.Domain.Entities.Contrataciones;
using MiGenteEnLinea.Domain.Entities.Empleados;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Queries.GetContrataciones;

/// <summary>
/// Handler para obtener lista de contrataciones con filtros.
/// </summary>
public class GetContratacionesQueryHandler : IRequestHandler<GetContratacionesQuery, List<ContratacionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetContratacionesQueryHandler> _logger;

    public GetContratacionesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetContratacionesQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ContratacionDto>> Handle(
        GetContratacionesQuery request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting contrataciones with filters - Status: {Estatus}, Page: {Page}, Size: {Size}",
            request.Estatus,
            request.PageNumber,
            request.PageSize);

        // Query base
        var query = _context.DetalleContrataciones.AsQueryable();

        // Aplicar filtros
        if (request.ContratacionId.HasValue)
        {
            query = query.Where(c => c.ContratacionId == request.ContratacionId.Value);
        }

        if (request.Estatus.HasValue)
        {
            query = query.Where(c => c.Estatus == request.Estatus.Value);
        }

        if (request.SoloPendientes == true)
        {
            query = query.Where(c => c.Estatus == 1); // Pendiente
        }

        if (request.SoloActivas == true)
        {
            query = query.Where(c => c.Estatus == 3); // En Progreso
        }

        if (request.SoloNoCalificadas == true)
        {
            query = query.Where(c => c.Calificado == false && c.Estatus == 4); // Completadas sin calificar
        }

        if (request.FechaInicioDesde.HasValue)
        {
            query = query.Where(c => c.FechaInicio >= request.FechaInicioDesde.Value);
        }

        if (request.FechaInicioHasta.HasValue)
        {
            query = query.Where(c => c.FechaInicio <= request.FechaInicioHasta.Value);
        }

        if (request.MontoMinimo.HasValue)
        {
            query = query.Where(c => c.MontoAcordado >= request.MontoMinimo.Value);
        }

        if (request.MontoMaximo.HasValue)
        {
            query = query.Where(c => c.MontoAcordado <= request.MontoMaximo.Value);
        }

        // Ordenar por fecha de inicio (más recientes primero)
        query = query.OrderByDescending(c => c.FechaInicio);

        // Paginación
        var skip = (request.PageNumber - 1) * request.PageSize;
        query = query.Skip(skip).Take(request.PageSize);

        // Ejecutar query
        var contrataciones = await query.ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} contrataciones",
            contrataciones.Count);

        // Mapear a DTOs
        var dtos = _mapper.Map<List<ContratacionDto>>(contrataciones);

        // ✅ ENRICH DTOs with contractor data (EmpleadoTemporal photo/identification)
        if (dtos.Any())
        {
            var empleadoTemporalIds = dtos
                .Where(d => d.ContratacionId.HasValue)
                .Select(d => d.ContratacionId.Value)
                .Distinct()
                .ToList();

            if (empleadoTemporalIds.Count > 0)
            {
                // Fetch EmpleadosTemporales with their contractor data
                var empleadosTemporales = await _context.Set<EmpleadoTemporal>()
                    .Where(et => empleadoTemporalIds.Contains(et.ContratacionId))
                    .ToListAsync(cancellationToken);

                var empleadoTemporalDict = empleadosTemporales.ToDictionary(et => et.ContratacionId);

                // Enrich each DTO with contractor info
                foreach (var dto in dtos.Where(d => d.ContratacionId.HasValue))
                {
                    if (empleadoTemporalDict.TryGetValue(dto.ContratacionId!.Value, out var empleadoTemporal))
                    {
                        dto.ContratistaIdentificacion = empleadoTemporal.Identificacion;
                        dto.ContratistaCompleteName = empleadoTemporal.ObtenerNombreCompleto();
                        dto.ContratistaFotoUrl = empleadoTemporal.Foto;

                        _logger.LogDebug(
                            "Enriched DTO {DetalleId} with contractor {Identificacion}",
                            dto.DetalleId,
                            empleadoTemporal.Identificacion);
                    }
                }
            }
        }

        return dtos;
    }
}
