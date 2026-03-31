using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de consulta al Padrón Nacional Dominicano.
/// Integra con API externa: https://abcportal.online/Sigeinfo/public/api/
/// 
/// Características:
/// - Autenticación con JWT (token cacheado 24 horas)
/// - Cache de consultas (5 minutos)
/// - Retry policy manejado por Polly en DI
/// - Logging estructurado de todas las operaciones
/// </summary>
public class PadronService : IPadronService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PadronService> _logger;
    private readonly PadronSettings _settings;

    // Cache keys
    private const string TokenCacheKey = "padron:auth:token";
    private const string CedulaCacheKeyPrefix = "padron:cedula:";
    
    // Cache durations
    private static readonly TimeSpan TokenCacheDuration = TimeSpan.FromHours(24);
    private static readonly TimeSpan CedulaCacheDuration = TimeSpan.FromMinutes(5);

    public PadronService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<PadronService> logger,
        IOptions<PadronSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("PadronAPI");
        _cache = cache;
        _logger = logger;
        _settings = settings.Value;

        // Validar configuración
        ValidarConfiguracion();
    }

    /// <summary>
    /// Consulta una cédula en el Padrón Nacional.
    /// Maneja autenticación, caching y logging.
    /// </summary>
    public async Task<PadronModel?> ConsultarCedulaAsync(string cedula, CancellationToken cancellationToken = default)
    {
        try
        {
            // PASO 1: Validar formato de cédula
            var cedulaLimpia = LimpiarCedula(cedula);
            if (!EsCedulaValida(cedulaLimpia))
            {
                _logger.LogWarning("Formato de cédula inválido: {Cedula}. Debe tener 11 dígitos", cedula);
                return null;
            }

            _logger.LogInformation("Consultando Padrón Nacional para cédula: {Cedula}", cedulaLimpia);

            // PASO 2: Verificar cache
            var cacheKey = $"{CedulaCacheKeyPrefix}{cedulaLimpia}";
            if (_cache.TryGetValue<PadronModel>(cacheKey, out var cachedResult))
            {
                _logger.LogInformation("Consulta Padrón desde CACHE: {Cedula}", cedulaLimpia);
                return cachedResult;
            }

            // PASO 3: Autenticar (obtener token JWT)
            var token = await ObtenerTokenAutenticacionAsync(cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("No se pudo autenticar con la API del Padrón Nacional");
                return null;
            }

            // PASO 4: Consultar individuo con token
            var response = await ConsultarIndividuoAsync(cedulaLimpia, token, cancellationToken);
            if (response == null)
            {
                return null;
            }

            // PASO 5: Deserializar respuesta
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogDebug("Respuesta Padrón API: {Content}", content);

            var padronData = DeserializarRespuestaPadron(content, cedulaLimpia);
            if (padronData == null)
            {
                _logger.LogError("No se pudo deserializar la respuesta del Padrón para cédula: {Cedula}", cedulaLimpia);
                return null;
            }

            // PASO 6: Guardar en cache
            _cache.Set(cacheKey, padronData, CedulaCacheDuration);

            _logger.LogInformation(
                "Consulta Padrón EXITOSA: {Cedula} - {Nombre} (cacheado {Minutos} min)",
                cedulaLimpia,
                padronData.NombreCompleto,
                CedulaCacheDuration.TotalMinutes);

            return padronData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al consultar Padrón Nacional: {Cedula}", cedula);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout al consultar Padrón Nacional: {Cedula}", cedula);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al parsear JSON de respuesta del Padrón: {Cedula}", cedula);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al consultar Padrón Nacional: {Cedula}", cedula);
            return null;
        }
    }

    private async Task<HttpResponseMessage?> ConsultarIndividuoAsync(
        string cedulaLimpia,
        string token,
        CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"individuo/{cedulaLimpia}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token del padrón expirado o inválido. Refrescando token para {Cedula}", cedulaLimpia);
            _cache.Remove(TokenCacheKey);

            var refreshedToken = await ObtenerTokenAutenticacionAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(refreshedToken))
            {
                _logger.LogError("No se pudo refrescar el token del padrón para {Cedula}", cedulaLimpia);
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
            response = await _httpClient.GetAsync($"individuo/{cedulaLimpia}", cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Cédula NO encontrada en Padrón Nacional: {Cedula}", cedulaLimpia);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Error al consultar Padrón. Status: {Status}, Cédula: {Cedula}, Body: {Body}",
                    response.StatusCode,
                    cedulaLimpia,
                    errorBody);
            }

            return null;
        }

        return response;
    }

    /// <summary>
    /// Obtiene token JWT del Padrón API.
    /// Token se cachea por 24 horas.
    /// </summary>
    private async Task<string?> ObtenerTokenAutenticacionAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Verificar cache de token
            if (_cache.TryGetValue<string>(TokenCacheKey, out var cachedToken))
            {
                _logger.LogDebug("Token de autenticación desde CACHE");
                return cachedToken;
            }

            _logger.LogInformation("Autenticando con API del Padrón Nacional...");

            // Preparar credenciales
            var loginData = new
            {
                username = _settings.Username,
                password = _settings.Password
            };

            var response = await _httpClient.PostAsJsonAsync("login", loginData, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Autenticación FALLIDA con API Padrón. Status: {Status}, Error: {Error}",
                    response.StatusCode,
                    errorContent);
                return null;
            }

            // Parsear respuesta
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(content);
            
            // La respuesta puede venir en diferentes formatos según la API
            // Intentar obtener token de diferentes propiedades
            string? token = null;
            
            if (jsonDoc.RootElement.TryGetProperty("token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }
            else if (jsonDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                token = accessTokenElement.GetString();
            }
            else if (jsonDoc.RootElement.TryGetProperty("accessToken", out var accessTokenCamelElement))
            {
                token = accessTokenCamelElement.GetString();
            }
            else if (jsonDoc.RootElement.TryGetProperty("data", out var dataElement) &&
                     dataElement.TryGetProperty("token", out var dataTokenElement))
            {
                token = dataTokenElement.GetString();
            }
            else if (jsonDoc.RootElement.TryGetProperty("data", out var nestedDataElement) &&
                     nestedDataElement.TryGetProperty("access_token", out var nestedAccessTokenElement))
            {
                token = nestedAccessTokenElement.GetString();
            }

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Token no encontrado en respuesta de autenticación. Respuesta: {Content}", content);
                return null;
            }

            // Cachear token (24 horas - asumiendo que no expira antes)
            _cache.Set(TokenCacheKey, token, TokenCacheDuration);

            _logger.LogInformation("Autenticación EXITOSA con API Padrón (token cacheado 24h)");

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante autenticación con API del Padrón Nacional");
            return null;
        }
    }

    /// <summary>
    /// Deserializa la respuesta JSON del Padrón a PadronModel.
    /// Maneja diferentes formatos de respuesta que puede devolver la API.
    /// </summary>
    private PadronModel? DeserializarRespuestaPadron(string jsonContent, string cedulaConsultada)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(jsonContent);
            var root = jsonDoc.RootElement;

            // La API puede devolver los datos directamente o dentro de una propiedad "data"
            var dataElement = root;
            if (root.TryGetProperty("data", out var nestedData))
            {
                dataElement = nestedData;
            }

            // Extraer campos (nombres pueden variar según versión de API)
            var cedula = ObtenerValorString(dataElement, "cedula", "Cedula", "CEDULA") ?? cedulaConsultada;
            var nombres = ObtenerValorString(dataElement, "nombres", "Nombres", "NOMBRES", "nombre", "Nombre");
            var apellido1 = ObtenerValorString(dataElement, "apellido1", "Apellido1", "APELLIDO1", "primer_apellido", "primerApellido");
            var apellido2 = ObtenerValorString(dataElement, "apellido2", "Apellido2", "APELLIDO2", "segundo_apellido", "segundoApellido");
            var photo = ObtenerValorString(dataElement, "photo", "Photo", "PHOTO", "foto", "Foto");
            var fechaNacimiento = ObtenerFecha(dataElement, "fecha_nacimiento", "fechaNacimiento", "FechaNacimiento", "FECHA_NACIMIENTO");
            var lugarNacimiento = ObtenerValorString(dataElement, "lugar_nacimiento", "lugarNacimiento", "LugarNacimiento");
            var estadoCivil = ObtenerValorString(dataElement, "estado_civil", "estadoCivil", "EstadoCivil");
            var ocupacion = ObtenerValorString(dataElement, "ocupacion", "Ocupacion", "OCUPACION");

            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellido1))
            {
                _logger.LogWarning("Respuesta del Padrón incompleta. Faltan campos requeridos (nombres, apellido1)");
                return null;
            }

            return new PadronModel
            {
                Cedula = cedula,
                Nombres = nombres,
                Apellido1 = apellido1,
                Apellido2 = apellido2,
                Photo = photo,
                FechaNacimiento = fechaNacimiento,
                LugarNacimiento = lugarNacimiento,
                EstadoCivil = estadoCivil,
                Ocupacion = ocupacion
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al deserializar respuesta del Padrón. JSON: {Json}", jsonContent);
            return null;
        }
    }

    /// <summary>
    /// Intenta obtener un valor string de un JsonElement probando múltiples nombres de propiedad.
    /// </summary>
    private string? ObtenerValorString(JsonElement element, params string[] propiedades)
    {
        foreach (var propiedad in propiedades)
        {
            if (element.TryGetProperty(propiedad, out var valor) && valor.ValueKind == JsonValueKind.String)
            {
                var texto = valor.GetString();
                if (!string.IsNullOrWhiteSpace(texto))
                    return texto.Trim();
            }
        }
        return null;
    }

    /// <summary>
    /// Intenta obtener una fecha de un JsonElement probando múltiples nombres de propiedad.
    /// </summary>
    private DateTime? ObtenerFecha(JsonElement element, params string[] propiedades)
    {
        foreach (var propiedad in propiedades)
        {
            if (element.TryGetProperty(propiedad, out var valor))
            {
                if (valor.ValueKind == JsonValueKind.String)
                {
                    var texto = valor.GetString();
                    if (DateTime.TryParse(texto, out var fecha))
                        return fecha;
                }
                else if (valor.TryGetDateTime(out var fecha))
                {
                    return fecha;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Limpia la cédula removiendo guiones y espacios.
    /// Ejemplo: "001-1234567-8" → "00112345678"
    /// </summary>
    private string LimpiarCedula(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
            return string.Empty;

        return cedula.Replace("-", "").Replace(" ", "").Trim();
    }

    /// <summary>
    /// Valida que la cédula tenga exactamente 11 dígitos.
    /// </summary>
    private bool EsCedulaValida(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
            return false;

        return cedula.Length == 11 && cedula.All(char.IsDigit);
    }

    /// <summary>
    /// Valida que la configuración del servicio sea correcta.
    /// </summary>
    private void ValidarConfiguracion()
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            throw new InvalidOperationException("PadronAPI:BaseUrl no está configurado en appsettings.json");

        if (string.IsNullOrWhiteSpace(_settings.Username))
            throw new InvalidOperationException("PadronAPI:Username no está configurado. Use User Secrets o variables de entorno");

        if (string.IsNullOrWhiteSpace(_settings.Password))
            throw new InvalidOperationException("PadronAPI:Password no está configurado. Use User Secrets o variables de entorno");

        _logger.LogInformation("PadronService configurado correctamente. BaseUrl: {BaseUrl}", _settings.BaseUrl);
    }
}
