namespace MiGenteEnLinea.API.Configuration;

public class DatabaseSeedingSecurityOptions
{
    public const string SectionName = "DatabaseSeedingSecurity";

    public bool Enabled { get; set; } = true;
    public bool RequireHeaderKey { get; set; } = true;
    public string HeaderName { get; set; } = "X-Seed-Key";
    public string HeaderValue { get; set; } = string.Empty;
    public bool AllowDemoSeedInProduction { get; set; }
}
