using MediatR;

namespace MiGenteEnLinea.Application.Features.Consultas.Queries.GetConsultasCount;

/// <summary>
/// Query para obtener el número de consultas de perfil realizadas por un empleador
/// </summary>
public record GetConsultasCountQuery : IRequest<int>
{
    /// <summary>
    /// UserId del empleador
    /// </summary>
    public required string EmpleadorUserId { get; init; }
}
