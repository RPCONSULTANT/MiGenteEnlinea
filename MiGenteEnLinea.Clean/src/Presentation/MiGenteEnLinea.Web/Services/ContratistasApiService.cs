namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Typed API service for Contratistas endpoints
/// </summary>
public class ContratistasApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<ContratistasApiService> _logger;

    public ContratistasApiService(IApiService apiService, ILogger<ContratistasApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Get contratista by UserId
    /// </summary>
    public async Task<ContratistaDto?> GetContratistaByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<ContratistaDto>($"/contratistas/{userId}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contratista {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Update contratista profile
    /// </summary>
    public async Task<bool> UpdateContratistaAsync(string userId, UpdateContratistaRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.PutAsync($"/contratistas/{userId}", request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contratista {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Upload contratista photo
    /// </summary>
    public async Task<UploadPhotoResponse?> UploadContratistaFotoAsync(string userId, IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.UploadFileAsync<UploadPhotoResponse>($"/contratistas/{userId}/foto", file, null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading contratista photo for {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get contratista's services
    /// </summary>
    public async Task<List<ServicioDto>?> GetServiciosAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiService.GetAsync<List<ServicioDto>>($"/contratistas/{userId}/servicios", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting servicios for contratista {UserId}", userId);
            throw;
        }
    }
}

// DTOs
public record ContratistaDto(
    int Id,
    string UserId,
    string Nombre,
    string Apellido,
    string? Cedula,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Titulo,
    string? Sector,
    int? Experiencia,
    string? Descripcion,
    string? FotoUrl,
    bool Activo,
    DateTime CreatedAt);

public record UpdateContratistaRequest
{
    public string? Titulo { get; init; }
    public string? Sector { get; init; }
    public int? Experiencia { get; init; }
    public string? Descripcion { get; init; }
    public string? Telefono { get; init; }
    public string? Direccion { get; init; }
}

public record ServicioDto(
    int Id,
    string Nombre,
    string? Descripcion);
