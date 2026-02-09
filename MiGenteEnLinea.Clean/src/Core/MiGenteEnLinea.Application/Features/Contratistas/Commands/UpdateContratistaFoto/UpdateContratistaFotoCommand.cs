using MediatR;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratistaFoto;

/// <summary>
/// Command: Actualizar foto de perfil del Contratista
/// </summary>
/// <remarks>
/// LÓGICA DE NEGOCIO:
/// - Buscar contratista por userId
/// - Validar tamaño máximo (5MB)
/// - Actualizar foto con método de dominio
/// - Guardar cambios en la base de datos
/// </remarks>
public record UpdateContratistaFotoCommand(
    string UserId,
    byte[] Foto
) : IRequest<bool>; // Retorna true si actualización exitosa
