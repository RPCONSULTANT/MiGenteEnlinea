using MediatR;
using MiGenteEnLinea.Application.Features.Empleadores.DTOs;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.SearchEmpleadores;

/// <summary>
/// Query: Buscar empleadores con paginación y filtros
/// </summary>
/// <remarks>
/// LÓGICA DE NEGOCIO:
/// - Búsqueda case-insensitive en Habilidades, Experiencia, Descripcion
/// - Soporta paginación (PageIndex, PageSize)
/// - Filtros: SoloActivos, Sector, Provincia
/// - Retorna total de registros para paginación en frontend
/// </remarks>
public record SearchEmpleadoresQuery(
    string? SearchTerm = null,
    bool? SoloActivos = null,
    string? Sector = null,
    string? Provincia = null,
    int PageIndex = 1,
    int PageSize = 10
) : IRequest<SearchEmpleadoresResult>;

/// <summary>
/// Resultado de búsqueda con paginación
/// </summary>
public sealed class SearchEmpleadoresResult
{
    public List<EmpleadorDto> Empleadores { get; init; } = new();
    public int TotalRecords { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}
