namespace MiGenteEnLinea.Web.Configuration;

/// <summary>
/// Opciones de configuración para la conexión con el API backend
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// Sección de configuración en appsettings.json
    /// </summary>
    public const string SectionName = "ApiConfiguration";

    /// <summary>
    /// URL base del API (ej: http://localhost:5015/api o https://api.migenteenlinea.com/api)
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5015/api";

    /// <summary>
    /// Timeout en segundos para las peticiones HTTP (default: 30 segundos)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Número de reintentos en caso de fallo temporal (default: 3)
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Endpoint para verificar el estado de salud del API
    /// </summary>
    public string HealthCheckEndpoint { get; set; } = "/health";

    /// <summary>
    /// Habilitar logging detallado de peticiones HTTP (solo development)
    /// </summary>
    public bool EnableRequestLogging { get; set; } = false;

    /// <summary>
    /// Timeout en milisegundos para health checks (default: 5000ms)
    /// </summary>
    public int HealthCheckTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// User-Agent personalizado para las peticiones HTTP
    /// </summary>
    public string UserAgent { get; set; } = "MiGenteEnLinea.Web/1.0";
}
