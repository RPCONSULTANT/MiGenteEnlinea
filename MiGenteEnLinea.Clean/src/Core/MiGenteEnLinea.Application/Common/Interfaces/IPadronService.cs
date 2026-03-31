using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiGenteEnLinea.Application.Common.Interfaces;

/// <summary>
/// Servicio para consultar el Padrón Nacional Dominicano (cédulas de identidad).
/// Integración con API externa abcportal.online/Sigeinfo.
/// </summary>
public interface IPadronService
{
    /// <summary>
    /// Consulta una cédula en el Padrón Nacional Dominicano.
    /// </summary>
    /// <param name="cedula">Cédula de 11 dígitos (puede tener guiones: XXX-XXXXXXX-X)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Datos del ciudadano si existe, null si no se encuentra o hay error</returns>
    Task<PadronModel?> ConsultarCedulaAsync(string cedula, CancellationToken cancellationToken = default);
}

/// <summary>
/// Modelo de respuesta del Padrón Nacional Dominicano.
/// Contiene información básica del ciudadano consultado.
/// </summary>
public class PadronModel
{
    /// <summary>
    /// Cédula de identidad (11 dígitos sin guiones).
    /// </summary>
    public string Cedula { get; set; } = null!;

    /// <summary>
    /// Nombres del ciudadano (puede contener varios nombres).
    /// Ejemplo: "JUAN PABLO"
    /// </summary>
    public string Nombres { get; set; } = null!;

    /// <summary>
    /// Primer apellido del ciudadano.
    /// Ejemplo: "GARCIA"
    /// </summary>
    public string Apellido1 { get; set; } = null!;

    /// <summary>
    /// Segundo apellido del ciudadano (opcional).
    /// Ejemplo: "MARTINEZ"
    /// </summary>
    public string? Apellido2 { get; set; }

    /// <summary>
    /// Foto del ciudadano devuelta por el padrón.
    /// Puede venir como data URL base64.
    /// </summary>
    public string? Photo { get; set; }

    /// <summary>
    /// Fecha de nacimiento del ciudadano (opcional, puede no estar disponible).
    /// </summary>
    public DateTime? FechaNacimiento { get; set; }

    /// <summary>
    /// Lugar de nacimiento (municipio/provincia).
    /// </summary>
    public string? LugarNacimiento { get; set; }

    /// <summary>
    /// Estado civil del ciudadano.
    /// Valores comunes: "Soltero(a)", "Casado(a)", "Divorciado(a)", "Viudo(a)", "Unión libre"
    /// </summary>
    public string? EstadoCivil { get; set; }

    /// <summary>
    /// Ocupación declarada del ciudadano.
    /// </summary>
    public string? Ocupacion { get; set; }

    /// <summary>
    /// Nombre completo formateado (Nombres + Apellido1 + Apellido2).
    /// </summary>
    public string NombreCompleto => $"{Nombres} {Apellido1} {Apellido2}".Trim();

    /// <summary>
    /// Edad calculada a partir de la fecha de nacimiento (si está disponible).
    /// </summary>
    public int? Edad
    {
        get
        {
            if (!FechaNacimiento.HasValue)
                return null;

            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Value.Year;

            // Ajustar si aún no ha cumplido años este año
            if (FechaNacimiento.Value.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }
    }
}
