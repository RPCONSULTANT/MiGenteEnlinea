using MediatR;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Domain.Interfaces.Repositories;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Contratistas;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratistaFoto;

/// <summary>
/// Handler: Procesa la actualización de URL de foto del Contratista
/// Asume que el archivo ya está guardado en wwwroot/uploads/ (por IFileStorageService)
/// Se encarga de persistir la URL en la BD
/// </summary>
public sealed class UpdateContratistaFotoCommandHandler : IRequestHandler<UpdateContratistaFotoCommand, UpdateContratistaFotoResult>
{
    private readonly IContratistaRepository _contratistaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContratistaFotoCommandHandler> _logger;

    public UpdateContratistaFotoCommandHandler(
        IContratistaRepository contratistaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateContratistaFotoCommandHandler> logger)
    {
        _contratistaRepository = contratistaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Manejaaactualizacion de URL de foto
    /// El archivo ya debería estar en wwwroot/uploads/
    /// </summary>
    public async Task<UpdateContratistaFotoResult> Handle(
        UpdateContratistaFotoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Actualizando foto URL para contratista. UserId: {UserId}, FotoUrl: {FotoUrl}",
                request.UserId, request.FotoUrl);

            // ============================================
            // PASO 1: Buscar contratista por userId
            // ============================================
            var contratista = await _contratistaRepository.GetByUserIdAsync(
                request.UserId,
                cancellationToken);

            if (contratista == null)
            {
                _logger.LogWarning(
                    "Contratista no encontrado para userId: {UserId}",
                    request.UserId);
                return UpdateContratistaFotoResult.FailureResult(
                    $"Contratista no encontrado para usuario {request.UserId}");
            }

            // ============================================
            // PASO 2: Validar URL no vacía
            // ============================================
            if (string.IsNullOrWhiteSpace(request.FotoUrl))
            {
                _logger.LogWarning(
                    "FotoUrl vacía para userId: {UserId}",
                    request.UserId);
                return UpdateContratistaFotoResult.FailureResult(
                    "URL de foto no puede estar vacía");
            }

            // ============================================
            // PASO 3: Llamar método de dominio ActualizarImagen()
            // Esto valida la URL y levanta un evento de dominio
            // ============================================
            try
            {
                contratista.ActualizarImagen(request.FotoUrl);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error en validación de dominio. UserId: {UserId}",
                    request.UserId);
                return UpdateContratistaFotoResult.FailureResult(ex.Message);
            }

            // ============================================
            // PASO 4: Guardar cambios en BD
            // ============================================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Foto actualizada exitosamente. ContratistaId: {ContratistaId}, FotoUrl: {FotoUrl}",
                contratista.Id,
                request.FotoUrl);

            return UpdateContratistaFotoResult.SuccessResult(request.FotoUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado actualizando foto. UserId: {UserId}",
                request.UserId);
            throw;
        }
    }
}
