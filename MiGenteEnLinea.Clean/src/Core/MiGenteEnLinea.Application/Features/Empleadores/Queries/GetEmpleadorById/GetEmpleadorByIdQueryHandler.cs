using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleadores.DTOs;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.GetEmpleadorById;

/// <summary>
/// Handler: Obtiene un Empleador por EmpleadorId
/// </summary>
public sealed class GetEmpleadorByIdQueryHandler : IRequestHandler<GetEmpleadorByIdQuery, EmpleadorDto?>
{
    private readonly IEmpleadorRepository _empleadorRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetEmpleadorByIdQueryHandler> _logger;

    public GetEmpleadorByIdQueryHandler(
        IEmpleadorRepository empleadorRepository,
        IApplicationDbContext context,
        ILogger<GetEmpleadorByIdQueryHandler> logger)
    {
        _empleadorRepository = empleadorRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<EmpleadorDto?> Handle(GetEmpleadorByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando empleador por EmpleadorId: {EmpleadorId}", request.EmpleadorId);

        var empleador = await _empleadorRepository.GetByIdProjectedAsync(
            request.EmpleadorId,
            e => new EmpleadorDto
            {
                EmpleadorId = e.Id,
                UserId = e.UserId,
                FechaPublicacion = e.FechaPublicacion,
                Habilidades = e.Habilidades,
                Experiencia = e.Experiencia,
                Descripcion = e.Descripcion,
                TieneFoto = e.Foto != null && e.Foto.Length > 0,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            },
            cancellationToken);

        if (empleador == null)
        {
            _logger.LogWarning("Empleador no encontrado. EmpleadorId: {EmpleadorId}", request.EmpleadorId);
            return null;
        }

        if (empleador == null)
        {
            return null;
        }

        var perfil = await _context.VPerfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == empleador.UserId && p.Tipo == 1, cancellationToken);

        if (perfil == null)
        {
            return empleador;
        }

        var nombreCompleto = string.Join(" ", new[] { perfil.Nombre, perfil.Apellido }
            .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

        var rnc = perfil.TipoIdentificacion == 3 ? perfil.Identificacion : null;
        var cedula = perfil.TipoIdentificacion == 1 ? perfil.Identificacion : null;

        return new EmpleadorDto
        {
            EmpleadorId = empleador.EmpleadorId,
            UserId = empleador.UserId,
            FechaPublicacion = empleador.FechaPublicacion,
            Habilidades = empleador.Habilidades,
            Experiencia = empleador.Experiencia,
            Descripcion = empleador.Descripcion,
            TieneFoto = empleador.TieneFoto,
            CreatedAt = empleador.CreatedAt,
            UpdatedAt = empleador.UpdatedAt,
            Nombre = perfil.Nombre,
            Apellido = perfil.Apellido,
            NombreCompleto = string.IsNullOrWhiteSpace(nombreCompleto) ? null : nombreCompleto,
            NombreComercial = perfil.NombreComercial,
            Identificacion = perfil.Identificacion,
            Rnc = rnc,
            Cedula = cedula,
            Email = perfil.Email,
            Telefono1 = perfil.Telefono1,
            Telefono2 = perfil.Telefono2,
            Direccion = perfil.Direccion,
            FechaIngreso = perfil.FechaCreacion,
            Activo = true,
            TotalContrataciones = 0,
            PromedioCalificaciones = empleador.PromedioCalificaciones,
            Provincia = empleador.Provincia,
            Ciudad = empleador.Ciudad,
            Sector = empleador.Sector,
            Whatsapp1 = empleador.Whatsapp1,
            Whatsapp2 = empleador.Whatsapp2
        };
    }
}
