using System.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        await EnsureContactoSolicitudesTableAsync();

        await using var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // Regla anti-duplicado: una pendiente por contratista-empleador
        await using (var duplicateCmd = connection.CreateCommand())
        {
            duplicateCmd.CommandText = @"
                SELECT COUNT(1)
                FROM ContactoSolicitudes
                WHERE contratistaUserId = @contratistaUserId
                  AND empleadorId = @empleadorId
                  AND estatus = 'Pendiente';";

            AddParam(duplicateCmd, "@contratistaUserId", contratistaUserId);
            AddParam(duplicateCmd, "@empleadorId", request.EmpleadorId);

            var count = Convert.ToInt32(await duplicateCmd.ExecuteScalarAsync());
            if (count > 0)
            {
                return Conflict(new
                {
                    code = "duplicate_pending_request",
                    message = "Ya existe una solicitud de contacto pendiente para este empleador.",
                    correlationId
                });
            }
        }

        int solicitudId;
        await using (var insertCmd = connection.CreateCommand())
        {
            insertCmd.CommandText = @"
                INSERT INTO ContactoSolicitudes
                    (contratistaUserId, empleadorId, mensaje, canalPreferido, estatus, createdAt, updatedAt)
                VALUES
                    (@contratistaUserId, @empleadorId, @mensaje, @canalPreferido, 'Pendiente', GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            AddParam(insertCmd, "@contratistaUserId", contratistaUserId);
            AddParam(insertCmd, "@empleadorId", request.EmpleadorId);
            AddParam(insertCmd, "@mensaje", request.Mensaje?.Trim());
            AddParam(insertCmd, "@canalPreferido", request.CanalPreferido?.Trim().ToLowerInvariant());

            solicitudId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
        }

        _logger.LogInformation(
            "CONTACT_REQUEST_CREATE_SUCCESS CorrelationId={CorrelationId} SolicitudId={SolicitudId} ContratistaUserId={ContratistaUserId} EmpleadorId={EmpleadorId}",
            correlationId,
            solicitudId,
            contratistaUserId,
            request.EmpleadorId);

        return Created($"/api/contactos/solicitudes/{solicitudId}", new
        {
            solicitudId,
            estatus = "Pendiente",
            correlationId
        });
    }

    private async Task EnsureContactoSolicitudesTableAsync()
    {
        var sql = @"
            IF OBJECT_ID('dbo.ContactoSolicitudes', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ContactoSolicitudes
                (
                    solicitudId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    contratistaUserId NVARCHAR(100) NOT NULL,
                    empleadorId INT NOT NULL,
                    mensaje NVARCHAR(500) NULL,
                    canalPreferido NVARCHAR(20) NULL,
                    estatus NVARCHAR(20) NOT NULL,
                    createdAt DATETIME NOT NULL,
                    updatedAt DATETIME NOT NULL
                );

                CREATE INDEX IX_ContactoSolicitudes_Contratista_Empleador_Estatus
                    ON dbo.ContactoSolicitudes (contratistaUserId, empleadorId, estatus);
            END";

        await _context.Database.ExecuteSqlRawAsync(sql);
    }

    private static void AddParam(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
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
