using MediatR;

namespace MiGenteEnLinea.Application.Features.Authentication.Commands.ActivateAccount;

/// <summary>
/// Command para activar una cuenta de usuario y establecer su contraseña
/// </summary>
/// <remarks>
/// Réplica de Activar.aspx.cs + SuscripcionesService.guardarCredenciales() del Legacy
/// Activa una cuenta que fue creada pero aún no activada (activo=false)
/// y establece la contraseña del usuario (flujo Legacy: register sin password → activate con password)
/// </remarks>
public sealed record ActivateAccountCommand : IRequest<bool>
{
    /// <summary>
    /// ID del usuario (GUID)
    /// </summary>
    public required string UserId { get; init; }
    
    /// <summary>
    /// Email del usuario (para validación adicional)
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// Contraseña a establecer para el usuario (flujo Legacy)
    /// </summary>
    public required string Password { get; init; }
    
    /// <summary>
    /// Confirmación de la contraseña
    /// </summary>
    public required string ConfirmPassword { get; init; }
}
