namespace MiGenteEnLinea.Web.Configuration;

public sealed class PaymentOptions
{
    public const string SectionName = "PaymentConfiguration";

    /// <summary>
    /// fake | real
    /// </summary>
    public string Mode { get; set; } = "fake";
}
