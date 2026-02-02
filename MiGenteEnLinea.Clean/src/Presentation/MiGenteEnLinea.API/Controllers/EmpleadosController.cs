using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Commands.DesactivarEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Commands.AddRemuneracion;
using MiGenteEnLinea.Application.Common.Models;
using MiGenteEnLinea.Application.Features.Empleados.Commands.RemoveRemuneracion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.DeleteRemuneracion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateRemuneraciones;
using MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateRemuneraciones;
using MiGenteEnLinea.Application.Features.Empleados.Commands.DarDeBajaEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Commands.ReactivarEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CancelarTrabajo;
using MiGenteEnLinea.Application.Features.Empleados.Commands.EliminarRecibo;
using MiGenteEnLinea.Application.Features.Empleados.Commands.EliminarEmpleadoTemporal;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CalificarContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.ModificarCalificacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.ProcesarPago;
using MiGenteEnLinea.Application.Features.Empleados.Commands.AnularRecibo;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetEmpleadoById;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetEmpleadosByEmpleador;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetReciboById;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetRecibosByEmpleado;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetRemuneraciones;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetDeduccionesTss;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetReciboContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetPagosContrataciones;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetFichaTemporales;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetTodosLosTemporales;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetVistaContratacionTemporal;
using MiGenteEnLinea.Application.Features.Empleados.Queries.GetReciboHeaderByPagoId;
using MiGenteEnLinea.Application.Features.Empleados.Queries.ConsultarPadron;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Authentication.Queries.GetProfileById;

namespace MiGenteEnLinea.API.Controllers;

/// <summary>
/// Controller REST API para gestión completa de empleados permanentes.
/// Incluye CRUD, remuneraciones extras, procesamiento de nómina, consulta de Padrón Nacional, y generación de contratos PDF.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos los endpoints requieren autenticación JWT
[Produces("application/json")]
public class EmpleadosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmpleadosController> _logger;
    private readonly IPdfService _pdfService;

    public EmpleadosController(
        IMediator mediator, 
        ILogger<EmpleadosController> logger,
        IPdfService pdfService)
    {
        _mediator = mediator;
        _logger = logger;
        _pdfService = pdfService;
    }

    // ========================================
    // CRUD EMPLEADOS PERMANENTES
    // ========================================

    /// <summary>
    /// Crear nuevo empleado permanente.
    /// </summary>
    /// <param name="command">Datos del empleado a crear</param>
    /// <returns>ID del empleado creado</returns>
    /// <response code="201">Empleado creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> CreateEmpleado([FromBody] CreateEmpleadoCommand command)
    {
        _logger.LogInformation("Creando nuevo empleado: {Nombre} {Apellido}", command.Nombre, command.Apellido);

        // Asegurar que el UserId del comando es el usuario autenticado
        command = command with { UserId = GetUserId() };

        var empleadoId = await _mediator.Send(command);

        _logger.LogInformation("Empleado creado exitosamente. EmpleadoId: {EmpleadoId}", empleadoId);

        return CreatedAtAction(
            nameof(GetEmpleadoById),
            new { id = empleadoId },
            new { empleadoId });
    }

    /// <summary>
    /// Obtener empleado por ID.
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <returns>Datos completos del empleado</returns>
    /// <response code="200">Empleado encontrado</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmpleadoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmpleadoDetalleDto>> GetEmpleadoById(int id)
    {
        _logger.LogInformation("Obteniendo empleado por ID: {EmpleadoId}", id);

        var query = new GetEmpleadoByIdQuery(GetUserId(), id);

        var empleado = await _mediator.Send(query);

        return Ok(empleado);
    }

    /// <summary>
    /// Actualizar datos de empleado existente.
    /// </summary>
    /// <param name="id">ID del empleado a actualizar</param>
    /// <param name="command">Nuevos datos del empleado</param>
    /// <returns>No content</returns>
    /// <response code="204">Empleado actualizado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o ID no coincide</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmpleado(int id, [FromBody] UpdateEmpleadoCommand command)
    {
        if (id != command.EmpleadoId)
        {
            _logger.LogWarning("El ID del empleado en la URL ({UrlId}) no coincide con el del body ({BodyId})", id, command.EmpleadoId);
            return BadRequest(new { error = "El ID del empleado no coincide" });
        }

        _logger.LogInformation("Actualizando empleado: {EmpleadoId}", id);

        // Asegurar que el UserId del comando es el usuario autenticado
        command = command with { UserId = GetUserId() };

        await _mediator.Send(command);

        _logger.LogInformation("Empleado actualizado exitosamente: {EmpleadoId}", id);

        return NoContent();
    }

    /// <summary>
    /// Eliminar empleado (soft delete - marca como inactivo).
    /// </summary>
    /// <param name="id">ID del empleado a eliminar</param>
    /// <returns>No content</returns>
    /// <response code="204">Empleado eliminado exitosamente</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        _logger.LogInformation("Eliminando empleado (soft delete): {EmpleadoId}", id);

        var command = new DesactivarEmpleadoCommand
        {
            EmpleadoId = id,
            UserId = GetUserId()
        };

        await _mediator.Send(command);

        _logger.LogInformation("Empleado eliminado exitosamente: {EmpleadoId}", id);

        return NoContent();
    }

    /// <summary>
    /// Dar de baja a un empleado (actualiza estado, fecha de salida, motivo y prestaciones).
    /// Migrado desde: EmpleadosService.darDeBaja(int empleadoID, string userID, DateTime fechaBaja, decimal prestaciones, string motivo)
    /// </summary>
    /// <param name="empleadoId">ID del empleado a dar de baja</param>
    /// <param name="request">Datos de la baja (fecha, motivo, prestaciones)</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Empleado dado de baja exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPut("{empleadoId}/dar-de-baja")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> DarDeBajaEmpleado(int empleadoId, [FromBody] DarDeBajaRequest request)
    {
        _logger.LogInformation(
            "Dando de baja empleado: {EmpleadoId}, Fecha: {FechaBaja}, Motivo: {Motivo}",
            empleadoId,
            request.FechaBaja,
            request.Motivo);

        var command = new DarDeBajaEmpleadoCommand(
            empleadoId,
            GetUserId(),
            request.FechaBaja,
            request.Prestaciones,
            request.Motivo);

        var result = await _mediator.Send(command);

        _logger.LogInformation("Empleado dado de baja exitosamente: {EmpleadoId}", empleadoId);

        return Ok(result);
    }

    /// <summary>
    /// Reactivar un empleado previamente dado de baja.
    /// </summary>
    /// <param name="empleadoId">ID del empleado a reactivar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Empleado reactivado exitosamente</response>
    /// <response code="400">Datos inválidos o empleado ya activo</response>
    /// <response code="401">No autenticado</response>
    /// <response code="404">Empleado no encontrado</response>
    [HttpPut("{empleadoId}/reactivar")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> ReactivarEmpleado(int empleadoId)
    {
        _logger.LogInformation("Reactivando empleado: {EmpleadoId}", empleadoId);

        var command = new ReactivarEmpleadoCommand(empleadoId, GetUserId());
        var result = await _mediator.Send(command);

        _logger.LogInformation("Empleado reactivado exitosamente: {EmpleadoId}", empleadoId);

        return Ok(result);
    }

    /// <summary>
    /// Cancelar un trabajo temporal (establece estatus = 3 en DetalleContrataciones).
    /// Migrado desde: EmpleadosService.cancelarTrabajo(int contratacionID, int detalleID)
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="detalleId">ID del detalle a cancelar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Trabajo cancelado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPut("contrataciones/{contratacionId}/detalle/{detalleId}/cancelar")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> CancelarTrabajo(int contratacionId, int detalleId)
    {
        _logger.LogInformation(
            "Cancelando trabajo temporal: ContratacionId={ContratacionId}, DetalleId={DetalleId}",
            contratacionId,
            detalleId);

        var command = new CancelarTrabajoCommand(contratacionId, detalleId);
        var result = await _mediator.Send(command);

        _logger.LogInformation(
            "Trabajo temporal cancelado: ContratacionId={ContratacionId}, DetalleId={DetalleId}",
            contratacionId,
            detalleId);

        return Ok(result);
    }

    /// <summary>
    /// Eliminar un recibo de empleado (Header + Detalle).
    /// Migrado desde: EmpleadosService.eliminarReciboEmpleado(int pagoID)
    /// </summary>
    /// <param name="pagoId">ID del recibo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Recibo eliminado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("recibos-empleado/{pagoId}/eliminar")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> EliminarReciboEmpleado(int pagoId)
    {
        _logger.LogWarning("Eliminando recibo de empleado: PagoId={PagoId}", pagoId);

        var command = new EliminarReciboEmpleadoCommand(pagoId);
        var result = await _mediator.Send(command);

        _logger.LogInformation("Recibo de empleado eliminado: PagoId={PagoId}", pagoId);

        return Ok(result);
    }

    /// <summary>
    /// Eliminar un recibo de contratación (Header + Detalle).
    /// Migrado desde: EmpleadosService.eliminarReciboContratacion(int pagoID)
    /// </summary>
    /// <param name="pagoId">ID del recibo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Recibo eliminado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("recibos-contratacion/{pagoId}/eliminar")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> EliminarReciboContratacion(int pagoId)
    {
        _logger.LogWarning("Eliminando recibo de contratación: PagoId={PagoId}", pagoId);

        var command = new EliminarReciboContratacionCommand(pagoId);
        var result = await _mediator.Send(command);

        _logger.LogInformation("Recibo de contratación eliminado: PagoId={PagoId}", pagoId);

        return Ok(result);
    }

    /// <summary>
    /// Obtener un recibo de contratación con su detalle y empleado temporal.
    /// Migrado desde: EmpleadosService.GetContratacion_ReciboByPagoID(int pagoID)
    /// </summary>
    /// <param name="pagoId">ID del recibo a consultar</param>
    /// <returns>Recibo de contratación con detalles</returns>
    /// <response code="200">Recibo obtenido exitosamente</response>
    /// <response code="404">Recibo no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("recibos-contratacion/{pagoId}")]
    [ProducesResponseType(typeof(ReciboContratacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReciboContratacionDto>> GetReciboContratacion(int pagoId)
    {
        _logger.LogInformation("Consultando recibo de contratación: PagoId={PagoId}", pagoId);

        var query = new GetReciboContratacionQuery(pagoId);
        var recibo = await _mediator.Send(query);

        if (recibo == null)
        {
            _logger.LogWarning("Recibo de contratación no encontrado: PagoId={PagoId}", pagoId);
            return NotFound(new { message = "Recibo de contratación no encontrado" });
        }

        return Ok(recibo);
    }

    /// <summary>
    /// Eliminar un empleado temporal y todos sus recibos asociados (cascade delete).
    /// Migrado desde: EmpleadosService.eliminarEmpleadoTemporal(int contratacionID)
    /// </summary>
    /// <param name="contratacionId">ID del empleado temporal a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Empleado temporal eliminado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("temporales/{contratacionId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> EliminarEmpleadoTemporal(int contratacionId)
    {
        _logger.LogWarning("Eliminando empleado temporal: ContratacionId={ContratacionId}", contratacionId);

        var command = new EliminarEmpleadoTemporalCommand(contratacionId);
        var result = await _mediator.Send(command);

        _logger.LogInformation("Empleado temporal eliminado: ContratacionId={ContratacionId}", contratacionId);

        return Ok(result);
    }

    /// <summary>
    /// Obtener pagos de contrataciones por contratacionID y detalleID
    /// Migrado de: EmpleadosService.GetEmpleador_RecibosContratacionesByID
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="detalleId">ID del detalle</param>
    /// <returns>Lista de pagos de contrataciones</returns>
    /// <response code="200">Lista de pagos obtenida exitosamente</response>
    /// <response code="400">Parámetros inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("pagos-contrataciones")]
    [ProducesResponseType(typeof(List<PagoContratacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PagoContratacionDto>>> GetPagosContrataciones(
        [FromQuery] int contratacionId,
        [FromQuery] int detalleId)
    {
        var query = new GetPagosContratacionesQuery(contratacionId, detalleId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Crear un nuevo empleado temporal con su detalle de contratación
    /// Migrado de: EmpleadosService.nuevoTemporal
    /// </summary>
    /// <param name="command">Datos del empleado temporal y detalle de contratación</param>
    /// <returns>ContratacionId del empleado temporal creado</returns>
    /// <response code="200">Empleado temporal creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("temporales")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> CreateEmpleadoTemporal([FromBody] CreateEmpleadoTemporalCommand command)
    {
        var contratacionId = await _mediator.Send(command);
        return Ok(contratacionId);
    }

    /// <summary>
    /// Crear un nuevo detalle de contratación
    /// Migrado de: EmpleadosService.nuevaContratacionTemporal
    /// </summary>
    /// <param name="command">Datos del detalle de contratación</param>
    /// <returns>DetalleId del detalle creado</returns>
    /// <response code="200">Detalle de contratación creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("contrataciones/detalles")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> CreateDetalleContratacion([FromBody] CreateDetalleContratacionCommand command)
    {
        var detalleId = await _mediator.Send(command);
        return Ok(detalleId);
    }

    /// <summary>
    /// Actualiza un detalle de contratación existente.
    /// Migrado de: EmpleadosService.actualizarContratacion
    /// </summary>
    /// <param name="contratacionId">ID de la contratación a actualizar</param>
    /// <param name="command">Datos actualizados del detalle de contratación</param>
    /// <returns>True si se actualizó exitosamente, False si no se encontró</returns>
    /// <response code="200">Detalle de contratación actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("contrataciones/detalles/{contratacionId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> UpdateDetalleContratacion(int contratacionId, [FromBody] UpdateDetalleContratacionCommand command)
    {
        if (contratacionId != command.ContratacionId)
        {
            return BadRequest("El ContratacionId de la ruta no coincide con el del comando");
        }

        var success = await _mediator.Send(command);
        
        if (!success)
        {
            return NotFound($"No se encontró el detalle de contratación con ContratacionId: {contratacionId}");
        }

        return Ok(success);
    }

    /// <summary>
    /// Marca una contratación como calificada y asigna el ID de la calificación.
    /// Migrado de: EmpleadosService.calificarContratacion
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="calificacionId">ID de la calificación asignada</param>
    /// <returns>True si se calificó exitosamente, False si no se encontró</returns>
    /// <response code="200">Contratación calificada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    /// <response code="404">Contratación no encontrada</response>
    [HttpPut("contrataciones/{contratacionId}/calificar")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> CalificarContratacion(
        int contratacionId,
        [FromQuery] int calificacionId)
    {
        var command = new CalificarContratacionCommand
        {
            ContratacionId = contratacionId,
            CalificacionId = calificacionId
        };

        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound($"No se encontró la contratación con ID: {contratacionId}");
        }

        return Ok(success);
    }

    /// <summary>
    /// Modifica una calificación existente.
    /// Migrado de: EmpleadosService.modificarCalificacionDeContratacion
    /// </summary>
    /// <param name="calificacionId">ID de la calificación a modificar</param>
    /// <param name="command">Datos actualizados de la calificación</param>
    /// <returns>True si se modificó exitosamente, False si no se encontró</returns>
    /// <response code="200">Calificación modificada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    /// <response code="404">Calificación no encontrada</response>
    [HttpPut("calificaciones/{calificacionId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> ModificarCalificacion(
        int calificacionId,
        [FromBody] ModificarCalificacionCommand command)
    {
        if (calificacionId != command.CalificacionId)
        {
            return BadRequest("El CalificacionId de la ruta no coincide con el del comando");
        }

        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound($"No se encontró la calificación con ID: {calificacionId}");
        }

        return Ok(success);
    }

    /// <summary>
    /// Obtiene ficha de empleado temporal con detalles de contratación.
    /// Migrado de: EmpleadosService.obtenerFichaTemporales
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Ficha de empleado temporal con detalles</returns>
    /// <response code="200">Ficha obtenida exitosamente</response>
    /// <response code="404">No se encontró la ficha temporal</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("temporales/ficha")]
    [ProducesResponseType(typeof(EmpleadoTemporalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmpleadoTemporalDto>> GetFichaTemporales(
        [FromQuery] int contratacionId,
        [FromQuery] string userId)
    {
        var query = new GetFichaTemporalesQuery
        {
            ContratacionId = contratacionId,
            UserId = userId
        };

        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"No se encontró ficha temporal con ContratacionId={contratacionId} y UserId={userId}");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtener todos los empleados temporales de un usuario (con transformación de nombres).
    /// Migrado de: EmpleadosService.obtenerTodosLosTemporales (line 526).
    /// Aplica lógica de negocio:
    ///   - tipo==1: Nombre = Nombre + Apellido
    ///   - tipo==2: Nombre = NombreComercial, Identificacion = Rnc
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de empleados temporales con nombres transformados</returns>
    /// <response code="200">Lista obtenida exitosamente (puede ser vacía)</response>
    /// <response code="400">UserId no proporcionado</response>
    [HttpGet("temporales/todos")]
    [ProducesResponseType(typeof(List<EmpleadoTemporalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<EmpleadoTemporalDto>>> GetTodosLosTemporales(
        [FromQuery] string userId)
    {
        var query = new GetTodosLosTemporalesQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query);

        return Ok(result); // Empty list is valid, not 404
    }

    /// <summary>
    /// Obtener vista completa de contratación temporal.
    /// Migrado de: EmpleadosService.obtenerVistaTemporal (line 554).
    /// </summary>
    /// <param name="contratacionId">ID de la contratación</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Vista completa con información del contratista y proyecto</returns>
    /// <response code="200">Vista obtenida exitosamente</response>
    /// <response code="404">Vista no encontrada</response>
    [HttpGet("temporales/vista")]
    [ProducesResponseType(typeof(VistaContratacionTemporalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VistaContratacionTemporalDto>> GetVistaContratacionTemporal(
        [FromQuery] int contratacionId,
        [FromQuery] string userId)
    {
        var query = new GetVistaContratacionTemporalQuery
        {
            ContratacionId = contratacionId,
            UserId = userId
        };

        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"No se encontró vista temporal con ContratacionId={contratacionId} y UserId={userId}");
        }

        return Ok(result);
    }

    /// <summary>
    /// Method #21: Obtener recibo completo por pagoId con detalle y empleado
    /// Migrado de: EmpleadosService.GetEmpleador_ReciboByPagoID(int pagoID) - line 212
    /// </summary>
    /// <param name="pagoId">ID del pago (PK de Empleador_Recibos_Header)</param>
    /// <returns>Recibo completo con header, detalles y datos del empleado</returns>
    /// <response code="200">Recibo obtenido exitosamente</response>
    /// <response code="404">No se encontró el recibo</response>
    [HttpGet("recibos/completo/{pagoId}")]
    [ProducesResponseType(typeof(ReciboHeaderCompletoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReciboHeaderCompletoDto>> GetReciboHeaderByPagoId(int pagoId)
    {
        var query = new GetReciboHeaderByPagoIdQuery { PagoId = pagoId };
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"No se encontró el recibo con PagoId: {pagoId}");
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Obtener todos los empleados del empleador autenticado.
    /// Soporta paginación, filtrado y búsqueda.
    /// </summary>
    /// <param name="soloActivos">Si true, solo muestra empleados activos. Default: true</param>
    /// <param name="searchTerm">Término de búsqueda (nombre, apellido o cédula)</param>
    /// <param name="pageIndex">Número de página (base 1). Default: 1</param>
    /// <param name="pageSize">Tamaño de página. Default: 20</param>
    /// <returns>Lista paginada de empleados</returns>
    /// <response code="200">Lista de empleados obtenida exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<EmpleadoListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedList<EmpleadoListDto>>> GetEmpleados(
        [FromQuery] bool? soloActivos = true,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation(
            "Obteniendo empleados: SoloActivos={SoloActivos}, SearchTerm={SearchTerm}, Page={PageIndex}/{PageSize}",
            soloActivos, searchTerm, pageIndex, pageSize);

        var query = new GetEmpleadosByEmpleadorQuery
        {
            UserId = GetUserId(),
            SoloActivos = soloActivos ?? true,
            SearchTerm = searchTerm,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        _logger.LogInformation(
            "Empleados obtenidos: {Count} de {Total} registros",
            result.Items.Count,
            result.TotalCount);

        return Ok(result);
    }

    // ========================================
    // REMUNERACIONES EXTRAS
    // ========================================

    /// <summary>
    /// Agregar remuneración extra a un empleado.
    /// Máximo 3 remuneraciones por empleado (slots 1, 2, 3).
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="command">Datos de la remuneración</param>
    /// <returns>No content</returns>
    /// <response code="204">Remuneración agregada exitosamente</response>
    /// <response code="400">Datos inválidos o slots llenos</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("{id}/remuneraciones")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddRemuneracion(int id, [FromBody] AddRemuneracionCommand command)
    {
        if (id != command.EmpleadoId)
        {
            _logger.LogWarning("El ID del empleado en la URL ({UrlId}) no coincide con el del body ({BodyId})", id, command.EmpleadoId);
            return BadRequest(new { error = "El ID del empleado no coincide" });
        }

        _logger.LogInformation("Agregando remuneración a empleado {EmpleadoId}: {Descripcion}", id, command.Descripcion);

        command = command with { UserId = GetUserId() };
        await _mediator.Send(command);

        _logger.LogInformation("Remuneración agregada exitosamente a empleado: {EmpleadoId}", id);

        return NoContent();
    }

    /// <summary>
    /// Eliminar una remuneración extra específica.
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="slot">Número de slot a eliminar (1, 2 o 3)</param>
    /// <returns>No content</returns>
    /// <response code="204">Remuneración eliminada exitosamente</response>
    /// <response code="400">Slot inválido</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("{id}/remuneraciones/{slot}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveRemuneracion(int id, int slot)
    {
        _logger.LogInformation("Eliminando remuneración slot {Slot} de empleado: {EmpleadoId}", slot, id);

        var command = new RemoveRemuneracionCommand
        {
            EmpleadoId = id,
            Numero = slot,
            UserId = GetUserId()
        };

        await _mediator.Send(command);

        _logger.LogInformation("Remuneración eliminada exitosamente: EmpleadoId={EmpleadoId}, Slot={Slot}", id, slot);

        return NoContent();
    }

    /// <summary>
    /// Obtener todas las remuneraciones adicionales de un empleado.
    /// Migrado desde: EmpleadosService.obtenerRemuneraciones(userID, empleadoID)
    /// </summary>
    /// <param name="empleadoId">ID del empleado</param>
    /// <returns>Lista de remuneraciones adicionales</returns>
    /// <response code="200">Remuneraciones obtenidas exitosamente (puede ser lista vacía)</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("{empleadoId}/remuneraciones")]
    [ProducesResponseType(typeof(List<RemuneracionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<RemuneracionDto>>> GetRemuneraciones(int empleadoId)
    {
        _logger.LogInformation("Obteniendo remuneraciones del empleado: {EmpleadoId}", empleadoId);

        var query = new GetRemuneracionesQuery(GetUserId(), empleadoId);
        var remuneraciones = await _mediator.Send(query);

        _logger.LogInformation("Remuneraciones obtenidas: {Count} registros", remuneraciones.Count);

        return Ok(remuneraciones);
    }

    /// <summary>
    /// Eliminar una remuneración adicional de la tabla Remuneraciones.
    /// Migrado desde: EmpleadosService.quitarRemuneracion(userID, id)
    /// </summary>
    /// <param name="remuneracionId">ID de la remuneración a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">Remuneración eliminada exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("remuneraciones/{remuneracionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteRemuneracion(int remuneracionId)
    {
        _logger.LogInformation("Eliminando remuneración: {RemuneracionId}", remuneracionId);

        var command = new DeleteRemuneracionCommand(GetUserId(), remuneracionId);
        await _mediator.Send(command);

        _logger.LogInformation("Remuneración eliminada exitosamente: {RemuneracionId}", remuneracionId);

        return NoContent();
    }

    /// <summary>
    /// Crear múltiples remuneraciones en batch para un empleado.
    /// Migrado desde: EmpleadosService.guardarOtrasRemuneraciones(List<Remuneraciones> rem)
    /// </summary>
    /// <param name="empleadoId">ID del empleado</param>
    /// <param name="command">Lista de remuneraciones a crear</param>
    /// <returns>Confirmación de creación</returns>
    /// <response code="200">Remuneraciones creadas exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("{empleadoId}/remuneraciones/batch")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> CreateRemuneracionesBatch(int empleadoId, [FromBody] List<RemuneracionItemDto> remuneraciones)
    {
        _logger.LogInformation("Creando {Count} remuneraciones para empleado: {EmpleadoId}", remuneraciones.Count, empleadoId);

        var command = new CreateRemuneracionesCommand
        {
            UserId = GetUserId(),
            EmpleadoId = empleadoId,
            Remuneraciones = remuneraciones
        };
        
        var result = await _mediator.Send(command);

        _logger.LogInformation("Remuneraciones creadas exitosamente para empleado: {EmpleadoId}", empleadoId);

        return Ok(result);
    }

    /// <summary>
    /// Actualizar todas las remuneraciones de un empleado (elimina existentes y crea nuevas).
    /// Migrado desde: EmpleadosService.actualizarRemuneraciones(List<Remuneraciones> rem, int empleadoID)
    /// </summary>
    /// <param name="empleadoId">ID del empleado</param>
    /// <param name="command">Lista de nuevas remuneraciones</param>
    /// <returns>Confirmación de actualización</returns>
    /// <response code="200">Remuneraciones actualizadas exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
    [HttpPut("{empleadoId}/remuneraciones/batch")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> UpdateRemuneracionesBatch(int empleadoId, [FromBody] List<RemuneracionItemDto> remuneraciones)
    {
        _logger.LogInformation("Actualizando remuneraciones para empleado: {EmpleadoId} (creará {Count} nuevas)", empleadoId, remuneraciones.Count);

        var command = new UpdateRemuneracionesCommand
        {
            UserId = GetUserId(),
            EmpleadoId = empleadoId,
            Remuneraciones = remuneraciones
        };
        
        var result = await _mediator.Send(command);

        _logger.LogInformation("Remuneraciones actualizadas exitosamente para empleado: {EmpleadoId}", empleadoId);

        return Ok(result);
    }

    // ========================================
    // NÓMINA Y PAGOS
    // ========================================

    /// <summary>
    /// Procesar pago de nómina para un empleado.
    /// Calcula percepciones (salario + extras) y deducciones (TSS).
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="command">Datos del pago a procesar</param>
    /// <returns>ID del recibo generado</returns>
    /// <response code="201">Pago procesado exitosamente</response>
    /// <response code="400">Datos inválidos o empleado inactivo</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("{id}/nomina")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> ProcesarPago(int id, [FromBody] ProcesarPagoCommand? command)
    {
        // DEBUG: Log incoming request for troubleshooting
        _logger.LogInformation("=== ProcesarPago REQUEST ===");
        _logger.LogInformation("URL id: {UrlId}", id);
        _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            _logger.LogWarning("ModelState errors: {Errors}", string.Join("; ", errors));
            return BadRequest(new { error = "Datos inválidos", errors });
        }
        
        _logger.LogInformation("Command is null: {IsNull}", command == null);
        if (command != null)
        {
            _logger.LogInformation("Command.EmpleadoId: {EmpleadoId}", command.EmpleadoId);
            _logger.LogInformation("Command.TipoConcepto: {TipoConcepto}", command.TipoConcepto);
            _logger.LogInformation("Command.FechaPago: {FechaPago}", command.FechaPago);
            _logger.LogInformation("Command.EsFraccion: {EsFraccion}", command.EsFraccion);
            _logger.LogInformation("Command.AplicarTss: {AplicarTss}", command.AplicarTss);
        }
        _logger.LogInformation("===========================");
        
        if (command == null)
        {
            _logger.LogWarning("El comando es null - error de deserialización del body");
            return BadRequest(new { error = "El body de la solicitud no es válido" });
        }
        
        if (id != command.EmpleadoId)
        {
            _logger.LogWarning("El ID del empleado en la URL ({UrlId}) no coincide con el del body ({BodyId})", id, command.EmpleadoId);
            return BadRequest(new { error = "El ID del empleado no coincide" });
        }

        _logger.LogInformation(
            "Procesando pago para empleado {EmpleadoId}: TipoConcepto={TipoConcepto}, EsFraccion={EsFraccion}",
            id, command.TipoConcepto, command.EsFraccion);

        command = command with { UserId = GetUserId() };
        var pagoId = await _mediator.Send(command);

        _logger.LogInformation("Pago procesado exitosamente. PagoId: {PagoId}", pagoId);

        return CreatedAtAction(
            nameof(GetReciboById),
            new { pagoId },
            new { pagoId });
    }

    /// <summary>
    /// Obtener recibo de pago por ID.
    /// Incluye header y líneas de detalles (percepciones y deducciones).
    /// </summary>
    /// <param name="pagoId">ID del recibo</param>
    /// <returns>Recibo completo con detalles</returns>
    /// <response code="200">Recibo encontrado</response>
    /// <response code="404">Recibo no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("recibos/detalle/{pagoId}")]
    [ProducesResponseType(typeof(ReciboDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReciboDetalleDto>> GetReciboById(int pagoId)
    {
        _logger.LogInformation("Obteniendo recibo por ID: {PagoId}", pagoId);

        var query = new GetReciboByIdQuery
        {
            PagoId = pagoId,
            UserId = GetUserId()
        };

        var recibo = await _mediator.Send(query);

        return Ok(recibo);
    }

    /// <summary>
    /// Obtener todos los recibos de un empleado.
    /// Soporta filtrado por estado y paginación.
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="soloActivos">Si true, excluye recibos anulados. Default: true</param>
    /// <param name="pageIndex">Número de página (base 1). Default: 1</param>
    /// <param name="pageSize">Tamaño de página. Default: 20</param>
    /// <returns>Lista paginada de recibos</returns>
    /// <response code="200">Lista de recibos obtenida exitosamente</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("{id}/recibos")]
    [ProducesResponseType(typeof(GetRecibosResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetRecibosResult>> GetRecibosByEmpleado(
        int id,
        [FromQuery] bool soloActivos = true,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation(
            "Obteniendo recibos de empleado {EmpleadoId}: SoloActivos={SoloActivos}, Page={PageIndex}/{PageSize}",
            id, soloActivos, pageIndex, pageSize);

        var query = new GetRecibosByEmpleadoQuery
        {
            UserId = GetUserId(),
            EmpleadoId = id,
            SoloActivos = soloActivos,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        _logger.LogInformation(
            "Recibos obtenidos para empleado {EmpleadoId}: {Count} de {Total}",
            id, result.Recibos.Count, result.TotalRecords);

        return Ok(result);
    }

    /// <summary>
    /// Anular recibo de pago (soft delete).
    /// Marca el recibo como anulado (Estado = 3) sin eliminar datos.
    /// </summary>
    /// <param name="pagoId">ID del recibo a anular</param>
    /// <param name="request">Motivo de anulación (opcional)</param>
    /// <returns>No content</returns>
    /// <response code="204">Recibo anulado exitosamente</response>
    /// <response code="400">Recibo ya está anulado</response>
    /// <response code="404">Recibo no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpDelete("recibos/{pagoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AnularRecibo(int pagoId, [FromBody] AnularReciboRequest request)
    {
        _logger.LogInformation("Anulando recibo: {PagoId}, Motivo: {Motivo}", pagoId, request.MotivoAnulacion);

        var command = new AnularReciboCommand
        {
            PagoId = pagoId,
            UserId = GetUserId(),
            MotivoAnulacion = request.MotivoAnulacion
        };

        await _mediator.Send(command);

        _logger.LogInformation("Recibo anulado exitosamente: {PagoId}", pagoId);

        return NoContent();
    }

    // ========================================
    // UTILIDADES
    // ========================================

    /// <summary>
    /// Consultar cédula en el Padrón Nacional Dominicano.
    /// Útil para validar identidad al crear/actualizar empleados.
    /// </summary>
    /// <param name="cedula">Cédula de 11 dígitos (puede incluir guiones: XXX-XXXXXXX-X)</param>
    /// <returns>Datos del ciudadano si existe</returns>
    /// <response code="200">Cédula encontrada en el Padrón</response>
    /// <response code="404">Cédula no encontrada</response>
    /// <response code="400">Formato de cédula inválido</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("padron/{cedula}")]
    [ProducesResponseType(typeof(PadronResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PadronResultDto>> ConsultarPadron(string cedula)
    {
        _logger.LogInformation("Consultando Padrón Nacional para cédula: {Cedula}", cedula);

        var query = new ConsultarPadronQuery { Cedula = cedula };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("Cédula no encontrada en el Padrón Nacional: {Cedula}", cedula);
            return NotFound(new { message = "Cédula no encontrada en el Padrón Nacional" });
        }

        _logger.LogInformation("Información del Padrón obtenida: {Cedula} - {Nombre}", result.Cedula, result.NombreCompleto);

        return Ok(result);
    }

    // ========================================
    // CATÁLOGOS
    // ========================================

    /// <summary>
    /// Obtener catálogo de deducciones TSS (Tesorería de la Seguridad Social).
    /// Retorna todas las deducciones disponibles con sus porcentajes.
    /// Migrado desde: EmpleadosService.deducciones()
    /// </summary>
    /// <returns>Lista de deducciones TSS</returns>
    /// <response code="200">Catálogo obtenido exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("deducciones-tss")]
    [ProducesResponseType(typeof(List<DeduccionTssDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DeduccionTssDto>>> GetDeduccionesTss()
    {
        _logger.LogInformation("Obteniendo catálogo de deducciones TSS");

        var query = new GetDeduccionesTssQuery();
        var deducciones = await _mediator.Send(query);

        _logger.LogInformation("Deducciones TSS obtenidas: {Count}", deducciones.Count);

        return Ok(deducciones);
    }

    // ========================================
    // GENERACIÓN DE DOCUMENTOS PDF
    // ========================================

    /// <summary>
    /// Genera PDF del contrato de trabajo para un empleado.
    /// Migrado desde: PrintViewer.aspx.cs + fichaEmpleado.aspx.cs (imprimirContratoPersonaFisica)
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="tipoContrato">Tipo de contrato: "persona-fisica" o "empresa" (default: persona-fisica)</param>
    /// <returns>Archivo PDF del contrato de trabajo</returns>
    /// <response code="200">PDF generado exitosamente</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="401">No autenticado</response>
    /// <remarks>
    /// Ejemplo:
    ///     GET /api/empleados/101/contrato?tipoContrato=persona-fisica
    ///     GET /api/empleados/101/contrato?tipoContrato=empresa
    /// 
    /// El PDF incluye:
    /// - Datos del empleador (nombre, cédula/RNC, dirección)
    /// - Datos del empleado (nombre, cédula, dirección, puesto, salario)
    /// - Fecha de inicio del contrato
    /// - Cláusulas legales estándar según Código de Trabajo RD
    /// 
    /// Response headers:
    /// - Content-Type: application/pdf
    /// - Content-Disposition: attachment; filename="contrato-{id}.pdf"
    /// </remarks>
    [HttpGet("{id}/contrato")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerarContratoPdf(int id, [FromQuery] string tipoContrato = "persona-fisica")
    {
        _logger.LogInformation(
            "Generando contrato PDF para empleado: {EmpleadoId}, Tipo: {TipoContrato}", 
            id, tipoContrato);

        try
        {
            // 1. Obtener UserId del usuario autenticado
            var userId = GetUserId();
            
            // 2. Obtener datos del empleado
            var empleadoQuery = new GetEmpleadoByIdQuery(userId, id);
            var empleado = await _mediator.Send(empleadoQuery);
            
            if (empleado == null)
            {
                return NotFound(new { error = $"Empleado con ID {id} no encontrado" });
            }

            // 3. Obtener datos del empleador (perfil del usuario autenticado)
            var perfilQuery = new GetProfileByIdQuery { UserId = userId };
            var perfil = await _mediator.Send(perfilQuery);
            
            if (perfil == null)
            {
                return NotFound(new { error = "Perfil del empleador no encontrado" });
            }

            // 3. Determinar datos del empleador según tipo
            string empleadorNombre;
            string empleadorRnc;

            if (tipoContrato == "empresa" && !string.IsNullOrEmpty(perfil.NombreComercial))
            {
                // Tipo empresa: usar datos comerciales
                empleadorNombre = perfil.NombreComercial;
                empleadorRnc = perfil.Identificacion ?? "N/A";
            }
            else
            {
                // Tipo persona física: usar datos personales
                empleadorNombre = $"{perfil.Nombre} {perfil.Apellido}".Trim();
                empleadorRnc = perfil.Identificacion ?? "N/A";
            }

            // 4. Generar PDF del contrato
            // Convert DateOnly? to DateTime for PDF service
            DateTime fechaInicio = empleado.FechaInicio.HasValue 
                ? empleado.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) 
                : DateTime.Today;
                
            var pdfBytes = _pdfService.GenerarContratoTrabajo(
                empleadorNombre: empleadorNombre,
                empleadorRnc: empleadorRnc,
                empleadoNombre: $"{empleado.Nombre} {empleado.Apellido}".Trim(),
                empleadoCedula: empleado.Identificacion ?? "N/A",
                puesto: empleado.Posicion ?? "No especificado",
                salario: empleado.Salario,
                fechaInicio: fechaInicio
            );

            _logger.LogInformation(
                "Contrato PDF generado exitosamente para empleado {EmpleadoId}. Tamaño: {Size} bytes",
                id, pdfBytes.Length);

            return File(pdfBytes, "application/pdf", $"contrato-{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando contrato PDF para empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al generar el contrato PDF" });
        }
    }

    /// <summary>
    /// Genera PDF del recibo de descargo por terminación de contrato.
    /// Usado cuando un empleado es dado de baja.
    /// Migrado desde: PrintViewer.aspx.cs (ReciboDescargoPersonaFisica_Empleador1)
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <param name="tipoContrato">Tipo de contrato: "persona-fisica" o "empresa"</param>
    /// <returns>Archivo PDF del descargo</returns>
    /// <response code="200">PDF generado exitosamente</response>
    /// <response code="404">Empleado no encontrado</response>
    /// <response code="400">Empleado no está dado de baja</response>
    /// <remarks>
    /// Este documento es el recibo de descargo que firma el empleado
    /// al terminar su relación laboral, indicando que recibió todas
    /// sus prestaciones laborales.
    /// </remarks>
    [HttpGet("{id}/descargo")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerarDescargoPdf(int id, [FromQuery] string tipoContrato = "persona-fisica")
    {
        _logger.LogInformation(
            "Generando descargo PDF para empleado: {EmpleadoId}, Tipo: {TipoContrato}", 
            id, tipoContrato);

        try
        {
            // 1. Obtener UserId del usuario autenticado
            var userId = GetUserId();
            
            // 2. Obtener datos del empleado
            var empleadoQuery = new GetEmpleadoByIdQuery(userId, id);
            var empleado = await _mediator.Send(empleadoQuery);
            
            if (empleado == null)
            {
                return NotFound(new { error = $"Empleado con ID {id} no encontrado" });
            }

            // Verificar que el empleado está inactivo (dado de baja)
            if (empleado.Activo == true)
            {
                return BadRequest(new { error = "El empleado debe estar dado de baja para generar el descargo" });
            }

            // 3. Obtener datos del empleador
            var perfilQuery = new GetProfileByIdQuery { UserId = userId };
            var perfil = await _mediator.Send(perfilQuery);
            
            if (perfil == null)
            {
                return NotFound(new { error = "Perfil del empleador no encontrado" });
            }

            // 3. Preparar datos para el descargo
            string empleadorNombre;
            string empleadorRnc;

            if (tipoContrato == "empresa" && !string.IsNullOrEmpty(perfil.NombreComercial))
            {
                empleadorNombre = perfil.NombreComercial;
                empleadorRnc = perfil.Identificacion ?? "N/A";
            }
            else
            {
                empleadorNombre = $"{perfil.Nombre} {perfil.Apellido}".Trim();
                empleadorRnc = perfil.Identificacion ?? "N/A";
            }

            // 4. Generar HTML del descargo y convertir a PDF
            // Use Prestaciones if available (liquidation amount), otherwise use salary
            var montoPrestaciones = empleado.Prestaciones ?? empleado.Salario;
            
            var descargoHtml = GenerarDescargoHtml(
                empleadorNombre: empleadorNombre,
                empleadorRnc: empleadorRnc,
                empleadoNombre: $"{empleado.Nombre} {empleado.Apellido}".Trim(),
                empleadoCedula: empleado.Identificacion ?? "N/A",
                empleadoDireccion: empleado.Direccion ?? "N/A",
                montoPrestaciones: montoPrestaciones,
                fechaBaja: empleado.FechaSalida ?? DateTime.Today
            );

            var pdfBytes = _pdfService.ConvertHtmlToPdf(descargoHtml);

            _logger.LogInformation(
                "Descargo PDF generado exitosamente para empleado {EmpleadoId}. Tamaño: {Size} bytes",
                id, pdfBytes.Length);

            return File(pdfBytes, "application/pdf", $"descargo-{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando descargo PDF para empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al generar el descargo PDF" });
        }
    }

    /// <summary>
    /// Genera HTML template para el recibo de descargo.
    /// </summary>
    private static string GenerarDescargoHtml(
        string empleadorNombre,
        string empleadorRnc,
        string empleadoNombre,
        string empleadoCedula,
        string empleadoDireccion,
        decimal montoPrestaciones,
        DateTime fechaBaja)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Recibo de Descargo</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }}
        h1 {{ text-align: center; color: #333; }}
        .content {{ text-align: justify; margin-bottom: 20px; }}
        .firma {{ margin-top: 60px; text-align: center; }}
        .firma-linea {{ border-top: 1px solid #000; width: 250px; margin: 40px auto 5px; padding-top: 5px; }}
    </style>
</head>
<body>
    <h1>RECIBO DE DESCARGO</h1>
    
    <div class='content'>
        <p>
            Yo, <strong>{empleadoNombre}</strong>, mayor de edad, portador(a) de la cédula de identidad 
            número <strong>{empleadoCedula}</strong>, domiciliado(a) en <strong>{empleadoDireccion}</strong>,
            República Dominicana, por medio del presente documento declaro:
        </p>

        <p>
            Que he recibido de <strong>{empleadorNombre}</strong>, identificación/RNC: <strong>{empleadorRnc}</strong>, 
            la suma de <strong>RD$ {montoPrestaciones:N2}</strong> por concepto de:
        </p>

        <ul>
            <li>Vacaciones</li>
            <li>Salario de días laborados</li>
            <li>Preaviso</li>
            <li>Cesantía</li>
            <li>Bonificación proporcional</li>
            <li>Cualquier otro concepto vinculado al contrato de trabajo</li>
        </ul>

        <p>
            En consecuencia, otorgo el más amplio finiquito que en derecho proceda a mi ex empleador,
            declarando que no tengo nada más que reclamar por ningún concepto relacionado con la
            relación laboral que existió entre nosotros.
        </p>

        <p>
            Hecho y firmado en Santo Domingo, Distrito Nacional, República Dominicana,
            a los {fechaBaja.Day} días del mes de {fechaBaja:MMMM} del año {fechaBaja.Year}.
        </p>
    </div>

    <div class='firma'>
        <div class='firma-linea'>
            {empleadoNombre}<br>
            Cédula: {empleadoCedula}
        </div>
    </div>
</body>
</html>";
    }

    // ========================================
    // HELPERS PRIVADOS
    // ========================================

    /// <summary>
    /// Obtiene el UserId del usuario autenticado desde el token JWT.
    /// </summary>
    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("No se pudo obtener el UserId del token JWT. User Claims: {Claims}",
                string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
            throw new UnauthorizedAccessException("Usuario no autenticado o token inválido");
        }

        return userId;
    }
}

/// <summary>
/// Request para anular recibo.
/// </summary>
public record AnularReciboRequest
{
    /// <summary>
    /// Motivo de la anulación del recibo (opcional).
    /// Máximo 500 caracteres.
    /// </summary>
    public string? MotivoAnulacion { get; init; }
}

/// <summary>
/// Request para dar de baja a un empleado.
/// </summary>
public record DarDeBajaRequest
{
    /// <summary>
    /// Fecha de la baja del empleado.
    /// </summary>
    public DateTime FechaBaja { get; init; }

    /// <summary>
    /// Monto de prestaciones laborales a pagar.
    /// </summary>
    public decimal Prestaciones { get; init; }

    /// <summary>
    /// Motivo de la baja.
    /// Máximo 500 caracteres.
    /// </summary>
    public string Motivo { get; init; } = string.Empty;
}
