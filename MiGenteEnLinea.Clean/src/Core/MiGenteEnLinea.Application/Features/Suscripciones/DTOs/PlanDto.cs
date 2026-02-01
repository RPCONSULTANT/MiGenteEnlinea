using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Suscripciones.DTOs;

/// <summary>
/// DTO genérico para representar planes de empleadores y contratistas.
/// </summary>
/// <remarks>
/// Propiedades específicas de empleadores (LimiteEmpleados, etc.) 
/// serán null para planes de contratistas.
/// </remarks>
public record PlanDto
{
    /// <summary>
    /// ID único del plan.
    /// </summary>
    [JsonPropertyName("id")]
    public int PlanId { get; init; }

    /// <summary>
    /// Nombre del plan (ej: "Básico", "Pro", "Enterprise").
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Precio mensual en DOP (Pesos Dominicanos).
    /// </summary>
    public decimal Precio { get; init; }

    /// <summary>
    /// Duración del plan en meses.
    /// </summary>
    public int DuracionMeses { get; init; } = 12;

    /// <summary>
    /// Indica si el plan está disponible para compra.
    /// </summary>
    public bool Activo { get; init; }

    /// <summary>
    /// Tipo de plan: "Empleador" o "Contratista".
    /// </summary>
    public string TipoPlan { get; init; } = string.Empty;

    /// <summary>
    /// Número máximo de empleados permitidos.
    /// Solo aplica para planes de empleadores (null para contratistas).
    /// </summary>
    public int? LimiteEmpleados { get; init; }

    /// <summary>
    /// Meses de historial disponibles.
    /// Solo aplica para planes de empleadores (null para contratistas).
    /// </summary>
    public int? MesesHistorico { get; init; }

    /// <summary>
    /// Indica si el plan incluye procesamiento de nómina.
    /// Solo aplica para planes de empleadores (null para contratistas).
    /// </summary>
    public bool? IncluyeNomina { get; init; }

    /// <summary>
    /// Características del plan (para mostrar en UI).
    /// </summary>
    public List<string>? Caracteristicas { get; init; }
}
