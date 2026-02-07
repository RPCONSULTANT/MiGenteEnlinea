namespace MiGenteEnLinea.API.Configuration;

/// <summary>
/// Opciones de configuración para CORS (Cross-Origin Resource Sharing)
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Sección de configuración en appsettings.json
    /// </summary>
    public const string SectionName = "CorsConfiguration";

    /// <summary>
    /// Origines permitidos para realizar peticiones al API
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Permitir credenciales (cookies, headers de autorización)
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Métodos HTTP permitidos (GET, POST, PUT, DELETE, etc.)
    /// Si está vacío, permite todos los métodos
    /// </summary>
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Headers permitidos en las peticiones
    /// Si está vacío, permite todos los headers
    /// </summary>
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Headers expuestos en las respuestas (para que el cliente pueda leerlos)
    /// </summary>
    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Tiempo máximo en segundos que el navegador puede cachear la respuesta preflight
    /// </summary>
    public int MaxAgeSeconds { get; set; } = 600; // 10 minutos
}
