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
    private static readonly IReadOnlyDictionary<string, string[]> MunicipiosPorProvincia =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Azua"] = ["Azua de Compostela", "Estebanía", "Guayabal", "Las Charcas", "Las Yayas de Viajama", "Padre Las Casas", "Peralta", "Pueblo Viejo", "Sabana Yegua", "Tábara Arriba"],
            ["Bahoruco"] = ["Neiba", "Galván", "Los Ríos", "Tamayo", "Villa Jaragua"],
            ["Barahona"] = ["Barahona", "Cabral", "Enriquillo", "Fundación", "Jaquimeyes", "La Ciénaga", "Las Salinas", "Paraíso", "Polo", "Vicente Noble"],
            ["Dajabón"] = ["Dajabón", "El Pino", "Loma de Cabrera", "Partido", "Restauración"],
            ["Distrito Nacional"] = ["Santo Domingo de Guzmán"],
            ["Duarte"] = ["San Francisco de Macorís", "Arenoso", "Castillo", "Eugenio María de Hostos", "Las Guáranas", "Pimentel", "Villa Riva"],
            ["Elías Piña"] = ["Comendador", "Bánica", "El Llano", "Hondo Valle", "Juan Santiago", "Pedro Santana"],
            ["El Seibo"] = ["Santa Cruz de El Seibo", "Miches"],
            ["Espaillat"] = ["Moca", "Cayetano Germosén", "Gaspar Hernández", "Jamao al Norte"],
            ["Hato Mayor"] = ["Hato Mayor del Rey", "El Valle", "Sabana de la Mar"],
            ["Hermanas Mirabal"] = ["Salcedo", "Tenares", "Villa Tapia"],
            ["Independencia"] = ["Jimaní", "Cristóbal", "Duvergé", "La Descubierta", "Mella", "Postrer Río"],
            ["La Altagracia"] = ["Salvaleón de Higüey", "San Rafael del Yuma"],
            ["La Romana"] = ["La Romana", "Guaymate", "Villa Hermosa"],
            ["La Vega"] = ["Concepción de La Vega", "Constanza", "Jarabacoa", "Jima Abajo"],
            ["María Trinidad Sánchez"] = ["Nagua", "Cabrera", "El Factor", "Río San Juan"],
            ["Monseñor Nouel"] = ["Bonao", "Maimón", "Piedra Blanca"],
            ["Monte Cristi"] = ["San Fernando de Monte Cristi", "Castañuelas", "Guayubín", "Las Matas de Santa Cruz", "Pepillo Salcedo", "Villa Vásquez"],
            ["Monte Plata"] = ["Monte Plata", "Bayaguana", "Peralvillo", "Sabana Grande de Boyá", "Yamasá"],
            ["Pedernales"] = ["Pedernales", "Oviedo"],
            ["Peravia"] = ["Baní", "Matanzas", "Nizao"],
            ["Puerto Plata"] = ["San Felipe de Puerto Plata", "Altamira", "Guananico", "Imbert", "Los Hidalgos", "Luperón", "Sosúa", "Villa Isabela", "Villa Montellano"],
            ["Samaná"] = ["Santa Bárbara de Samaná", "Las Terrenas", "Sánchez"],
            ["San Cristóbal"] = ["San Cristóbal", "Bajos de Haina", "Cambita Garabitos", "Los Cacaos", "Sabana Grande de Palenque", "San Gregorio de Nigua", "Villa Altagracia", "Yaguate"],
            ["San José de Ocoa"] = ["San José de Ocoa", "Rancho Arriba", "Sabana Larga"],
            ["San Juan"] = ["San Juan de la Maguana", "Bohechío", "El Cercado", "Juan de Herrera", "Las Matas de Farfán", "Vallejuelo"],
            ["San Pedro de Macorís"] = ["San Pedro de Macorís", "Consuelo", "Guayacanes", "Quisqueya", "Ramón Santana", "San José de los Llanos"],
            ["Sánchez Ramírez"] = ["Cotuí", "Cevicos", "Fantino", "La Mata"],
            ["Santiago"] = ["Santiago de los Caballeros", "Bisonó", "Jánico", "Licey al Medio", "Puñal", "Sabana Iglesia", "San José de las Matas", "Tamboril", "Villa González"],
            ["Santiago Rodríguez"] = ["San Ignacio de Sabaneta", "Monción", "Villa Los Almácigos"],
            ["Santo Domingo"] = ["Santo Domingo Este", "Santo Domingo Norte", "Santo Domingo Oeste", "Boca Chica", "Los Alcarrizos", "Pedro Brand", "San Antonio de Guerra"],
            ["Valverde"] = ["Santa Cruz de Mao", "Esperanza", "Laguna Salada"]
        };

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

    /// <summary>
    /// Obtiene municipios por provincia.
    /// </summary>
    [HttpGet("municipios")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetMunicipios([FromQuery] string? provincia = null)
    {
        if (string.IsNullOrWhiteSpace(provincia))
        {
            var flattened = MunicipiosPorProvincia
                .OrderBy(x => x.Key)
                .SelectMany(x => x.Value.Select(nombre => new { Provincia = x.Key, Nombre = nombre }))
                .ToList();
            return Ok(flattened);
        }

        var key = provincia.Trim();
        if (!MunicipiosPorProvincia.TryGetValue(key, out var values))
        {
            return Ok(Array.Empty<object>());
        }

        var municipios = values
            .Select(nombre => new { Provincia = key, Nombre = nombre })
            .ToList();

        return Ok(municipios);
    }
}
