using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Contratistas.Common;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Contratistas;

namespace MiGenteEnLinea.Application.Features.Contratistas.Queries.SearchContratistas;

/// <summary>
/// Handler: Busca contratistas con filtros y paginación
/// </summary>
public class SearchContratistasQueryHandler : IRequestHandler<SearchContratistasQuery, SearchContratistasResult>
{
    private readonly IContratistaRepository _contratistaRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SearchContratistasQueryHandler> _logger;

    public SearchContratistasQueryHandler(
        IContratistaRepository contratistaRepository,
        IApplicationDbContext context,
        ILogger<SearchContratistasQueryHandler> logger)
    {
        _contratistaRepository = contratistaRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<SearchContratistasResult> Handle(SearchContratistasQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Buscando contratistas. SearchTerm: {SearchTerm}, Provincia: {Provincia}, PageIndex: {PageIndex}",
            request.SearchTerm, request.Provincia, request.PageIndex);

        // Validar y ajustar PageSize
        var pageSize = request.PageSize;
        if (pageSize > 100) pageSize = 100;
        if (pageSize < 1) pageSize = 10;

        // Validar y ajustar PageIndex
        var pageIndex = request.PageIndex;
        if (pageIndex < 1) pageIndex = 1;

        // BUSCAR usando Repository con proyección DTO
        var (contratistas, totalRecords) = await _contratistaRepository.SearchProjectedAsync<ContratistaDto>(
            searchTerm: request.SearchTerm,
            provincia: request.Provincia,
            sector: request.Sector,
            experienciaMinima: request.ExperienciaMinima,
            soloActivos: request.SoloActivos,
            pageNumber: pageIndex,
            pageSize: pageSize,
            selector: c => new ContratistaDto
            {
                ContratistaId = c.Id,
                UserId = c.UserId,
                FechaIngreso = c.FechaIngreso,
                Titulo = c.Titulo,
                Tipo = c.Tipo,
                Identificacion = c.Identificacion,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                NombreCompleto = $"{c.Nombre} {c.Apellido}",
                Sector = c.Sector,
                Experiencia = c.Experiencia,
                Presentacion = c.Presentacion,
                Telefono1 = c.Telefono1,
                Whatsapp1 = c.Whatsapp1,
                Telefono2 = c.Telefono2,
                Whatsapp2 = c.Whatsapp2,
                Email = c.Email != null ? c.Email.Value : null,
                Activo = c.Activo,
                Provincia = c.Provincia,
                NivelNacional = c.NivelNacional,
                ImagenUrl = c.ImagenUrl,
                // Campos calculados
                TieneWhatsApp = (c.Telefono1 != null && c.Whatsapp1) || (c.Telefono2 != null && c.Whatsapp2),
                PerfilCompleto = !string.IsNullOrWhiteSpace(c.UserId) &&
                                 !string.IsNullOrWhiteSpace(c.Nombre) &&
                                 !string.IsNullOrWhiteSpace(c.Apellido) &&
                                 !string.IsNullOrWhiteSpace(c.Titulo) &&
                                 !string.IsNullOrWhiteSpace(c.Presentacion) &&
                                 !string.IsNullOrWhiteSpace(c.Telefono1) &&
                                 !string.IsNullOrWhiteSpace(c.Provincia),
                PuedeRecibirTrabajos = c.Activo &&
                                       !string.IsNullOrWhiteSpace(c.Telefono1) &&
                                       (!string.IsNullOrWhiteSpace(c.Presentacion) || !string.IsNullOrWhiteSpace(c.Titulo))
            },
            ct: cancellationToken);

        var contratistasList = contratistas.ToList();

        if (contratistasList.Count > 0)
        {
            var identifications = contratistasList
                .Select(c => c.Identificacion)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .ToList();

            if (identifications.Count > 0)
            {
                var promedioByIdentificacion = await _context.Calificaciones
                    .AsNoTracking()
                    .Where(c => identifications.Contains(c.ContratistaIdentificacion))
                    .GroupBy(c => c.ContratistaIdentificacion)
                    .Select(g => new
                    {
                        Identificacion = g.Key,
                        Promedio = g.Average(x => (x.Puntualidad + x.Cumplimiento + x.Conocimientos + x.Recomendacion) / 4.0m)
                    })
                    .ToListAsync(cancellationToken);

                var promedioLookup = promedioByIdentificacion
                    .Where(p => !string.IsNullOrWhiteSpace(p.Identificacion))
                    .ToDictionary(p => p.Identificacion!, p => p.Promedio);

                for (var i = 0; i < contratistasList.Count; i++)
                {
                    var contratista = contratistasList[i];
                    if (!string.IsNullOrWhiteSpace(contratista.Identificacion) &&
                        promedioLookup.TryGetValue(contratista.Identificacion, out var promedio))
                    {
                        contratistasList[i] = contratista with { PromedioCalificacion = promedio };
                    }
                }
            }
        }

        _logger.LogInformation(
            "Búsqueda completada. Total: {TotalRecords}, Página: {PageIndex}/{TotalPages}",
            totalRecords, pageIndex, (int)Math.Ceiling(totalRecords / (double)pageSize));

        return new SearchContratistasResult(contratistasList, totalRecords, pageIndex, pageSize);
    }
}
