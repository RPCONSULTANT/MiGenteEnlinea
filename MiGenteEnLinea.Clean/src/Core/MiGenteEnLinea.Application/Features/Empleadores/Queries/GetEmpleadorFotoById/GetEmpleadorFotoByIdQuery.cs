using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.GetEmpleadorFotoById;

/// <summary>
/// Query: obtiene la foto/logo de un empleador por ID.
/// </summary>
public sealed record GetEmpleadorFotoByIdQuery(int EmpleadorId) : IRequest<byte[]?>;
