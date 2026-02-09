namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Typed API service for Nóminas endpoints
/// </summary>
public class NominasApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<NominasApiService> _logger;

    public NominasApiService(IApiService apiService, ILogger<NominasApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Get payroll history for user
    /// </summary>
    public async Task<List<NominaHistorialDto>?> GetHistorialNominaAsync(
        int userId, 
        int pageIndex = 1, 
        int pageSize = 10, 
        string? periodo = null,
        int? estado = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/nominas/historial/{userId}?pageIndex={pageIndex}&pageSize={pageSize}";
            
            if (!string.IsNullOrEmpty(periodo))
                url += $"&periodo={Uri.EscapeDataString(periodo)}";
            
            if (estado.HasValue)
                url += $"&estado={estado.Value}";

            return await _apiService.GetAsync<List<NominaHistorialDto>>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payroll history for UserId {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get payroll history for authenticated user
    /// </summary>
    public async Task<List<NominaHistorialDto>?> GetMiHistorialNominaAsync(
        int pageIndex = 1,
        int pageSize = 10,
        string? periodo = null,
        int? estado = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/nominas/historial?pageIndex={pageIndex}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(periodo))
                url += $"&periodo={Uri.EscapeDataString(periodo)}";

            if (estado.HasValue)
                url += $"&estado={estado.Value}";

            return await _apiService.GetAsync<List<NominaHistorialDto>>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my payroll history");
            throw;
        }
    }

    /// <summary>
    /// Get payroll summary for a period
    /// </summary>
    public async Task<NominaResumenDto?> GetResumenNominaAsync(
        int empleadorId = 0,
        string? periodo = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        bool incluirDetalleEmpleados = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = "/nominas/resumen?";
            
            if (empleadorId > 0)
                url += $"empleadorId={empleadorId}&";
            
            if (!string.IsNullOrEmpty(periodo))
                url += $"periodo={Uri.EscapeDataString(periodo)}&";
            
            if (fechaInicio.HasValue)
                url += $"fechaInicio={fechaInicio:yyyy-MM-dd}&";
            
            if (fechaFin.HasValue)
                url += $"fechaFin={fechaFin:yyyy-MM-dd}&";
            
            url += $"incluirDetalleEmpleados={incluirDetalleEmpleados}";

            return await _apiService.GetAsync<NominaResumenDto>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payroll summary");
            throw;
        }
    }
}

// DTOs
public record NominaHistorialDto(
    int NominaId,
    string Periodo,
    int CantidadEmpleados,
    decimal TotalNomina,
    DateTime FechaProcesamiento,
    int Estado,
    string EstadoTexto,
    bool EmailEnviado,
    DateTime? FechaEnvioEmail,
    string? Notas);

public record NominaResumenDto(
    int EmpleadorId,
    string Periodo,
    decimal TotalNomina,
    int CantidadEmpleados,
    List<object> DetalleEmpleados);
