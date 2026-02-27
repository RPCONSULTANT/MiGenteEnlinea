using MiGenteEnLinea.Infrastructure.Persistence.Seeding;

namespace MiGenteEnLinea.API.Services;

public interface IDatabaseInitializationService
{
    Task<SeedExecutionReport> RunCatalogSeedAsync(CancellationToken cancellationToken = default);
    Task<SeedExecutionReport> RunDemoSeedAsync(CancellationToken cancellationToken = default);
    Task<SeedExecutionReport> RunRepairPlansAsync(CancellationToken cancellationToken = default);
    Task<SeedExecutionReport> RunMigrationsAndCatalogSeedAsync(CancellationToken cancellationToken = default);
}
