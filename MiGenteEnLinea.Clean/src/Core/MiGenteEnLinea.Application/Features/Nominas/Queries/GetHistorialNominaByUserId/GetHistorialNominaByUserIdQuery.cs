using MediatR;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;

namespace MiGenteEnLinea.Application.Features.Nominas.Queries.GetHistorialNominaByUserId;

/// <summary>
/// Query para obtener el histórico de nóminas procesadas de un empleador (usuario).
/// Retorna una lista paginada de nóminas con información de períodos, empleados y montos.
/// </summary>
public record GetHistorialNominaByUserIdQuery : IRequest<List<NominaHistorialDto>>
{
    /// <summary>
    /// ID del usuario (empleador)
    /// </summary>
    public int UserId { get; init; }

    /// <summary>
    /// Número de página (1-based)
    /// </summary>
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// Cantidad de registros por página
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filtro opcional de período (ej: "2025-01")
    /// </summary>
    public string? Periodo { get; init; }

    /// <summary>
    /// Filtro opcional de estado (Procesado, Parcial, Error)
    /// </summary>
    public int? Estado { get; init; }
}
