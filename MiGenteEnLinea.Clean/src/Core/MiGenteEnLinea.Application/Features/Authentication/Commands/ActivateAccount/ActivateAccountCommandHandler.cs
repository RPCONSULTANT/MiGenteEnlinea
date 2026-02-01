using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Interfaces.Repositories;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Authentication;

namespace MiGenteEnLinea.Application.Features.Authentication.Commands.ActivateAccount;

/// <summary>
/// Handler para ActivateAccountCommand
/// ESTRATEGIA DE MIGRACIÓN: Identity Primary + Legacy Sync
/// 1. Activar cuenta en ASP.NET Core Identity (EmailConfirmed = true) - PRIMARIO
/// 2. Establecer contraseña en Identity (usando ChangePasswordByIdAsync) - PRIMARIO
/// 3. Sincronizar con tabla Legacy Credenciales (Activo = true, PasswordHash) - SECUNDARIO
/// Réplica EXACTA de Activar.aspx.cs del Legacy
/// </summary>
public sealed class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, bool>
{
    private readonly IIdentityService _identityService; // ✅ Sistema Identity (primario)
    private readonly ICredencialRepository _credencialRepository; // ✅ Para sincronizar Legacy
    private readonly IPasswordHasher _passwordHasher; // ✅ Para hashear password en Legacy sync
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateAccountCommandHandler> _logger;

    public ActivateAccountCommandHandler(
        IIdentityService identityService,
        ICredencialRepository credencialRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ActivateAccountCommandHandler> logger)
    {
        _identityService = identityService;
        _credencialRepository = credencialRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Iniciando activación de cuenta. UserId: {UserId}, Email: {Email}",
            request.UserId, request.Email);

        // ================================================================================
        // PASO 1: ACTIVAR CUENTA EN IDENTITY (EmailConfirmed = true) ✅
        // ================================================================================
        var identitySuccess = await _identityService.ActivateAccountAsync(request.UserId, request.Email);

        if (!identitySuccess)
        {
            _logger.LogWarning(
                "Activación fallida en Identity. UserId: {UserId}, Email: {Email}",
                request.UserId, request.Email);
            return false;
        }

        _logger.LogInformation(
            "Cuenta activada en Identity (EmailConfirmed=true). UserId: {UserId}",
            request.UserId);

        // ================================================================================
        // PASO 2: ESTABLECER CONTRASEÑA EN IDENTITY ✅
        // ================================================================================
        // Flujo Legacy: El usuario se registra sin contraseña, luego activa la cuenta
        // y en ese momento establece la contraseña por primera vez
        var passwordSuccess = await _identityService.ChangePasswordByIdAsync(request.UserId, request.Password);

        if (!passwordSuccess)
        {
            _logger.LogError(
                "Error al establecer contraseña en Identity. UserId: {UserId}",
                request.UserId);
            return false;
        }

        _logger.LogInformation(
            "Contraseña establecida en Identity. UserId: {UserId}",
            request.UserId);

        // ================================================================================
        // PASO 3: SINCRONIZAR CON TABLA LEGACY (SECUNDARIO) ✅
        // ================================================================================
        // Nota: Si esta sincronización falla, el usuario aún puede autenticarse con Identity
        try
        {
            var credencial = await _credencialRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (credencial == null)
            {
                _logger.LogWarning(
                    "Credencial Legacy no encontrada para sincronización. UserId: {UserId}. Usuario puede autenticarse con Identity.",
                    request.UserId);
                return true; // Retornar success porque Identity ya está activado y tiene password
            }

            // Activar y establecer password en Legacy
            credencial.Activar();
            
            // Hashear password para Legacy (BCrypt via IPasswordHasher)
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            credencial.ActualizarPasswordHash(passwordHash);
            
            _credencialRepository.Update(credencial);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Tabla Legacy Credenciales sincronizada (Activo=true, PasswordHash actualizado). UserId: {UserId}",
                request.UserId);
        }
        catch (Exception ex)
        {
            // NO fallar la operación si Legacy sync falla
            // El usuario ya está activado en Identity con password y puede autenticarse
            _logger.LogError(
                ex,
                "Error al sincronizar con Legacy Credenciales. UserId: {UserId}. Usuario activado en Identity correctamente.",
                request.UserId);
        }

        _logger.LogInformation(
            "Cuenta activada exitosamente. UserId: {UserId}, Email: {Email}",
            request.UserId, request.Email);

        return true;
    }
}
