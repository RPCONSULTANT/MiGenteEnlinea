using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;

/// <summary>
/// Command to create a new DetalleContrataciones entry
/// Migrated from: EmpleadosService.nuevaContratacionTemporal(DetalleContrataciones det) - line 438
/// </summary>
public record CreateDetalleContratacionCommand : IRequest<int>
{
    public int ContratacionId { get; init; }
    public string? DescripcionCorta { get; init; }
    public string? DescripcionAmpliada { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFinal { get; init; }  // Renamed from FechaFin to match Domain entity
    public decimal? MontoAcordado { get; init; }
    public string? EsquemaPagos { get; init; }
    public int? Estatus { get; init; }
}
