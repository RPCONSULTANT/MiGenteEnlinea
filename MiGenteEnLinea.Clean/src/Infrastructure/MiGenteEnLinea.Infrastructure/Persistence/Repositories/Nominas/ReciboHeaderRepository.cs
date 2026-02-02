using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Domain.Entities.Nominas;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Nominas;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.Infrastructure.Persistence.Repositories.Nominas;

/// <summary>
/// Implementación del repositorio para ReciboHeader.
/// </summary>
public class ReciboHeaderRepository : Repository<ReciboHeader>, IReciboHeaderRepository
{
    public ReciboHeaderRepository(MiGenteDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReciboHeader>> GetByEmpleadoIdAsync(
        int empleadoId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.EmpleadoId == empleadoId)
            .OrderByDescending(r => r.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReciboHeader>> GetByEmpleadorIdAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReciboHeader>> GetByPeriodoAsync(
        DateOnly periodoInicio, 
        DateOnly periodoFin, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.PeriodoInicio >= periodoInicio && r.PeriodoFin <= periodoFin)
            .OrderBy(r => r.PeriodoInicio)
            .ThenBy(r => r.EmpleadoId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReciboHeader>> GetByEstadoAsync(
        int estado, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Estado == estado)
            .OrderByDescending(r => r.FechaRegistro)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ReciboHeader?> GetWithDetallesAsync(
        int pagoId, 
        CancellationToken cancellationToken = default)
    {
        // Cargar header y detalles por separado para evitar problemas con IReadOnlyCollection
        var header = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.PagoId == pagoId, cancellationToken);
        
        if (header == null) return null;
        
        // Los detalles se cargarán automáticamente si el contexto tiene tracking,
        // o pueden ser consultados por separado usando el DbContext directamente
        return await _dbSet
            .Include("Detalles")
            .FirstOrDefaultAsync(r => r.PagoId == pagoId, cancellationToken);
    }
}
