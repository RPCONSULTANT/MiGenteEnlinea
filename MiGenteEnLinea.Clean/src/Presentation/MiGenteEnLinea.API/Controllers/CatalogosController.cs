using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.API.Controllers;

/// <summary>
/// Controller para cat\u00e1logos y datos de referencia
/// Endpoints p\u00fablicos (sin autenticaci\u00f3n requerida) para provincias, sectores, servicios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class CatalogosController : ControllerBase
{
    private readonly MiGenteDbContext _context;
    private readonly ILogger<CatalogosController> _logger;

    public CatalogosController(MiGenteDbContext context, ILogger<CatalogosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la lista de todas las provincias de Rep\u00fablica Dominicana
    /// </summary>
    /// <returns>Lista de provincias ordenadas alfab\u00e9ticamente</returns>
    /// <response code="200">Lista de provincias retornada exitosamente</response>
    [HttpGet("provincias")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvincias()
    {
        _logger.LogInformation("Obteniendo cat\u00e1logo de provincias");

        var provincias = await _context.Provincias
            .OrderBy(p => p.Nombre)
            .Select(p => new 
            { 
                p.ProvinciaId, 
                p.Nombre 
            })
            .ToListAsync();

        _logger.LogInformation("{Count} provincias retornadas", provincias.Count);
        return Ok(provincias);
    }

    /// <summary>
    /// Obtiene la lista de sectores/industrias disponibles
    /// </summary>
    /// <returns>Lista de sectores ordenados</returns>
    /// <response code="200">Lista de sectores retornada exitosamente</response>
    [HttpGet("sectores")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSectores()
    {
        _logger.LogInformation("Obteniendo cat\u00e1logo de sectores");

        var sectores = await _context.Sectores
            .OrderBy(s => s.Orden)
            .ThenBy(s => s.Nombre)
            .Select(s => new 
            { 
                s.SectorId, 
                Sector = s.Nombre
            })
            .ToListAsync();

        _logger.LogInformation("{Count} sectores retornados", sectores.Count);
        return Ok(sectores);
    }

    /// <summary>
    /// Obtiene el cat\u00e1logo de servicios que pueden ofrecer los contratistas
    /// </summary>
    /// <returns>Lista de servicios ordenados alfab\u00e9ticamente</returns>
    /// <response code="200">Lista de servicios retornada exitosamente</response>
    [HttpGet("servicios")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServicios()
    {
        _logger.LogInformation("Obteniendo cat\u00e1logo de servicios");

        var servicios = await _context.Servicios
            .OrderBy(s => s.Descripcion)
            .Select(s => new 
            { 
                s.ServicioId, 
                s.Descripcion 
            })
            .ToListAsync();

        _logger.LogInformation("{Count} servicios retornados", servicios.Count);
        return Ok(servicios);
    }
}
