using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Domain.Entities.Contactos;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactosController : ControllerBase
{
    private readonly MiGenteDbContext _context;
    private readonly ILogger<ContactosController> _logger;

    public ContactosController(MiGenteDbContext context, ILogger<ContactosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("solicitudes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearSolicitudContacto([FromBody] CrearSolicitudContactoRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;
        var contratistaUserId = GetUserId();

        if (request.EmpleadorId <= 0)
        {
            return BadRequest(new { code = "validation_error", message = "El empleadorId es requerido.", correlationId });
        }

        if (!string.IsNullOrWhiteSpace(request.Mensaje) && request.Mensaje.Length > 500)
        {
            return BadRequest(new { code = "validation_error", message = "El mensaje no puede exceder 500 caracteres.", correlationId });
        }

        if (!string.IsNullOrWhiteSpace(request.CanalPreferido))
        {
            var allowed = new[] { "whatsapp", "email", "telefono" };
            if (!allowed.Contains(request.CanalPreferido.Trim().ToLowerInvariant()))
            {
                return BadRequest(new { code = "validation_error", message = "Canal preferido inválido.", correlationId });
            }
        }

        // Regla anti-duplicado: una pendiente por contratista-empleador
        var existePendiente = await _context.ContactoSolicitudes
            .AsNoTracking()
            .AnyAsync(x =>
                x.ContratistaUserId == contratistaUserId &&
                x.EmpleadorId == request.EmpleadorId &&
                x.Estatus == "Pendiente");

        if (existePendiente)
        {
            return Conflict(new
            {
                code = "duplicate_pending_request",
                message = "Ya existe una solicitud de contacto pendiente para este empleador.",
                correlationId
            });
        }

        var solicitud = ContactoSolicitud.Crear(
            contratistaUserId,
            request.EmpleadorId,
            request.Mensaje,
            request.CanalPreferido);

        _context.ContactoSolicitudes.Add(solicitud);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "CONTACT_REQUEST_CREATE_SUCCESS CorrelationId={CorrelationId} SolicitudId={SolicitudId} ContratistaUserId={ContratistaUserId} EmpleadorId={EmpleadorId}",
            correlationId,
            solicitud.SolicitudId,
            contratistaUserId,
            request.EmpleadorId);

        return Created($"/api/contactos/solicitudes/{solicitud.SolicitudId}", new
        {
            solicitudId = solicitud.SolicitudId,
            estatus = "Pendiente",
            correlationId
        });
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
}

public sealed class CrearSolicitudContactoRequest
{
    public int EmpleadorId { get; init; }
    public string? Mensaje { get; init; }
    public string? CanalPreferido { get; init; }
}
