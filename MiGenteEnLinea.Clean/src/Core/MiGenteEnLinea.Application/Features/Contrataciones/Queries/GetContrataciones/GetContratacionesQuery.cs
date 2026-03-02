using MediatR;
using MiGenteEnLinea.Application.Features.Contrataciones.DTOs;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Queries.GetContrataciones;

/// <summary>
/// Query para obtener lista de contrataciones con filtros opcionales.
/// 
/// CONTEXTO:
/// - Soporta filtros por estado, empleador, contratista, fechas
/// - Incluye paginación
/// - Usado para listados y búsquedas
/// </summary>
public record GetContratacionesQuery : IRequest<List<ContratacionDto>>
{
    /// <summary>
    /// UserId autenticado que consulta. Se inyecta en el controller.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Alcance de consulta: "mine" (default) o "all" (solo admin).
    /// </summary>
    public string Scope { get; init; } = "mine";

    /// <summary>
    /// Filtro por ID de contratación padre (opcional)
    /// </summary>
    public int? ContratacionId { get; init; }

    /// <summary>
    /// Filtro por estado (opcional).
    /// 1=Pendiente, 2=Aceptada, 3=En Progreso, 4=Completada, 5=Cancelada, 6=Rechazada
    /// </summary>
    public int? Estatus { get; init; }

    /// <summary>
    /// Filtro por fecha de inicio desde (opcional)
    /// </summary>
    public DateOnly? FechaInicioDesde { get; init; }

    /// <summary>
    /// Filtro por fecha de inicio hasta (opcional)
    /// </summary>
    public DateOnly? FechaInicioHasta { get; init; }

    /// <summary>
    /// Filtro por monto mínimo (opcional)
    /// </summary>
    public decimal? MontoMinimo { get; init; }

    /// <summary>
    /// Filtro por monto máximo (opcional)
    /// </summary>
    public decimal? MontoMaximo { get; init; }

    /// <summary>
    /// Solo contrataciones pendientes (opcional)
    /// </summary>
    public bool? SoloPendientes { get; init; }

    /// <summary>
    /// Solo contrataciones activas (En Progreso) (opcional)
    /// </summary>
    public bool? SoloActivas { get; init; }

    /// <summary>
    /// Solo contrataciones no calificadas (opcional)
    /// </summary>
    public bool? SoloNoCalificadas { get; init; }

    /// <summary>
    /// Número de página (para paginación)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamaño de página (para paginación)
    /// </summary>
    public int PageSize { get; init; } = 20;
}
