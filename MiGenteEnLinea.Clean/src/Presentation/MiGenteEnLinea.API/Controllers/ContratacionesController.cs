using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.AcceptContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.CancelContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.CancelarTrabajo;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.CompleteContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.CreateContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.EliminarEmpleadoTemporal;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.RejectContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Commands.StartContratacion;
using MiGenteEnLinea.Application.Features.Contrataciones.Queries.GetContratacionById;
using MiGenteEnLinea.Application.Features.Contrataciones.Queries.GetContrataciones;
using MiGenteEnLinea.Application.Features.Authentication.Queries.GetProfileById;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System;

namespace MiGenteEnLinea.API.Controllers;

/// <summary>
/// Controller para gestión de contrataciones entre empleadores y contratistas.
/// 
/// WORKFLOW DE CONTRATACIÓN:
/// 1. Empleador crea propuesta (POST /api/contrataciones) → Estado: Pendiente
/// 2. Empleador confirma aceptación (PUT /api/contrataciones/{id}/accept) → Estado: Aceptada
///    O rechaza (PUT /api/contrataciones/{id}/reject) → Estado: Rechazada
/// 3. Trabajo inicia (PUT /api/contrataciones/{id}/start) → Estado: En Progreso
/// 4. Trabajo completa (PUT /api/contrataciones/{id}/complete) → Estado: Completada
/// 5. Empleador califica (POST /api/calificaciones) → Calificado = true
/// 
/// ESTADOS:
/// - 1 = Pendiente (propuesta enviada)
/// - 2 = Aceptada (contratista aceptó)
/// - 3 = En Progreso (trabajo iniciado)
/// - 4 = Completada (trabajo finalizado)
/// - 5 = Cancelada (cancelada por cualquier razón)
/// - 6 = Rechazada (contratista rechazó)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContratacionesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContratacionesController> _logger;
    private readonly MiGenteDbContext _context;
    private readonly IPdfService _pdfService;

    public ContratacionesController(
        IMediator mediator,
        ILogger<ContratacionesController> logger,
        MiGenteDbContext context,
        IPdfService pdfService)
    {
        _mediator = mediator;
        _logger = logger;
        _context = context;
        _pdfService = pdfService;
    }

    /// <summary>
    /// Crea una nueva propuesta de contratación.
    /// </summary>
    /// <param name="command">Datos de la contratación</param>
    /// <returns>ID del detalle de contratación creado</returns>
    /// <response code="200">Contratación creada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> Create([FromBody] CreateContratacionCommand command)
    {
        var correlationId = HttpContext.TraceIdentifier;
        _logger.LogInformation("CREATE_CONTRATACION_START CorrelationId={CorrelationId}", correlationId);

        try
        {
            var userId = GetUserId();
            var safeCommand = command with { EmpleadorUserId = userId };
            var detalleId = await _mediator.Send(safeCommand);
            _logger.LogInformation(
                "CREATE_CONTRATACION_SUCCESS CorrelationId={CorrelationId} DetalleId={DetalleId}",
                correlationId,
                detalleId);
            return Ok(detalleId);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "CREATE_CONTRATACION_FAIL_UNAUTHORIZED CorrelationId={CorrelationId}", correlationId);
            return Unauthorized(new
            {
                code = "unauthorized",
                message = ex.Message,
                correlationId
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "CREATE_CONTRATACION_FAIL_ARGUMENT CorrelationId={CorrelationId}", correlationId);
            return BadRequest(new
            {
                code = "business_rule_error",
                message = ex.Message,
                details = new[] { ex.Message },
                correlationId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CREATE_CONTRATACION_FAIL_UNKNOWN CorrelationId={CorrelationId}", correlationId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = "internal_error",
                message = "Error interno procesando la contratación.",
                correlationId
            });
        }
    }

    /// <summary>
    /// Obtiene una contratación específica por ID.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <returns>Detalles de la contratación</returns>
    /// <response code="200">Contratación encontrada</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting contratacion {Id}", id);

        var query = new GetContratacionByIdQuery { DetalleId = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = $"Contratación con ID {id} no encontrada" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene lista de contrataciones con filtros opcionales.
    /// </summary>
    /// <param name="query">Filtros de búsqueda</param>
    /// <returns>Lista de contrataciones</returns>
    /// <response code="200">Lista de contrataciones (puede estar vacía)</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetContratacionesQuery query)
    {
        _logger.LogInformation("Getting contrataciones with filters");

        var scope = query.Scope;
        if (!IsAdminUser())
        {
            scope = "mine";
        }

        var safeQuery = query with
        {
            UserId = GetUserId(),
            Scope = string.IsNullOrWhiteSpace(scope) ? "mine" : scope
        };

        var result = await _mediator.Send(safeQuery);
        return Ok(result);
    }

    /// <summary>
    /// Empleador confirma la aceptación de una propuesta de contratación.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <returns>Confirmación de aceptación</returns>
    /// <response code="200">Contratación aceptada exitosamente</response>
    /// <response code="400">No se puede aceptar (estado inválido)</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("{id}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accept(int id)
    {
        _logger.LogInformation("Accepting contratacion {Id}", id);

        try
        {
            var command = new AcceptContratacionCommand { DetalleId = id, UserId = GetUserId() };
            await _mediator.Send(command);
            return Ok(new { message = "Contratación aceptada exitosamente" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized accept for contratacion {Id}", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot accept contratacion {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Contratista rechaza una propuesta de contratación.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <param name="command">Motivo del rechazo</param>
    /// <returns>Confirmación de rechazo</returns>
    /// <response code="200">Contratación rechazada exitosamente</response>
    /// <response code="400">No se puede rechazar (estado inválido o motivo vacío)</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectContratacionCommand command)
    {
        _logger.LogInformation("Rejecting contratacion {Id}", id);

        if (command.DetalleId != id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Contratación rechazada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reject contratacion {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Inicia el trabajo de una contratación aceptada.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <returns>Confirmación de inicio</returns>
    /// <response code="200">Trabajo iniciado exitosamente</response>
    /// <response code="400">No se puede iniciar (estado inválido)</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("{id}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start(int id)
    {
        _logger.LogInformation("Starting contratacion {Id}", id);

        try
        {
            var command = new StartContratacionCommand { DetalleId = id, UserId = GetUserId() };
            await _mediator.Send(command);
            return Ok(new { message = "Trabajo iniciado exitosamente" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized start for contratacion {Id}", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot start contratacion {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Marca una contratación como completada.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <returns>Confirmación de completado</returns>
    /// <response code="200">Trabajo completado exitosamente</response>
    /// <response code="400">No se puede completar (estado inválido)</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(int id)
    {
        _logger.LogInformation("Completing contratacion {Id}", id);

        try
        {
            var command = new CompleteContratacionCommand { DetalleId = id, UserId = GetUserId() };
            await _mediator.Send(command);
            return Ok(new { message = "Trabajo completado exitosamente" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized complete for contratacion {Id}", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot complete contratacion {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancela una contratación.
    /// </summary>
    /// <param name="id">ID del detalle de contratación</param>
    /// <param name="command">Motivo de cancelación</param>
    /// <returns>Confirmación de cancelación</returns>
    /// <response code="200">Contratación cancelada exitosamente</response>
    /// <response code="400">No se puede cancelar (estado Completada o motivo vacío)</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelContratacionCommand command)
    {
        _logger.LogInformation("Canceling contratacion {Id}", id);

        if (command.DetalleId != id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Contratación cancelada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot cancel contratacion {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene contrataciones pendientes (estado = Pendiente).
    /// </summary>
    /// <returns>Lista de contrataciones pendientes</returns>
    /// <response code="200">Lista de contrataciones pendientes</response>
    [HttpGet("pendientes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendientes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting pendientes contrataciones");

        var query = new GetContratacionesQuery 
        { 
            UserId = GetUserId(),
            Scope = "mine",
            SoloPendientes = true,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene contrataciones activas (estado = En Progreso).
    /// </summary>
    /// <returns>Lista de contrataciones activas</returns>
    /// <response code="200">Lista de contrataciones activas</response>
    [HttpGet("activas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivas([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting activas contrataciones");

        var query = new GetContratacionesQuery 
        { 
            UserId = GetUserId(),
            Scope = "mine",
            SoloActivas = true,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene contrataciones completadas sin calificar.
    /// </summary>
    /// <returns>Lista de contrataciones completadas sin calificar</returns>
    /// <response code="200">Lista de contrataciones sin calificar</response>
    [HttpGet("sin-calificar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSinCalificar([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting contrataciones sin calificar");

        var query = new GetContratacionesQuery 
        { 
            UserId = GetUserId(),
            Scope = "mine",
            SoloNoCalificadas = true,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Cancela un trabajo/contratación (GAP-006).
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="detalleId">ID del detalle de contratación</param>
    /// <returns>Resultado de la cancelación (siempre true por paridad Legacy)</returns>
    /// <response code="200">Trabajo cancelado exitosamente</response>
    /// <response code="400">Parámetros inválidos</response>
    /// <remarks>
    /// Endpoint implementado para GAP-006: CancelarTrabajo
    /// 
    /// LÓGICA LEGACY: EmpleadosService.cancelarTrabajo() (líneas 233-245)
    /// 
    /// COMPORTAMIENTO:
    /// - Busca DetalleContratacion por contratacionID + detalleID
    /// - Si existe: actualiza estatus (DDD usa estatus = 5 "Cancelada")
    /// - Si NO existe: no hace nada pero retorna true igual (paridad Legacy)
    /// - Siempre retorna true (no lanza excepción si no encuentra)
    /// 
    /// NOTA ARQUITECTURAL:
    /// - Legacy usaba estatus = 3 para "Cancelada"
    /// - DDD usa estatus = 5 (ESTADO_CANCELADA) mediante método Cancelar()
    /// - Ambos representan el mismo estado semántico: "Trabajo cancelado"
    /// 
    /// EJEMPLO REQUEST:
    /// 
    ///     POST /api/contrataciones/cancelar-trabajo?contratacionId=45&amp;detalleId=12
    /// 
    /// EJEMPLO RESPONSE:
    /// 
    ///     {
    ///       "success": true,
    ///       "message": "Trabajo cancelado exitosamente"
    ///     }
    /// 
    /// USO TÍPICO:
    /// - Empleador decide no continuar con un trabajo iniciado
    /// - Problemas durante ejecución que impiden completar
    /// - Cambios en requerimientos que invalidan el contrato
    /// </remarks>
    [HttpPost("cancelar-trabajo")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelarTrabajo(
        [FromQuery] int contratacionId,
        [FromQuery] int detalleId)
    {
        _logger.LogInformation(
            "Canceling work - ContractID: {ContratacionId}, DetailID: {DetalleId}",
            contratacionId,
            detalleId);

        var command = new CancelarTrabajoCommand
        {
            ContratacionId = contratacionId,
            DetalleId = detalleId
        };

        var success = await _mediator.Send(command);

        return Ok(new 
        { 
            success, 
            message = "Trabajo cancelado exitosamente" 
        });
    }

    /// <summary>
    /// Elimina un empleado temporal y sus datos relacionados (GAP-007).
    /// </summary>
    /// <param name="contratacionId">ID de la contratación temporal a eliminar</param>
    /// <returns>Resultado de la eliminación (siempre true por paridad Legacy)</returns>
    /// <response code="200">Empleado temporal eliminado exitosamente</response>
    /// <response code="400">Parámetros inválidos</response>
    /// <remarks>
    /// Endpoint implementado para GAP-007: EliminarEmpleadoTemporal
    /// 
    /// LÓGICA LEGACY: EmpleadosService.eliminarEmpleadoTemporal() (líneas 299-357)
    /// 
    /// COMPORTAMIENTO:
    /// - Busca EmpleadoTemporal por contratacionID
    /// - Si existe: elimina en cascada (recibos detalles → headers → empleado)
    /// - Si NO existe: no hace nada pero retorna true igual (paridad Legacy)
    /// - Siempre retorna true (no lanza excepción si no encuentra)
    /// 
    /// OPERACIONES DE ELIMINACIÓN (orden crítico):
    /// 1. Empleador_Recibos_Detalle_Contrataciones (nietos - detalles de recibos)
    /// 2. Empleador_Recibos_Header_Contrataciones (hijos - headers de recibos)
    /// 3. EmpleadosTemporales (root - empleado temporal)
    /// 
    /// NOTA ARQUITECTURAL:
    /// - Legacy: Múltiples DbContext con SaveChanges() separados (anti-pattern)
    /// - Clean: Transacción única con SaveChanges() al final (mejor práctica)
    /// - EF Core: DeleteBehavior.Restrict requiere cascade manual
    /// - DDD: No hay método Eliminar() en entidad → operación de infraestructura
    /// 
    /// EJEMPLO REQUEST:
    /// 
    ///     DELETE /api/contrataciones/empleado-temporal?contratacionId=123
    /// 
    /// EJEMPLO RESPONSE:
    /// 
    ///     {
    ///       "success": true,
    ///       "message": "Empleado temporal eliminado exitosamente"
    ///     }
    /// 
    /// USO TÍPICO:
    /// - Empleador decide eliminar una contratación temporal completa
    /// - Limpieza de registros temporales no utilizados
    /// - Cancelación total de una contratación con eliminación de historial
    /// 
    /// ADVERTENCIA:
    /// - Esta es una operación destructiva (hard delete, no soft delete)
    /// - Se eliminan TODOS los recibos asociados a la contratación
    /// - No se puede deshacer la operación
    /// - Usar con precaución en producción
    /// </remarks>
    [HttpDelete("empleado-temporal")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EliminarEmpleadoTemporal(
        [FromQuery] int contratacionId)
    {
        _logger.LogInformation(
            "Deleting EmpleadoTemporal - ContratacionId: {ContratacionId}",
            contratacionId);

        var command = new EliminarEmpleadoTemporalCommand
        {
            ContratacionId = contratacionId
        };

        var success = await _mediator.Send(command);

        return Ok(new 
        { 
            success, 
            message = "Empleado temporal eliminado exitosamente" 
        });
    }

    /// <summary>
    /// Genera el contrato PDF de una contratación temporal.
    /// Disponible desde estado Aceptada en adelante.
    /// </summary>
    [HttpGet("{detalleId}/contrato-pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetContratoTemporalPdf(int detalleId)
    {
        var correlationId = HttpContext.TraceIdentifier;
        var userId = GetUserId();

        var detalle = await _context.DetalleContrataciones
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DetalleId == detalleId);

        if (detalle == null)
        {
            return NotFound(new
            {
                code = "not_found",
                message = "No se encontró el detalle de contratación solicitado.",
                correlationId
            });
        }

        if (!detalle.ContratacionId.HasValue)
        {
            return Conflict(new
            {
                code = "invalid_state",
                message = "La contratación no tiene vínculo temporal para generar contrato.",
                correlationId
            });
        }

        // Estados permitidos: Aceptada(2), En Progreso(3), Completada(4)
        if (detalle.Estatus is < 2 or > 4)
        {
            return Conflict(new
            {
                code = "invalid_state",
                message = "El contrato solo está disponible para contrataciones aceptadas o en ejecución.",
                correlationId
            });
        }

        var temporal = await _context.EmpleadosTemporales
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ContratacionId == detalle.ContratacionId.Value);

        if (temporal == null)
        {
            return NotFound(new
            {
                code = "not_found",
                message = "No se encontró la contratación temporal asociada.",
                correlationId
            });
        }

        if (!string.Equals(temporal.UserId, userId, StringComparison.OrdinalIgnoreCase) && !IsAdminUser())
        {
            return Forbid();
        }

        var perfil = await _mediator.Send(new GetProfileByIdQuery { UserId = userId });
        if (perfil == null)
        {
            return NotFound(new
            {
                code = "not_found",
                message = "No se encontró el perfil del empleador autenticado.",
                correlationId
            });
        }

        var empleadorNombre = !string.IsNullOrWhiteSpace(perfil.NombreComercial)
            ? perfil.NombreComercial
            : $"{perfil.Nombre} {perfil.Apellido}".Trim();
        var empleadorIdentificacion = perfil.Identificacion ?? "N/A";

        var contratistaNombre = temporal.Tipo == 2
            ? (temporal.NombreComercial ?? "Contratista")
            : $"{temporal.Nombre} {temporal.Apellido}".Trim();
        var contratistaIdentificacion = temporal.Tipo == 2
            ? (temporal.Rnc ?? "N/A")
            : (temporal.Identificacion ?? "N/A");

        var fechaInicio = detalle.FechaInicio.ToDateTime(TimeOnly.MinValue);
        var pdfBytes = _pdfService.GenerarContratoTrabajo(
            empleadorNombre: empleadorNombre,
            empleadorRnc: empleadorIdentificacion,
            empleadoNombre: string.IsNullOrWhiteSpace(contratistaNombre) ? "Contratista" : contratistaNombre,
            empleadoCedula: contratistaIdentificacion,
            puesto: detalle.DescripcionCorta,
            salario: detalle.MontoAcordado,
            fechaInicio: fechaInicio);

        _logger.LogInformation(
            "TEMP_CONTRACT_PDF_SUCCESS CorrelationId={CorrelationId} DetalleId={DetalleId} UserId={UserId}",
            correlationId,
            detalleId,
            userId);

        return File(pdfBytes, "application/pdf", $"contrato-temporal-{detalleId}.pdf");
    }

    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado");
        }

        return userId;
    }

    private bool IsAdminUser()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        return roles.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }
}
