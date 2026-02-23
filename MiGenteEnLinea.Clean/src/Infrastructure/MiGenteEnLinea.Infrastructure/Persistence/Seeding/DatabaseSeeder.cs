using Microsoft.Extensions.Logging;

namespace MiGenteEnLinea.Infrastructure.Persistence.Seeding;

/// <summary>
/// Orquestador de seeding para mantener compatibilidad con llamadas existentes.
/// </summary>
public class DatabaseSeeder
{
    private readonly CatalogDatabaseSeeder _catalogSeeder;
    private readonly DemoDatabaseSeeder _demoSeeder;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        CatalogDatabaseSeeder catalogSeeder,
        DemoDatabaseSeeder demoSeeder,
        ILogger<DatabaseSeeder> logger)
    {
        _catalogSeeder = catalogSeeder;
        _demoSeeder = demoSeeder;
        _logger = logger;
    }

    public async Task SeedAsync(bool includeDemoData)
    {
        _logger.LogInformation("Iniciando seeding de base de datos. DemoData={IncludeDemoData}", includeDemoData);

        await _catalogSeeder.SeedAsync();

        if (includeDemoData)
        {
            await _demoSeeder.SeedAsync();
        }
        else
        {
            _logger.LogInformation("Omitiendo seed de datos demo.");
        }

        _logger.LogInformation("Seeding completado exitosamente.");
    }
}
