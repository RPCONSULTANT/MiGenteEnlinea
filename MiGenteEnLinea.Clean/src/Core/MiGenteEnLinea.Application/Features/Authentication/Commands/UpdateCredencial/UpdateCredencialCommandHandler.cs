using MediatR;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Interfaces.Repositories;

namespace MiGenteEnLinea.Application.Features.Authentication.Commands.UpdateCredencial;

/// <summary>
/// Handler para UpdateCredencialCommand
/// Réplica EXACTA de SuscripcionesService.actualizarCredenciales() del Legacy
/// GAP-012: Actualiza password, email y estado activo en una credencial
/// 
/// SINCRONIZACIÓN DUAL: Este handler actualiza TANTO Credenciales (Legacy) como AspNetUsers (Identity)
/// para mantener ambos sistemas en sincronía durante el período de transición.
/// </summary>
public sealed class UpdateCredencialCommandHandler : IRequestHandler<UpdateCredencialCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UpdateCredencialCommandHandler> _logger;

    public UpdateCredencialCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IIdentityService identityService,
        ILogger<UpdateCredencialCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _identityService = identityService;
        _logger = logger;
    }

    /// <summary>
    /// Actualiza credencial completa (password + email + activo)
    /// 
    /// Legacy behavior (SuscripcionesService.cs líneas 157-177):
    /// - Query: db.Credenciales.Where(x => x.email == c.email AND x.userID == c.userID).FirstOrDefault()
    /// - Si existe: result.password = c.password; result.activo = c.activo; result.email = c.email;
    /// - db.SaveChanges()
    /// - Retorna true
    /// 
    /// ⚠️ PROBLEMA LEGACY:
    /// - El WHERE usa email + userID, pero el password ya viene ENCRIPTADO desde el cliente
    /// - En MiPerfilEmpleador.aspx.cs línea 275: cr.password = crypt.Encrypt(txtPassword.Text);
    /// - Legacy NO valida si el email ya existe en otra credencial (puede causar duplicados)
    /// 
    /// Clean behavior:
    /// - Query por userID solamente (más seguro)
    /// - Hashea password con BCrypt (si se provee)
    /// - Valida que nuevo email no exista en otra credencial
    /// - Actualiza email, password (hasheado) y activo
    /// </summary>
    public async Task<bool> Handle(UpdateCredencialCommand request, CancellationToken cancellationToken)
    {
        // ================================================================================
        // PASO 1: OBTENER CREDENCIAL ACTUAL POR USERID
        // ================================================================================
        // Legacy línea 163: db.Credenciales.Where(x => x.email == c.email && x.userID == c.userID).FirstOrDefault()
        // Clean: Solo por userId (más seguro)
        var credencial = await _unitOfWork.Credenciales
            .GetByUserIdAsync(request.UserId, cancellationToken);

        if (credencial == null)
        {
            _logger.LogWarning(
                "No se encontró credencial para actualizar. UserId: {UserId}",
                request.UserId);
            return false;
        }

        // ================================================================================
        // PASO 2: VALIDAR QUE NUEVO EMAIL NO EXISTA EN OTRA CREDENCIAL
        // ================================================================================
        // ⚠️ Legacy NO hace esta validación, pero Clean sí debe hacerla para evitar duplicados
        if (credencial.Email.Value != request.Email)
        {
            var emailExiste = await _unitOfWork.Credenciales
                .ExistsByEmailAsync(request.Email, cancellationToken);

            if (emailExiste)
            {
                _logger.LogWarning(
                    "Email ya existe en otra credencial. Email: {Email}",
                    request.Email);
                return false;
            }
        }

        // ================================================================================
        // PASO 3: ACTUALIZAR CREDENCIAL (Legacy: Credenciales table)
        // ================================================================================
        // Legacy líneas 166-168:
        // result.password = c.password;  // ⚠️ Ya viene encriptado desde cliente
        // result.activo = c.activo;
        // result.email = c.email;

        var emailCambiado = credencial.Email.Value != request.Email;
        var passwordCambiado = !string.IsNullOrWhiteSpace(request.Password);
        var activoCambiado = credencial.Activo != request.Activo;

        // Actualizar email
        var nuevoEmail = Domain.ValueObjects.Email.Create(request.Email);
        if (nuevoEmail == null)
        {
            _logger.LogWarning("Email inválido: {Email}", request.Email);
            return false;
        }
        credencial.ActualizarEmail(nuevoEmail);

        // Actualizar password (solo si se provee)
        if (passwordCambiado)
        {
            var passwordHasheado = _passwordHasher.HashPassword(request.Password);
            credencial.ActualizarPasswordHash(passwordHasheado);
        }

        // Actualizar estado activo
        if (request.Activo && !credencial.Activo)
        {
            credencial.Activar();
        }
        else if (!request.Activo && credencial.Activo)
        {
            credencial.Desactivar();
        }

        // ================================================================================
        // PASO 4: SINCRONIZAR CON IDENTITY (AspNetUsers table)
        // ================================================================================
        // El sistema usa dual-write: Credenciales (Legacy) + AspNetUsers (Identity)
        // Debemos sincronizar los cambios a ambas tablas para mantener consistencia
        
        try
        {
            // Sincronizar password con Identity (si cambió)
            if (passwordCambiado)
            {
                var passwordSynced = await _identityService.ChangePasswordByIdAsync(request.UserId, request.Password);
                if (!passwordSynced)
                {
                    _logger.LogWarning(
                        "No se pudo sincronizar password con Identity. UserId: {UserId} (usuario puede no existir en AspNetUsers)",
                        request.UserId);
                    // No retornamos false - el usuario puede ser Legacy-only (no migrado aún)
                }
            }

            // Sincronizar email con Identity (si cambió)
            if (emailCambiado)
            {
                var emailSynced = await _identityService.UpdateUserEmailAsync(request.UserId, request.Email);
                if (!emailSynced)
                {
                    _logger.LogWarning(
                        "No se pudo sincronizar email con Identity. UserId: {UserId} (usuario puede no existir en AspNetUsers)",
                        request.UserId);
                }
            }

            // Sincronizar estado activo con Identity (si cambió a inactivo)
            if (activoCambiado && !request.Activo)
            {
                var deactivateSynced = await _identityService.DeactivateUserAsync(request.UserId);
                if (!deactivateSynced)
                {
                    _logger.LogWarning(
                        "No se pudo desactivar usuario en Identity. UserId: {UserId} (usuario puede no existir en AspNetUsers)",
                        request.UserId);
                }
            }
            // TODO: Si activoCambiado && request.Activo, implementar ReactivateUserAsync en IIdentityService
        }
        catch (NotImplementedException)
        {
            // LegacyIdentityService throws NotImplementedException for these methods
            // This is expected for users who haven't migrated to Identity yet
            _logger.LogDebug(
                "Usuario usa LegacyIdentityService, no se sincroniza con Identity. UserId: {UserId}",
                request.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Error sincronizando con Identity (no bloqueante). UserId: {UserId}",
                request.UserId);
            // No retornamos false - la actualización de Credenciales fue exitosa
        }

        // ================================================================================
        // PASO 5: GUARDAR CAMBIOS EN CREDENCIALES
        // ================================================================================
        // No necesitamos llamar UpdateAsync, el DbContext detecta los cambios automáticamente
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Credencial actualizada exitosamente. UserId: {UserId}, Email: {Email}, Activo: {Activo}, " +
            "EmailCambiado: {EmailCambiado}, PasswordCambiado: {PasswordCambiado}, ActivoCambiado: {ActivoCambiado}",
            request.UserId,
            request.Email,
            request.Activo,
            emailCambiado,
            passwordCambiado,
            activoCambiado);

        return true;
    }
}
