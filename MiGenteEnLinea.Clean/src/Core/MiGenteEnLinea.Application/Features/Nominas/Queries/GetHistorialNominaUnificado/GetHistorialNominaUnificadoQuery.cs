using MediatR;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;

namespace MiGenteEnLinea.Application.Features.Nominas.Queries.GetHistorialNominaUnificado;

public record GetHistorialNominaUnificadoQuery : IRequest<List<NominaHistorialUnificadoDto>>
{
    public string UserId { get; init; } = string.Empty;
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public int? EmpleadoId { get; init; }
}
