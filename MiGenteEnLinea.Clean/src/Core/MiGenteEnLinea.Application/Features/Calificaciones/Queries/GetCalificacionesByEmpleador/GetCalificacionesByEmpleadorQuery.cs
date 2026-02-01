using MediatR;
using MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

namespace MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificacionesByEmpleador;

/// <summary>
/// Query para obtener calificaciones realizadas por un empleador
/// </summary>
public record GetCalificacionesByEmpleadorQuery : IRequest<List<CalificacionDto>>
{
    /// <summary>
    /// UserId del empleador que realizó las calificaciones
    /// </summary>
    public required string UserId { get; init; }
}
