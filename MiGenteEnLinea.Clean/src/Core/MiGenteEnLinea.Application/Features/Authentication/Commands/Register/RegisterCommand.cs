using MediatR;
using MiGenteEnLinea.Application.Features.Authentication.DTOs;

namespace MiGenteEnLinea.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Command para registrar un nuevo usuario en el sistema
/// </summary>
/// <remarks>
/// Réplica de SuscripcionesService.GuardarPerfil() del Legacy
/// Crea: Cuenta, Credencial, Contratista (si tipo=2), Suscripción inicial
/// FLUJO LEGACY: El usuario se registra SIN contraseña, luego activa su cuenta
/// y en ese momento crea la contraseña.
/// </remarks>
public sealed record RegisterCommand : IRequest<RegisterResult>
{
    /// <summary>
    /// Email del usuario (único en el sistema)
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// Contraseña sin hashear (OPCIONAL - se crea en la activación)
    /// En el flujo Legacy, el usuario crea la contraseña al activar la cuenta.
    /// Si viene vacía o null, la cuenta queda pendiente de activación.
    /// </summary>
    public string? Password { get; init; }
    
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public required string Nombre { get; init; }
    
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public required string Apellido { get; init; }
    
    /// <summary>
    /// Tipo de usuario: 1 = Empleador, 2 = Contratista
    /// </summary>
    public required int Tipo { get; init; }
    
    /// <summary>
    /// Teléfono 1 (opcional)
    /// </summary>
    public string? Telefono1 { get; init; }
    
    /// <summary>
    /// Teléfono 2 (opcional)
    /// </summary>
    public string? Telefono2 { get; init; }
    
    /// <summary>
    /// URL del host para generar el link de activación
    /// Ejemplo: "https://migenteenlinea.com" o "http://localhost:5244"
    /// Si no se proporciona, se usa un valor por defecto
    /// </summary>
    public string? Host { get; init; }
}
