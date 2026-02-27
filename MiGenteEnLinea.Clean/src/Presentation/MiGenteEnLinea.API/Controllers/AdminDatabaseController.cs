using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiGenteEnLinea.API.Configuration;
using MiGenteEnLinea.API.Services;
using MiGenteEnLinea.Infrastructure.Persistence.Seeding;

namespace MiGenteEnLinea.API.Controllers;

[ApiController]
[Route("api/admin/database")]
[Authorize(Roles = "Admin")]
public class AdminDatabaseController : ControllerBase
{
    private readonly IDatabaseInitializationService _databaseInitializationService;
    private readonly DatabaseSeedingSecurityOptions _securityOptions;
    private readonly ILogger<AdminDatabaseController> _logger;

    public AdminDatabaseController(
        IDatabaseInitializationService databaseInitializationService,
        IOptions<DatabaseSeedingSecurityOptions> securityOptions,
        ILogger<AdminDatabaseController> logger)
    {
        _databaseInitializationService = databaseInitializationService;
        _securityOptions = securityOptions.Value;
        _logger = logger;
    }

    [HttpPost("seed-catalogs")]
    public async Task<IActionResult> SeedCatalogs(CancellationToken cancellationToken)
    {
        var guard = ValidateSecurity();
        if (guard is not null)
            return guard;

        var result = await _databaseInitializationService.RunCatalogSeedAsync(cancellationToken);
        if (result.Locked)
            return Conflict(new { message = "Seeding is already running." });

        return Ok(result);
    }

    [HttpPost("seed-demo")]
    public async Task<IActionResult> SeedDemo(CancellationToken cancellationToken)
    {
        var guard = ValidateSecurity();
        if (guard is not null)
            return guard;

        try
        {
            var result = await _databaseInitializationService.RunDemoSeedAsync(cancellationToken);
            if (result.Locked)
                return Conflict(new { message = "Seeding is already running." });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = ex.Message,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpPost("repair-plans")]
    public async Task<IActionResult> RepairPlans(CancellationToken cancellationToken)
    {
        var guard = ValidateSecurity();
        if (guard is not null)
            return guard;

        var result = await _databaseInitializationService.RunRepairPlansAsync(cancellationToken);
        if (result.Locked)
            return Conflict(new { message = "Seeding is already running." });

        return Ok(result);
    }

    [HttpPost("migrate-and-seed-catalogs")]
    public async Task<IActionResult> MigrateAndSeedCatalogs(CancellationToken cancellationToken)
    {
        var guard = ValidateSecurity();
        if (guard is not null)
            return guard;

        var result = await _databaseInitializationService.RunMigrationsAndCatalogSeedAsync(cancellationToken);
        if (result.Locked)
            return Conflict(new { message = "Seeding is already running." });

        return Ok(result);
    }

    private IActionResult? ValidateSecurity()
    {
        if (!_securityOptions.Enabled)
        {
            _logger.LogWarning("db.seed.api.disabled traceId={TraceId}", HttpContext.TraceIdentifier);
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Database seeding API is disabled.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        if (!_securityOptions.RequireHeaderKey)
            return null;

        var headerName = string.IsNullOrWhiteSpace(_securityOptions.HeaderName)
            ? "X-Seed-Key"
            : _securityOptions.HeaderName;

        if (!Request.Headers.TryGetValue(headerName, out var provided))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = $"Missing required header: {headerName}",
                traceId = HttpContext.TraceIdentifier
            });
        }

        if (string.IsNullOrWhiteSpace(_securityOptions.HeaderValue) ||
            !string.Equals(provided.ToString(), _securityOptions.HeaderValue, StringComparison.Ordinal))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Invalid seeding security header.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        return null;
    }
}
