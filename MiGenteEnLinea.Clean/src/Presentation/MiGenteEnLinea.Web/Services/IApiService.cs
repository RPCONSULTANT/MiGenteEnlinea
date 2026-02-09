using System.Net.Http.Headers;

namespace MiGenteEnLinea.Web.Services;

/// <summary>
/// Generic API service interface for making HTTP requests to MiGenteEnLinea.API
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Perform GET request and deserialize response
    /// </summary>
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default) where TResponse : class;

    /// <summary>
    /// Perform POST request with body and deserialize response
    /// </summary>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// Perform POST request without expecting a response body
    /// </summary>
    Task<bool> PostAsync<TRequest>(string endpoint, TRequest data, CancellationToken cancellationToken = default) where TRequest : class;

    /// <summary>
    /// Perform PUT request with body and deserialize response
    /// </summary>
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// Perform PUT request without expecting a response body
    /// </summary>
    Task<bool> PutAsync<TRequest>(string endpoint, TRequest data, CancellationToken cancellationToken = default) where TRequest : class;

    /// <summary>
    /// Perform DELETE request with optional body and deserialize response
    /// </summary>
    Task<TResponse?> DeleteAsync<TResponse>(string endpoint, object? data = null, CancellationToken cancellationToken = default) where TResponse : class;

    /// <summary>
    /// Perform DELETE request without expecting a response body
    /// </summary>
    Task<bool> DeleteAsync(string endpoint, object? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload file with multipart/form-data
    /// </summary>
    Task<TResponse?> UploadFileAsync<TResponse>(string endpoint, IFormFile file, Dictionary<string, string>? additionalData = null, CancellationToken cancellationToken = default) 
        where TResponse : class;

    /// <summary>
    /// Set JWT token for authentication
    /// </summary>
    void SetAuthenticationToken(string token);

    /// <summary>
    /// Clear authentication token
    /// </summary>
    void ClearAuthenticationToken();
}
