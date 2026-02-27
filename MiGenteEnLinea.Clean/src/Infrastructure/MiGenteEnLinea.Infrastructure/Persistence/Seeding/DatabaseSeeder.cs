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

    public async Task<SeedExecutionReport> SeedAsync(bool includeDemoData)
    {
        _logger.LogInformation("db.seed.run.start type=full includeDemo={IncludeDemoData}", includeDemoData);

        var startedAt = DateTime.UtcNow;
        var blocks = new List<SeedBlockResult>();
        var errors = new List<string>();

        try
        {
            var catalogReport = await _catalogSeeder.SeedAsync();
            blocks.AddRange(catalogReport.Blocks);

            if (includeDemoData)
            {
                await _demoSeeder.SeedAsync();
                blocks.Add(new SeedBlockResult { BlockName = "DemoData", Inserted = 1 });
            }
            else
            {
                blocks.Add(new SeedBlockResult { BlockName = "DemoData", Skipped = 1 });
                _logger.LogInformation("Omitiendo seed de datos demo.");
            }

            _logger.LogInformation("db.seed.run.finish type=full success=true");
        }
        catch (Exception ex)
        {
            errors.Add(ex.Message);
            _logger.LogError(ex, "db.seed.run.finish type=full success=false");
            throw;
        }

        return new SeedExecutionReport
        {
            StartedAtUtc = startedAt,
            CompletedAtUtc = DateTime.UtcNow,
            Success = errors.Count == 0,
            Blocks = blocks,
            Errors = errors
        };
    }

    public Task<SeedExecutionReport> RepairPlansAsync() => _catalogSeeder.RepairPlansAsync();
}
