namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Typed API service for Empleadores endpoints
/// </summary>
public class EmpleadoresApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<EmpleadoresApiService> _logger;

    public EmpleadoresApiService(IApiService apiService, ILogger<EmpleadoresApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Search empleadores with filters
    /// </summary>
    public async Task<SearchEmpleadoresResponse?> SearchEmpleadoresAsync(
        string? searchTerm = null,
        bool? soloActivos = true,
        string? sector = null,
        string? provincia = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            
            if (soloActivos.HasValue)
                queryParams.Add($"soloActivos={soloActivos.Value.ToString().ToLower()}");
            
            if (!string.IsNullOrWhiteSpace(sector))
                queryParams.Add($"sector={Uri.EscapeDataString(sector)}");
            
            if (!string.IsNullOrWhiteSpace(provincia))
                queryParams.Add($"provincia={Uri.EscapeDataString(provincia)}");
            
            queryParams.Add($"pageIndex={pageIndex}");
            queryParams.Add($"pageSize={pageSize}");
            
            var queryString = string.Join("&", queryParams);
            var endpoint = $"/empleadores?{queryString}";
            
            return await _apiService.GetAsync<SearchEmpleadoresResponse>(endpoint, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching empleadores");
            throw;
        }
    }

    /// <summary>
    /// Get empleador by UserId
    /// </summary>
    public async Task<EmpleadorDto?> GetEmpleadorByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<EmpleadorDto>($"/empleadores/{userId}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting empleador {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Upload empleador photo
    /// </summary>
    public async Task<UploadPhotoResponse?> UploadEmpleadorFotoAsync(string userId, IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.UploadFileAsync<UploadPhotoResponse>($"/empleadores/{userId}/foto", file, null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading empleador photo for {UserId}", userId);
            throw;
        }
    }
}

// DTOs
public record SearchEmpleadoresResponse(
    List<EmpleadorDto> Items,
    int TotalCount,
    int PageIndex,
    int PageSize,
    int TotalPages);

public record EmpleadorDto(
    int Id,
    string UserId,
    string Nombre,
    string? RNC,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Sector,
    string? Provincia,
    string? FotoUrl,
    bool Activo,
    DateTime CreatedAt);

public record UploadPhotoResponse(
    bool Success,
    string? ImageUrl,
    string? Message);
