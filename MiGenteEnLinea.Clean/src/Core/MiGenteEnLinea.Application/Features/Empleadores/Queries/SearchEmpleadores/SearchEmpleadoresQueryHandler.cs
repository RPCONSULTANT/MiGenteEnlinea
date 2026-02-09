using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleadores.DTOs;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.SearchEmpleadores;

/// <summary>
/// Handler: Busca empleadores con paginación y filtros
/// </summary>
public sealed class SearchEmpleadoresQueryHandler : IRequestHandler<SearchEmpleadoresQuery, SearchEmpleadoresResult>
{
    private readonly IEmpleadorRepository _empleadorRepository;
    private readonly ILogger<SearchEmpleadoresQueryHandler> _logger;

    public SearchEmpleadoresQueryHandler(
        IEmpleadorRepository empleadorRepository,
        ILogger<SearchEmpleadoresQueryHandler> logger)
    {
        _empleadorRepository = empleadorRepository;
        _logger = logger;
    }

    public async Task<SearchEmpleadoresResult> Handle(SearchEmpleadoresQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Búsqueda de empleadores. SearchTerm: {SearchTerm}, SoloActivos: {SoloActivos}, Sector: {Sector}, Provincia: {Provincia}, PageIndex: {PageIndex}, PageSize: {PageSize}",
            request.SearchTerm ?? "N/A", request.SoloActivos, request.Sector ?? "N/A", request.Provincia ?? "N/A", request.PageIndex, request.PageSize);

        // Usar método SearchProjectedAsync del repositorio
        var (empleadores, totalRecords) = await _empleadorRepository.SearchProjectedAsync(
            request.SearchTerm,
            request.SoloActivos,
            request.Sector,
            request.Provincia,
            request.PageIndex,
            request.PageSize,
            e => new EmpleadorDto
            {
                EmpleadorId = e.Id,
                UserId = e.UserId,
                FechaPublicacion = e.FechaPublicacion,
                Habilidades = e.Habilidades,
                Experiencia = e.Experiencia,
                Descripcion = e.Descripcion,
                TieneFoto = e.Foto != null && e.Foto.Length > 0,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            },
            cancellationToken);

        _logger.LogInformation(
            "Búsqueda completada. Registros encontrados: {TotalRecords}, Página actual: {PageIndex}/{TotalPages}",
            totalRecords, request.PageIndex, (int)Math.Ceiling((double)totalRecords / request.PageSize));

        return new SearchEmpleadoresResult
        {
            Empleadores = empleadores.ToList(),
            TotalRecords = totalRecords,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}
