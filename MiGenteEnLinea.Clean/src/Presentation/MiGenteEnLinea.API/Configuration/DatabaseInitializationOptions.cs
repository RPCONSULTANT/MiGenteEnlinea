namespace MiGenteEnLinea.API.Configuration;

public class DatabaseInitializationOptions
{
    public const string SectionName = "DatabaseInitialization";

    public bool ApplyMigrationsOnStartup { get; set; }
    public bool RunCatalogSeedOnStartup { get; set; }
    public bool RunDemoSeedOnStartup { get; set; }
    public bool FailFastOnInitializationError { get; set; }

    public static DatabaseInitializationOptions CreateDefaults(string environmentName)
    {
        var isProduction = string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase);

        return new DatabaseInitializationOptions
        {
            ApplyMigrationsOnStartup = !isProduction,
            RunCatalogSeedOnStartup = !isProduction,
            RunDemoSeedOnStartup = false,
            FailFastOnInitializationError = true
        };
    }
}
