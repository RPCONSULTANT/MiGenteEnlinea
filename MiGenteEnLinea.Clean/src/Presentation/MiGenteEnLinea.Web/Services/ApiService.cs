using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MiGenteEnLinea.Web.Configuration;

namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Generic API service for making HTTP requests to MiGenteEnLinea.API with automatic error handling,
/// authentication, and retry logic
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _authToken;

    public ApiService(HttpClient httpClient, IOptions<ApiOptions> apiOptions, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Configure JSON options to match API serialization
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Set base URL from configuration
        if (!string.IsNullOrEmpty(apiOptions.Value?.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(apiOptions.Value.BaseUrl);
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(apiOptions.Value?.TimeoutSeconds ?? 30);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void SetAuthenticationToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthenticationToken()
    {
        _authToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default) where TResponse : class
    {
        try
        {
            _logger.LogInformation("GET request to {Endpoint}", endpoint);
            
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            return await HandleResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GET request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default) 
        where TRequest : class 
        where TResponse : class
    {
        try
        {
            _logger.LogInformation("POST request to {Endpoint}", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            return await HandleResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data, CancellationToken cancellationToken = default) where TRequest : class
    {
        try
        {
            _logger.LogInformation("POST request to {Endpoint} (no response expected)", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                await LogErrorResponse(response, endpoint);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
            return false;
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default) 
        where TRequest : class 
        where TResponse : class
    {
        try
        {
            _logger.LogInformation("PUT request to {Endpoint}", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            
            return await HandleResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PUT request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data, CancellationToken cancellationToken = default) where TRequest : class
    {
        try
        {
            _logger.LogInformation("PUT request to {Endpoint} (no response expected)", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                await LogErrorResponse(response, endpoint);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PUT request to {Endpoint}", endpoint);
            return false;
        }
    }

    public async Task<TResponse?> DeleteAsync<TResponse>(string endpoint, object? data = null, CancellationToken cancellationToken = default) where TResponse : class
    {
        try
        {
            _logger.LogInformation("DELETE request to {Endpoint}", endpoint);
            
            HttpResponseMessage response;
            
            if (data != null)
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions), 
                        Encoding.UTF8, 
                        "application/json")
                };
                
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            else
            {
                response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            }
            
            return await HandleResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("DELETE request to {Endpoint} (no response expected)", endpoint);
            
            HttpResponseMessage response;
            
            if (data != null)
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions), 
                        Encoding.UTF8, 
                        "application/json")
                };
                
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            else
            {
                response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            }
            
            if (!response.IsSuccessStatusCode)
            {
                await LogErrorResponse(response, endpoint);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
            return false;
        }
    }

    public async Task<TResponse?> UploadFileAsync<TResponse>(string endpoint, IFormFile file, Dictionary<string, string>? additionalData = null, CancellationToken cancellationToken = default) 
        where TResponse : class
    {
        try
        {
            _logger.LogInformation("File upload to {Endpoint}, File: {FileName}", endpoint, file.FileName);
            
            using var content = new MultipartFormDataContent();
            
            // Add file
            using var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);
            
            // Add additional data if provided
            if (additionalData != null)
            {
                foreach (var kvp in additionalData)
                {
                    content.Add(new StringContent(kvp.Value), kvp.Key);
                }
            }
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            return await HandleResponse<TResponse>(response, endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to {Endpoint}", endpoint);
            throw;
        }
    }

    private async Task<TResponse?> HandleResponse<TResponse>(HttpResponseMessage response, string endpoint) where TResponse : class
    {
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                _logger.LogWarning("Empty response from {Endpoint}", endpoint);
                return null;
            }
            
            try
            {
                var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {Endpoint}. Response: {Response}", endpoint, responseContent);
                throw;
            }
        }
        
        await LogErrorResponse(response, endpoint);
        
        // Throw appropriate exception based on status code
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException($"Unauthorized access to {endpoint}");
        }
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Resource not found: {endpoint}");
        }
        
        throw new HttpRequestException($"Request to {endpoint} failed with status {response.StatusCode}");
    }

    private async Task LogErrorResponse(HttpResponseMessage response, string endpoint)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "Request to {Endpoint} failed with status {StatusCode}. Response: {Response}", 
            endpoint, 
            response.StatusCode, 
            errorContent);
    }
}
