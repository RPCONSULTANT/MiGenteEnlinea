namespace MiGenteEnLinea.Infrastructure.Options;

/// <summary>
/// Configuración de modo de procesamiento de pagos.
/// </summary>
public sealed class PaymentProcessingOptions
{
    public const string SectionName = "PaymentProcessing";

    /// <summary>
    /// Modo operativo: Fake o Real.
    /// </summary>
    public string Mode { get; set; } = "Fake";

    /// <summary>
    /// Habilita endpoint de checkout simple sin tarjeta.
    /// </summary>
    public bool AllowSimpleCheckout { get; set; } = true;

    /// <summary>
    /// Si true, aunque esté en Fake se mantiene validación de tarjeta.
    /// </summary>
    public bool RequireCardValidationInFakeMode { get; set; } = false;

    public bool IsFakeMode()
    {
        return string.Equals(Mode, "Fake", StringComparison.OrdinalIgnoreCase);
    }
}
