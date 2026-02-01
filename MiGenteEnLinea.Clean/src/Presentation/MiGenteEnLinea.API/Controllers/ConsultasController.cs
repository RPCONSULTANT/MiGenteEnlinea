using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiGenteEnLinea.Application.Features.Consultas.Commands.RegistrarConsulta;
using MiGenteEnLinea.Application.Features.Consultas.Queries.GetConsultasCount;

namespace MiGenteEnLinea.API.Controllers;

/// <summary>
/// Controller para tracking de consultas de perfil
/// Registra cuando un empleador visita el perfil de un contratista
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConsultasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConsultasController> _logger;

    public ConsultasController(
        IMediator mediator,
        ILogger<ConsultasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registrar una consulta de perfil
    /// </summary>
    /// <param name="command">Datos de la consulta</param>
    /// <returns>ID de la consulta registrada (0 si fue duplicado ignorado)</returns>
    /// <response code="200">Consulta registrada (o ignorada si es duplicado reciente)</response>
    /// <response code="401">Usuario no autenticado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegistrarConsulta([FromBody] RegistrarConsultaCommand command)
    {
        // Agregar IP del cliente
        var commandWithIp = command with { IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() };
        
        var consultaId = await _mediator.Send(commandWithIp);
        
        return Ok(new { consultaId, registrada = consultaId > 0 });
    }

    /// <summary>
    /// Obtener el número de consultas realizadas por un empleador
    /// </summary>
    /// <param name="userId">ID del empleador</param>
    /// <returns>Número de consultas</returns>
    /// <response code="200">Conteo de consultas</response>
    /// <response code="401">Usuario no autenticado</response>
    [HttpGet("count/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConsultasCount(string userId)
    {
        var query = new GetConsultasCountQuery { EmpleadorUserId = userId };
        var count = await _mediator.Send(query);
        
        return Ok(new { count });
    }
}
