using System.Net;
using System.Text.Json;
using MiGenteEnLinea.Application.Common.Exceptions;

namespace MiGenteEnLinea.API.Middleware;

/// <summary>
/// Middleware para manejar excepciones globalmente y convertirlas en respuestas HTTP apropiadas.
/// Este middleware intercepta todas las excepciones no manejadas y las convierte en respuestas JSON.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;

            case FluentValidation.ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Errores de validación";
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Errors = validationEx.Errors
                    .Select(e => new ValidationError { Property = e.PropertyName, Message = e.ErrorMessage })
                    .ToList();
                _logger.LogWarning(validationEx, "Validation failed: {Errors}", 
                    string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage)));
                break;

            case UnauthorizedAccessException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "No autorizado";
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                _logger.LogWarning(unauthorizedEx, "Unauthorized access attempt");
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning(argEx, "Invalid argument: {Message}", argEx.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Ha ocurrido un error interno. Por favor intente nuevamente.";
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null // PascalCase para mantener consistencia
        };

        var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(result);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public List<ValidationError>? Errors { get; set; }
}

public class ValidationError
{
    public string Property { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
