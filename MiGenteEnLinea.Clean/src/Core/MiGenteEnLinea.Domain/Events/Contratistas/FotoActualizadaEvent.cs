using MiGenteEnLinea.Domain.Common;

namespace MiGenteEnLinea.Domain.Events.Contratistas;

/// <summary>
/// Evento de dominio: Se dispara cuando un contratista actualiza su foto de perfil (byte array)
/// 
/// CASOS DE USO:
/// - Invalidar cache de foto de perfil
/// - Generar thumbnails de diferentes tamaños
/// - Optimizar imagen para web
/// - Registrar en auditoría
/// - Actualizar resultados de búsqueda con nueva foto
/// </summary>
public sealed class FotoActualizadaEvent : DomainEvent
{
    /// <summary>
    /// ID del contratista que actualizó su foto
    /// </summary>
    public int ContratistaId { get; }

    public FotoActualizadaEvent(int contratistaId)
    {
        ContratistaId = contratistaId;
    }
}
