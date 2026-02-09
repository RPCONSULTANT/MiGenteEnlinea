using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetResumenUsoEmpleador;

/// <summary>
/// Query para obtener resumen de uso actual del empleador.
/// Incluye: empleados registrados, contratistas consultados, nóminas procesadas, límites del plan.
/// </summary>
public record GetResumenUsoEmpleadorQuery : IRequest<ResumenUsoEmpleadorDto>
{
    /// <summary>
    /// ID del usuario (empleador)
    /// </summary>
    public string UserId { get; init; } = string.Empty;
}

/// <summary>
/// DTO con resumen de uso del empleador
/// </summary>
public class ResumenUsoEmpleadorDto
{
    /// <summary>
    /// Cantidad de empleados activos registrados
    /// </summary>
    public int EmpleadosRegistrados { get; set; }

    /// <summary>
    /// Límite de empleados según el plan actual
    /// </summary>
    public int LimiteEmpleados { get; set; }

    /// <summary>
    /// Cantidad de contratistas consultados en últimos 30 días
    /// </summary>
    public int ContratistasConsultados { get; set; }

    /// <summary>
    /// Límite de contratistas consultables según el plan actual
    /// </summary>
    public int LimiteContratistas { get; set; }

    /// <summary>
    /// Cantidad de nóminas procesadas en el mes actual
    /// </summary>
    public int NominasProcesadasMes { get; set; }

    /// <summary>
    /// Indica si el plan incluye nómina
    /// </summary>
    public bool PlanInclujeNomina { get; set; }

    /// <summary>
    /// Porcentaje de empleados usados (0-100)
    /// </summary>
    public decimal PorcentajeEmpleados => LimiteEmpleados > 0 
        ? (decimal)EmpleadosRegistrados / LimiteEmpleados * 100 
        : 0;

    /// <summary>
    /// Porcentaje de contratistas consultados (0-100)
    /// </summary>
    public decimal PorcentajeContratistas => LimiteContratistas > 0 
        ? (decimal)ContratistasConsultados / LimiteContratistas * 100 
        : 0;

    /// <summary>
    /// Determina si el empleador está cerca del límite (>80%)
    /// </summary>
    public bool EmpleadosCercaDeLimite => PorcentajeEmpleados >= 80;
}
