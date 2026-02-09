using MediatR;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Domain.Interfaces.Repositories;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Contratistas;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratistaFoto;

/// <summary>
/// Handler: Procesa la actualización de la foto del Contratista
/// </summary>
public sealed class UpdateContratistaFotoCommandHandler : IRequestHandler<UpdateContratistaFotoCommand, bool>
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
    /// Maneja la actualización de la foto del contratista
    /// </summary>
    /// <exception cref="InvalidOperationException">Si contratista no existe</exception>
    /// <exception cref="ArgumentException">Si foto excede tamaño máximo</exception>
    public async Task<bool> Handle(UpdateContratistaFotoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Actualizando foto de contratista. UserId: {UserId}, TamañoFoto: {TamañoBytes} bytes",
            request.UserId, request.Foto.Length);

        // ============================================
        // PASO 1: Buscar contratista por userId
        // ============================================
        var contratista = await _contratistaRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (contratista == null)
        {
            _logger.LogWarning("Contratista no encontrado para userId: {UserId}", request.UserId);
            throw new InvalidOperationException($"Contratista no encontrado para usuario {request.UserId}");
        }

        // ============================================
        // PASO 2: Actualizar foto con método de dominio
        // ============================================
        // El método ActualizarFoto() de la entidad Contratista maneja:
        // - Validación de tamaño máximo (5MB)
        // - Validación de foto no vacía
        // - Levanta eventos de dominio
        try
        {
            contratista.ActualizarFoto(request.Foto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al actualizar foto: {Mensaje}", ex.Message);
            throw; // Re-throw para que API retorne 400 Bad Request
        }

        // ============================================
        // PASO 3: Guardar cambios
        // ============================================
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Foto de contratista actualizada exitosamente. UserId: {UserId}",
            request.UserId);

        return true;
    }
}
