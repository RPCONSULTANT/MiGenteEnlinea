using MediatR;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.GetEmpleadorFotoById;

/// <summary>
/// Handler: obtiene la foto/logo de un empleador por ID.
/// </summary>
public sealed class GetEmpleadorFotoByIdQueryHandler : IRequestHandler<GetEmpleadorFotoByIdQuery, byte[]?>
{
    private readonly IEmpleadorRepository _empleadorRepository;

    public GetEmpleadorFotoByIdQueryHandler(IEmpleadorRepository empleadorRepository)
    {
        _empleadorRepository = empleadorRepository;
    }

    public async Task<byte[]?> Handle(GetEmpleadorFotoByIdQuery request, CancellationToken cancellationToken)
    {
        var foto = await _empleadorRepository.GetByIdProjectedAsync<byte[]>(
            request.EmpleadorId,
            e => e.Foto!,
            cancellationToken);

        return foto != null && foto.Length > 0 ? foto : null;
    }
}
