using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;

/// <summary>
/// Command to create a new EmpleadoTemporal and DetalleContrataciones
/// Migrated from: EmpleadosService.nuevoTemporal(EmpleadosTemporales temp, DetalleContrataciones det) - line 387
/// </summary>
public record CreateEmpleadoTemporalCommand : IRequest<int>
{
    // EmpleadoTemporal fields
    public string UserId { get; init; } = string.Empty;
    public int? Tipo { get; init; }
    public string? NombreComercial { get; init; }
    public string? Rnc { get; init; }
    public string? Nombre { get; init; }
    public string? Apellido { get; init; }
    public string? Identificacion { get; init; }
    public string? Sexo { get; init; }
    public string? Telefono { get; init; }
    public string? Direccion { get; init; }
    public string? Email { get; init; }
    public DateTime? FechaNacimiento { get; init; }

    // DetalleContrataciones fields
    public string? Servicio { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFinal { get; init; }  // Renamed from FechaFin to match Domain entity
    public decimal? Pago { get; init; }
    public string? LugarTrabajo { get; init; }
    public string? HorarioTrabajo { get; init; }
    public int? Estatus { get; init; }
}
