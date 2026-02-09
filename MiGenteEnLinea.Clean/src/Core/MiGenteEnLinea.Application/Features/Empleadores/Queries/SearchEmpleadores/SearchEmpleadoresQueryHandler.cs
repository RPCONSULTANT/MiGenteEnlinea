using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleadores.DTOs;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;

namespace MiGenteEnLinea.Application.Features.Empleadores.Queries.SearchEmpleadores;

/// <summary>
/// Handler: Busca empleadores con paginación y filtros
/// </summary>
public sealed class SearchEmpleadoresQueryHandler : IRequestHandler<SearchEmpleadoresQuery, SearchEmpleadoresResult>
{
    private readonly IEmpleadorRepository _empleadorRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SearchEmpleadoresQueryHandler> _logger;

    public SearchEmpleadoresQueryHandler(
        IEmpleadorRepository empleadorRepository,
        IApplicationDbContext context,
        ILogger<SearchEmpleadoresQueryHandler> logger)
    {
        _empleadorRepository = empleadorRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<SearchEmpleadoresResult> Handle(SearchEmpleadoresQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Búsqueda de empleadores. SearchTerm: {SearchTerm}, SoloActivos: {SoloActivos}, Sector: {Sector}, Provincia: {Provincia}, PageIndex: {PageIndex}, PageSize: {PageSize}",
            request.SearchTerm ?? "N/A", request.SoloActivos, request.Sector ?? "N/A", request.Provincia ?? "N/A", request.PageIndex, request.PageSize);

        // Usar método SearchProjectedAsync del repositorio
        var (empleadores, totalRecords) = await _empleadorRepository.SearchProjectedAsync(
            request.SearchTerm,
            request.SoloActivos,
            request.Sector,
            request.Provincia,
            request.PageIndex,
            request.PageSize,
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

        var empleadoresList = empleadores.ToList();

        if (empleadoresList.Count > 0)
        {
            var userIds = empleadoresList
                .Select(e => e.UserId)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Distinct()
                .ToList();

            if (userIds.Count > 0)
            {
                var perfiles = await _context.VPerfiles
                    .AsNoTracking()
                    .Where(p => p.UserId != null && userIds.Contains(p.UserId) && p.Tipo == 1)
                    .ToListAsync(cancellationToken);

                var perfilesLookup = perfiles
                    .Where(p => !string.IsNullOrWhiteSpace(p.UserId))
                    .ToDictionary(p => p.UserId!, p => p);

                for (var i = 0; i < empleadoresList.Count; i++)
                {
                    var empleador = empleadoresList[i];
                    if (perfilesLookup.TryGetValue(empleador.UserId, out var perfil))
                    {
                        var nombreCompleto = string.Join(" ", new[] { perfil.Nombre, perfil.Apellido }
                            .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

                        var rnc = perfil.TipoIdentificacion == 3 ? perfil.Identificacion : null;
                        var cedula = perfil.TipoIdentificacion == 1 ? perfil.Identificacion : null;

                        empleadoresList[i] = new EmpleadorDto
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
            }
        }

        _logger.LogInformation(
            "Búsqueda completada. Registros encontrados: {TotalRecords}, Página actual: {PageIndex}/{TotalPages}",
            totalRecords, request.PageIndex, (int)Math.Ceiling((double)totalRecords / request.PageSize));

        return new SearchEmpleadoresResult
        {
            Empleadores = empleadoresList,
            TotalRecords = totalRecords,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}
