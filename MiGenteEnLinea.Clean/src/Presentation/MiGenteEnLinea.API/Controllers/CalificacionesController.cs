using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiGenteEnLinea.Application.Features.Calificaciones.Commands.CalificarPerfil;
using MiGenteEnLinea.Application.Features.Calificaciones.Commands.CreateCalificacion;
using MiGenteEnLinea.Application.Features.Calificaciones.DTOs;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificacionById;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificaciones;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificacionesByContratista;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetCalificacionesByEmpleador;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetPromedioCalificacion;
using MiGenteEnLinea.Application.Features.Calificaciones.Queries.GetTodasCalificaciones;

namespace MiGenteEnLinea.API.Controllers;

/// <summary>
/// Controller: Calificaciones y Reviews (4 dimensiones de evaluación)
/// Gestiona la creación y lectura de calificaciones
/// 
/// NOTA: Las calificaciones son INMUTABLES (no se pueden editar ni eliminar)
/// Dimensiones: Puntualidad, Cumplimiento, Conocimientos, Recomendación (1-5 cada una)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CalificacionesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CalificacionesController> _logger;

    public CalificacionesController(
        IMediator mediator,
        ILogger<CalificacionesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crear una nueva calificación (4 dimensiones)
    /// </summary>
    /// <param name="command">Datos de la calificación</param>
    /// <returns>ID de la calificación creada</returns>
    /// <response code="201">Calificación creada exitosamente</response>
    /// <response code="400">Datos inválidos o calificación duplicada</response>
    /// <response code="401">Usuario no autenticado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateCalificacionCommand command)
    {
        try
        {
            var calificacionId = await _mediator.Send(command);
            
            _logger.LogInformation(
                "Calificación creada: ID={CalificacionId}, Empleador={EmpleadorUserId}, Contratista={ContratistaIdentificacion}",
                calificacionId,
                command.EmpleadorUserId,
                command.ContratistaIdentificacion);

            return CreatedAtAction(
                nameof(GetById),
                new { id = calificacionId },
                new { calificacionId });
        }
        catch (InvalidOperationException ex)
        {
            // Calificación duplicada
            _logger.LogWarning(ex, "Error al crear calificación duplicada");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener una calificación por ID
    /// </summary>
    /// <param name="id">ID de la calificación</param>
    /// <returns>Detalles de la calificación con 4 dimensiones</returns>
    /// <response code="200">Calificación encontrada</response>
    /// <response code="404">Calificación no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CalificacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCalificacionByIdQuery(id);
        var calificacion = await _mediator.Send(query);

        if (calificacion == null)
        {
            return NotFound(new { message = $"Calificación con ID {id} no encontrada" });
        }

        return Ok(calificacion);
    }

    /// <summary>
    /// Obtener calificaciones de un contratista (paginadas)
    /// </summary>
    /// <param name="identificacion">RNC o Cédula del contratista</param>
    /// <param name="userId">Opcional: Filtrar por empleador que calificó</param>
    /// <param name="pageNumber">Número de página (default: 1)</param>
    /// <param name="pageSize">Tamaño de página (default: 10)</param>
    /// <param name="orderBy">Campo de ordenamiento: 'Fecha' (default)</param>
    /// <param name="orderDirection">Dirección: 'asc' o 'desc' (default)</param>
    /// <returns>Lista paginada de calificaciones</returns>
    /// <response code="200">Lista de calificaciones (puede estar vacía)</response>
    [HttpGet("contratista/{identificacion}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByContratista(
        string identificacion,
        [FromQuery] string? userId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? orderBy = null,
        [FromQuery] string? orderDirection = "desc")
    {
        var query = new GetCalificacionesByContratistaQuery
        {
            Identificacion = identificacion,
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection
        };

        var result = await _mediator.Send(query);

        return Ok(new
        {
            items = result.Items,
            pageIndex = result.PageIndex,
            totalPages = result.TotalPages,
            totalCount = result.TotalCount,
            pageSize = result.PageSize,
            hasPreviousPage = result.HasPreviousPage,
            hasNextPage = result.HasNextPage
        });
    }

    /// <summary>
    /// Obtener promedio y distribución de calificaciones
    /// </summary>
    /// <param name="identificacion">RNC o Cédula del contratista</param>
    /// <returns>Estadísticas de calificaciones (promedio basado en 4 dimensiones)</returns>
    /// <response code="200">Promedio calculado</response>
    /// <response code="404">No hay calificaciones para esta identificación</response>
    [HttpGet("promedio/{identificacion}")]
    [ProducesResponseType(typeof(PromedioCalificacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPromedio(string identificacion)
    {
        var query = new GetPromedioCalificacionQuery(identificacion);
        var promedio = await _mediator.Send(query);

        if (promedio == null)
        {
            return NotFound(new 
            { 
                message = $"No hay calificaciones para la identificación {identificacion}" 
            });
        }

        return Ok(promedio);
    }

    /// <summary>
    /// Obtener calificaciones hechas por un empleador
    /// </summary>
    /// <param name="userId">ID del empleador que realizó las calificaciones</param>
    /// <returns>Lista de calificaciones realizadas por el empleador</returns>
    /// <response code="200">Lista de calificaciones (puede estar vacía)</response>
    [HttpGet("por-empleador/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(List<CalificacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmpleador(string userId)
    {
        var query = new GetCalificacionesByEmpleadorQuery { UserId = userId };
        var calificaciones = await _mediator.Send(query);
        return Ok(calificaciones);
    }

    // ============================================
    // ENDPOINTS ADICIONALES - MIGRADOS DESDE CalificacionesService.cs
    // ============================================

    /// <summary>
    /// LOTE 6.0.2: Calificar perfil de contratista (Legacy calificarPerfil)
    /// </summary>
    /// <remarks>
    /// Migrado desde: CalificacionesService.calificarPerfil(Calificaciones cal)
    /// 
    /// Sample request:
    /// 
    ///     POST /api/calificaciones/calificar-perfil
    ///     {
    ///       "empleadorUserId": "12345678-1234-1234-1234-123456789012",
    ///       "contratistaIdentificacion": "40212345678",
    ///       "contratistaNombre": "Juan Pérez",
    ///       "puntualidad": 5,
    ///       "cumplimiento": 4,
    ///       "conocimientos": 5,
    ///       "recomendacion": 5
    ///     }
    /// </remarks>
    [HttpPost("calificar-perfil")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> CalificarPerfil([FromBody] CalificarPerfilCommand command)
    {
        _logger.LogInformation(
            "POST /api/calificaciones/calificar-perfil - EmpleadorUserId: {EmpleadorUserId}, ContratistaIdentificacion: {ContratistaIdentificacion}",
            command.EmpleadorUserId,
            command.ContratistaIdentificacion);

        var calificacionId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = calificacionId },
            new { calificacionId });
    }

    /// <summary>
    /// LOTE 6.0.2: Obtener todas las calificaciones (Legacy getTodas)
    /// </summary>
    /// <remarks>
    /// Migrado desde: CalificacionesService.getTodas()
    /// 
    /// NOTA: Este endpoint usa CalificacionVistaDto (placeholder).
    /// La vista VCalificaciones aún no está implementada en Clean Architecture.
    /// </remarks>
    [HttpGet("todas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CalificacionVistaDto>>> GetTodasCalificaciones()
    {
        _logger.LogInformation("GET /api/calificaciones/todas");

        var query = new GetTodasCalificacionesQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// LOTE 6.0.2: Obtener calificaciones por identificación (Legacy getById)
    /// </summary>
    /// <remarks>
    /// Migrado desde: CalificacionesService.getById(string id, string userID = null)
    /// 
    /// Sample request:
    /// 
    ///     GET /api/calificaciones/legacy/40212345678
    ///     GET /api/calificaciones/legacy/40212345678?userId=12345678-1234-1234-1234-123456789012
    /// 
    /// NOTA: Este endpoint usa CalificacionVistaDto (placeholder).
    /// La vista VCalificaciones aún no está implementada en Clean Architecture.
    /// </remarks>
    [HttpGet("legacy/{identificacion}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CalificacionVistaDto>>> GetCalificacionesLegacy(
        string identificacion,
        [FromQuery] string? userId = null)
    {
        _logger.LogInformation(
            "GET /api/calificaciones/legacy/{Identificacion}?userId={UserId}",
            identificacion,
            userId);

        var query = new GetCalificacionesQuery(identificacion, userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
