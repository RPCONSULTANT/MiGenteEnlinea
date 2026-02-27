using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiGenteEnLinea.API.Configuration;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using MiGenteEnLinea.Infrastructure.Persistence.Seeding;

namespace MiGenteEnLinea.API.Services;

public class DatabaseInitializationService : IDatabaseInitializationService
{
    private static readonly SemaphoreSlim SeedLock = new(1, 1);

    private readonly MiGenteDbContext _dbContext;
    private readonly CatalogDatabaseSeeder _catalogSeeder;
    private readonly DemoDatabaseSeeder _demoSeeder;
    private readonly DatabaseSeeder _databaseSeeder;
    private readonly DatabaseSeedingSecurityOptions _securityOptions;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        MiGenteDbContext dbContext,
        CatalogDatabaseSeeder catalogSeeder,
        DemoDatabaseSeeder demoSeeder,
        DatabaseSeeder databaseSeeder,
        IOptions<DatabaseSeedingSecurityOptions> securityOptions,
        IWebHostEnvironment environment,
        ILogger<DatabaseInitializationService> logger)
    {
        _dbContext = dbContext;
        _catalogSeeder = catalogSeeder;
        _demoSeeder = demoSeeder;
        _databaseSeeder = databaseSeeder;
        _securityOptions = securityOptions.Value;
        _environment = environment;
        _logger = logger;
    }

    public Task<SeedExecutionReport> RunCatalogSeedAsync(CancellationToken cancellationToken = default)
        => RunExclusiveAsync(() => _catalogSeeder.SeedAsync(), "catalog", cancellationToken);

    public Task<SeedExecutionReport> RunRepairPlansAsync(CancellationToken cancellationToken = default)
        => RunExclusiveAsync(() => _databaseSeeder.RepairPlansAsync(), "repair-plans", cancellationToken);

    public Task<SeedExecutionReport> RunMigrationsAndCatalogSeedAsync(CancellationToken cancellationToken = default)
        => RunExclusiveAsync(async () =>
        {
            _logger.LogInformation("db.seed.migrate.start");
            await _dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("db.seed.migrate.finish");
            return await _catalogSeeder.SeedAsync();
        }, "migrate-catalog", cancellationToken);

    public Task<SeedExecutionReport> RunDemoSeedAsync(CancellationToken cancellationToken = default)
        => RunExclusiveAsync(async () =>
        {
            if (_environment.IsProduction() && !_securityOptions.AllowDemoSeedInProduction)
            {
                throw new InvalidOperationException("Demo seed is disabled in Production.");
            }

            var startedAt = DateTime.UtcNow;
            await _demoSeeder.SeedAsync();
            return new SeedExecutionReport
            {
                StartedAtUtc = startedAt,
                CompletedAtUtc = DateTime.UtcNow,
                Success = true,
                Blocks = [new SeedBlockResult { BlockName = "DemoData", Inserted = 1 }]
            };
        }, "demo", cancellationToken);

    private async Task<SeedExecutionReport> RunExclusiveAsync(
        Func<Task<SeedExecutionReport>> action,
        string operationName,
        CancellationToken cancellationToken)
    {
        if (!await SeedLock.WaitAsync(0, cancellationToken))
        {
            return new SeedExecutionReport
            {
                StartedAtUtc = DateTime.UtcNow,
                CompletedAtUtc = DateTime.UtcNow,
                Success = false,
                Locked = true,
                Errors = ["Seeding is already running."]
            };
        }

        try
        {
            _logger.LogInformation("db.seed.operation.start operation={Operation}", operationName);
            var result = await action();
            _logger.LogInformation("db.seed.operation.finish operation={Operation} success={Success}", operationName, result.Success);
            return result;
        }
        finally
        {
            SeedLock.Release();
        }
    }
}
