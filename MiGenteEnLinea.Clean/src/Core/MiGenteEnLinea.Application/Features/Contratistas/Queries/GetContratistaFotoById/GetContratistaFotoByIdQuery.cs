using MediatR;

namespace MiGenteEnLinea.Application.Features.Contratistas.Queries.GetContratistaFotoById;

/// <summary>
/// Query: Obtiene la foto de perfil de un contratista por ID
/// </summary>
public sealed record GetContratistaFotoByIdQuery(int ContratistaId) : IRequest<byte[]?>;
