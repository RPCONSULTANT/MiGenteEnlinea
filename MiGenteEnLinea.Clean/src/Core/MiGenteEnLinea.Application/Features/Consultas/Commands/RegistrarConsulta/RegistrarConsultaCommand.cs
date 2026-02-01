using MediatR;

namespace MiGenteEnLinea.Application.Features.Consultas.Commands.RegistrarConsulta;

/// <summary>
/// Command para registrar una consulta de perfil
/// Se llama cuando un empleador visita el perfil de un contratista
/// </summary>
public record RegistrarConsultaCommand : IRequest<int>
{
    /// <summary>
    /// UserId del empleador que consulta el perfil
    /// </summary>
    public required string EmpleadorUserId { get; init; }

    /// <summary>
    /// Identificación del contratista cuyo perfil se consulta
    /// </summary>
    public required string ContratistaIdentificacion { get; init; }

    /// <summary>
    /// IP desde la cual se realiza la consulta (para auditoría)
    /// </summary>
    public string? IpAddress { get; init; }
}
