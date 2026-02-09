using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Contratistas.Common;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Contratistas;

namespace MiGenteEnLinea.Application.Features.Contratistas.Queries.GetContratistaById;

/// <summary>
/// Handler: Obtiene un contratista por su ID
/// </summary>
public class GetContratistaByIdQueryHandler : IRequestHandler<GetContratistaByIdQuery, ContratistaDto?>
{
    private readonly IContratistaRepository _contratistaRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetContratistaByIdQueryHandler> _logger;

    public GetContratistaByIdQueryHandler(
        IContratistaRepository contratistaRepository,
        IApplicationDbContext context,
        ILogger<GetContratistaByIdQueryHandler> logger)
    {
        _contratistaRepository = contratistaRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<ContratistaDto?> Handle(GetContratistaByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando contratista por ID: {ContratistaId}", request.ContratistaId);

        var contratista = await _contratistaRepository.GetByIdProjectedAsync(
            request.ContratistaId,
            c => new ContratistaDto
            {
                ContratistaId = c.Id,
                UserId = c.UserId,
                FechaIngreso = c.FechaIngreso,
                Titulo = c.Titulo,
                Tipo = c.Tipo,
                Identificacion = c.Identificacion,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                NombreCompleto = $"{c.Nombre} {c.Apellido}",
                Sector = c.Sector,
                Experiencia = c.Experiencia,
                Presentacion = c.Presentacion,
                Telefono1 = c.Telefono1,
                Whatsapp1 = c.Whatsapp1,
                Telefono2 = c.Telefono2,
                Whatsapp2 = c.Whatsapp2,
                Email = c.Email != null ? c.Email.Value : null,
                Activo = c.Activo,
                Provincia = c.Provincia,
                NivelNacional = c.NivelNacional,
                ImagenUrl = c.ImagenUrl,
                // Campos calculados
                TieneWhatsApp = (c.Telefono1 != null && c.Whatsapp1) || (c.Telefono2 != null && c.Whatsapp2),
                PerfilCompleto = !string.IsNullOrWhiteSpace(c.UserId) &&
                                 !string.IsNullOrWhiteSpace(c.Nombre) &&
                                 !string.IsNullOrWhiteSpace(c.Apellido) &&
                                 !string.IsNullOrWhiteSpace(c.Titulo) &&
                                 !string.IsNullOrWhiteSpace(c.Presentacion) &&
                                 !string.IsNullOrWhiteSpace(c.Telefono1) &&
                                 !string.IsNullOrWhiteSpace(c.Provincia),
                PuedeRecibirTrabajos = c.Activo &&
                                       !string.IsNullOrWhiteSpace(c.Telefono1) &&
                                       (!string.IsNullOrWhiteSpace(c.Presentacion) || !string.IsNullOrWhiteSpace(c.Titulo))
            },
            cancellationToken);

        if (contratista != null && !string.IsNullOrWhiteSpace(contratista.Identificacion))
        {
            var promedio = await _context.Calificaciones
                .AsNoTracking()
                .Where(c => c.ContratistaIdentificacion == contratista.Identificacion)
                .Select(c => (decimal?)(c.Puntualidad + c.Cumplimiento + c.Conocimientos + c.Recomendacion) / 4.0m)
                .DefaultIfEmpty(null)
                .AverageAsync(cancellationToken);

            contratista = contratista with { PromedioCalificacion = promedio };
        }

        if (contratista == null)
        {
            _logger.LogInformation("Contratista no encontrado para ID: {ContratistaId}", request.ContratistaId);
        }
        else
        {
            _logger.LogInformation("Contratista encontrado. UserId: {UserId}", contratista.UserId);
        }

        return contratista;
    }
}
