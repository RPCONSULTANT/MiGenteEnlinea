using MediatR;

namespace MiGenteEnLinea.Application.Features.Nominas.Commands.ProcesarNominaLote;

/// <summary>
/// Command para procesar nómina en lote (batch processing).
/// Permite procesar pagos para múltiples empleados simultáneamente.
/// </summary>
public record ProcesarNominaLoteCommand : IRequest<ProcesarNominaLoteResult>
{
    /// <summary>
    /// ID del empleador que procesa la nómina
    /// </summary>
    public int EmpleadorId { get; init; }

    /// <summary>
    /// Período de pago (ej: "2025-01", "2025-Q1")
    /// </summary>
    public string Periodo { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de pago
    /// </summary>
    public DateTime FechaPago { get; init; }

    /// <summary>
    /// Concepto general de los pagos del lote.
    /// </summary>
    public string TipoConcepto { get; init; } = "Salario";

    /// <summary>
    /// Indica si el lote debe calcular pago fraccionado del período.
    /// </summary>
    public bool EsFraccion { get; init; }

    /// <summary>
    /// Aplicar TSS automáticamente a empleados que lo permitan.
    /// </summary>
    public bool AplicarTss { get; init; } = true;

    /// <summary>
    /// Compatibilidad con el frontend actual: IDs simples de empleados a procesar.
    /// </summary>
    public List<int> EmpleadoIds { get; init; } = new();

    /// <summary>
    /// Lista de empleados a procesar con sus detalles de pago
    /// </summary>
    public List<EmpleadoNominaItem> Empleados { get; init; } = new();

    /// <summary>
    /// Notas generales para todos los recibos
    /// </summary>
    public string? Notas { get; init; }

    /// <summary>
    /// Generar PDFs automáticamente después de procesar
    /// </summary>
    public bool GenerarPdfs { get; init; } = true;

    /// <summary>
    /// Enviar recibos por email automáticamente
    /// </summary>
    public bool EnviarEmails { get; init; } = false;
}

/// <summary>
/// Item de nómina para un empleado individual
/// </summary>
public record EmpleadoNominaItem
{
    public int EmpleadoId { get; init; }
    public decimal Salario { get; init; }
    public bool AplicarTss { get; init; } = true;
    public List<ConceptoNominaItem> Conceptos { get; init; } = new();
}

/// <summary>
/// Concepto de nómina (ingreso o deducción)
/// </summary>
public record ConceptoNominaItem
{
    public string Concepto { get; init; } = string.Empty;
    public decimal Monto { get; init; }
    public string? Detalle { get; init; }
    public bool EsDeduccion { get; init; } // true = deducción, false = ingreso
}

/// <summary>
/// Resultado del procesamiento en lote
/// </summary>
public record ProcesarNominaLoteResult
{
    public int RecibosCreados { get; init; }
    public int EmpleadosProcesados { get; init; }
    public decimal TotalPagado { get; init; }
    public decimal TotalDeducciones { get; init; }
    public List<int> ReciboIds { get; init; } = new();
    public List<string> Errores { get; init; } = new();
    public bool Exitoso => Errores.Count == 0;
}
