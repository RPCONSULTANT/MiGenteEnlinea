namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Typed API service for Suscripciones endpoints
/// </summary>
public class SuscripcionesApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<SuscripcionesApiService> _logger;

    public SuscripcionesApiService(IApiService apiService, ILogger<SuscripcionesApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Get active subscription for user
    /// </summary>
    public async Task<SuscripcionDto?> GetSuscripcionActivaAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<SuscripcionDto>($"/suscripciones/{userId}/activa", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active subscription for {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get subscription history for user
    /// </summary>
    public async Task<List<SuscripcionDto>?> GetHistorialSuscripcionesAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<List<SuscripcionDto>>($"/suscripciones/{userId}/historial", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscription history for {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Cancel active subscription
    /// </summary>
    public async Task<bool> CancelarSuscripcionAsync(string userId, string motivo, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CancelarSuscripcionCommand(userId, motivo);
            return await _apiService.DeleteAsync($"/suscripciones/{userId}", command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription for {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get available plans
    /// </summary>
    public async Task<List<PlanDto>?> GetPlanesAsync(string tipoPlan, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<List<PlanDto>>($"/suscripciones/planes?tipo={tipoPlan}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plans for tipo {TipoPlan}", tipoPlan);
            throw;
        }
    }
}

// DTOs
public record SuscripcionDto(
    int Id,
    string UserId,
    int PlanId,
    string PlanNombre,
    decimal Monto,
    DateTime FechaInicio,
    DateTime FechaVencimiento,
    string Estado,
    bool Activa);

public record CancelarSuscripcionCommand(
    string UserId,
    string Motivo);

public record PlanDto(
    int Id,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int DuracionDias,
    string TipoPlan);
