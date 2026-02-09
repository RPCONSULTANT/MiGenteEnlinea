using MediatR;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Contratistas;

namespace MiGenteEnLinea.Application.Features.Contratistas.Queries.GetContratistaFotoById;

/// <summary>
/// Handler: Obtiene la foto de perfil de un contratista por ID
/// </summary>
public sealed class GetContratistaFotoByIdQueryHandler : IRequestHandler<GetContratistaFotoByIdQuery, byte[]?>
{
    private readonly IContratistaRepository _contratistaRepository;

    public GetContratistaFotoByIdQueryHandler(IContratistaRepository contratistaRepository)
    {
        _contratistaRepository = contratistaRepository;
    }

    public async Task<byte[]?> Handle(GetContratistaFotoByIdQuery request, CancellationToken cancellationToken)
    {
        var foto = await _contratistaRepository.GetByIdProjectedAsync(
            request.ContratistaId,
            c => c.Foto,
            cancellationToken);

        return foto;
    }
}
