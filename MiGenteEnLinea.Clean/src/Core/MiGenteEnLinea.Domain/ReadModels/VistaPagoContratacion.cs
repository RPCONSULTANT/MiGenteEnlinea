namespace MiGenteEnLinea.Domain.ReadModels;

/// <summary>
/// Vista de solo lectura para consultas de pagos a contratistas temporales
/// </summary>
/// <remarks>
/// Esta vista agrega información de recibos de pago por contrataciones
/// con sus detalles para obtener el monto total. Similar a VistaPago
/// pero para contrataciones temporales en lugar de empleados permanentes.
/// </remarks>
public sealed class VistaPagoContratacion
{
    /// <summary>
    /// ID del pago
    /// </summary>
    public int PagoId { get; init; }

    /// <summary>
    /// ID del usuario empleador que realizó el pago
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Fecha de registro del recibo
    /// </summary>
    public DateTime? FechaRegistro { get; init; }

    /// <summary>
    /// Fecha en que se realizó el pago
    /// </summary>
    public DateTime? FechaPago { get; init; }

    /// <summary>
    /// Campo de expresión 1 (auxiliar)
    /// </summary>
    public string? Expr1 { get; init; }

    /// <summary>
    /// Monto total del pago (suma de todos los detalles)
    /// </summary>
    public decimal? Monto { get; init; }

    /// <summary>
    /// ID de la contratación temporal asociada
    /// </summary>
    public int? ContratacionId { get; init; }

    /// <summary>
    /// ID del detalle de contratación asociado
    /// </summary>
    public int? DetalleId { get; init; }
}
