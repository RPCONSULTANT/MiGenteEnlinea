using System;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.ConsultarPadron;

/// <summary>
/// DTO de respuesta para consulta del Padrón Nacional.
/// Contiene información básica del ciudadano consultado.
/// </summary>
public record PadronResultDto
{
    /// <summary>
    /// Cédula de identidad (11 dígitos sin guiones).
    /// </summary>
    public string Cedula { get; init; } = null!;

    /// <summary>
    /// Nombre completo formateado (Nombres + Apellidos).
    /// </summary>
    public string NombreCompleto { get; init; } = null!;

    /// <summary>
    /// Nombres del ciudadano.
    /// </summary>
    public string Nombres { get; init; } = null!;

    /// <summary>
    /// Primer apellido.
    /// </summary>
    public string PrimerApellido { get; init; } = null!;

    /// <summary>
    /// Segundo apellido (opcional).
    /// </summary>
    public string? SegundoApellido { get; init; }

    /// <summary>
    /// Foto devuelta por el padrón.
    /// Puede venir como data URL lista para previsualizar.
    /// </summary>
    public string? Photo { get; init; }

    /// <summary>
    /// Fecha de nacimiento (opcional).
    /// </summary>
    public DateTime? FechaNacimiento { get; init; }

    /// <summary>
    /// Edad calculada a partir de la fecha de nacimiento.
    /// </summary>
    public int? Edad { get; init; }

    /// <summary>
    /// Lugar de nacimiento (municipio/provincia).
    /// </summary>
    public string? LugarNacimiento { get; init; }

    /// <summary>
    /// Estado civil.
    /// Valores: "Soltero(a)", "Casado(a)", "Divorciado(a)", "Viudo(a)", "Unión libre"
    /// </summary>
    public string? EstadoCivil { get; init; }

    /// <summary>
    /// Ocupación declarada.
    /// </summary>
    public string? Ocupacion { get; init; }
}
