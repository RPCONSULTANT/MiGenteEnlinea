namespace MiGenteEnLinea.Application.Features.Nominas.DTOs;

/// <summary>
/// DTO para mostrar un registro del histórico de nómina en el listado
/// </summary>
public class NominaHistorialDto
{
    /// <summary>
    /// ID de la nómina (EmpleadorRecibosHeader.Id)
    /// </summary>
    public int NominaId { get; set; }

    /// <summary>
    /// Período de la nómina (ej: "Enero 2025")
    /// </summary>
    public string Periodo { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de empleados procesados en esta nómina
    /// </summary>
    public int CantidadEmpleados { get; set; }

    /// <summary>
    /// Monto total de la nómina procesada
    /// </summary>
    public decimal TotalNomina { get; set; }

    /// <summary>
    /// Fecha de procesamiento de la nómina
    /// </summary>
    public DateTime FechaProcesamiento { get; set; }

    /// <summary>
    /// Estado: 1 = Procesado, 2 = Parcial, 3 = Error, etc.
    /// </summary>
    public int Estado { get; set; }

    /// <summary>
    /// Descripción del estado
    /// </summary>
    public string EstadoTexto { get; set; } = "Procesado";

    /// <summary>
    /// Indica si los recibos fueron enviados por email
    /// </summary>
    public bool EmailEnviado { get; set; }

    /// <summary>
    /// Fecha de envío de emails (opcional)
    /// </summary>
    public DateTime? FechaEnvioEmail { get; set; }

    /// <summary>
    /// Nota o comentario sobre la nómina
    /// </summary>
    public string? Notas { get; set; }
}
