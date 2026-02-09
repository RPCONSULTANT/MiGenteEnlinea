using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Domain.Entities.Empleadores;
using MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using System.Linq.Expressions;

namespace MiGenteEnLinea.Infrastructure.Persistence.Repositories.Empleadores;

/// <summary>
/// Implementación del repositorio Empleador con EF Core
/// </summary>
public class EmpleadorRepository : Repository<Empleador>, IEmpleadorRepository
{
    public EmpleadorRepository(MiGenteDbContext context) : base(context)
    {
    }

    // ============================================
    // BÚSQUEDAS BÁSICAS
    // ============================================

    public async Task<Empleador?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.UserId == userId, ct);
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet
            .AnyAsync(e => e.UserId == userId, ct);
    }

    public async Task<TResult?> GetByUserIdProjectedAsync<TResult>(
        string userId,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    // ============================================
    // BÚSQUEDAS PAGINADAS CON FILTROS
    // ============================================

    public async Task<(IEnumerable<Empleador> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        // Query base
        var query = _dbSet.AsNoTracking();

        // Filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();

            query = query.Where(e =>
                (e.Habilidades != null && e.Habilidades.ToLower().Contains(searchLower)) ||
                (e.Experiencia != null && e.Experiencia.ToLower().Contains(searchLower)) ||
                (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchLower))
            );
        }

        // Contar total
        var totalCount = await query.CountAsync(ct);

        // Paginación y ordenamiento
        var items = await query
            .OrderByDescending(e => e.FechaPublicacion ?? e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    // ============================================
    // PROYECCIONES DTO (OPTIMIZACIÓN)
    // ============================================

    public async Task<TResult?> GetByIdProjectedAsync<TResult>(
        int id,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IEnumerable<TResult> Items, int TotalCount)> SearchProjectedAsync<TResult>(
        string? searchTerm,
        bool? soloActivos,
        string? sector,
        string? provincia,
        int pageNumber,
        int pageSize,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class
    {
        // Query base
        var query = _dbSet.AsNoTracking();

        // Filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();

            query = query.Where(e =>
                (e.Habilidades != null && e.Habilidades.ToLower().Contains(searchLower)) ||
                (e.Experiencia != null && e.Experiencia.ToLower().Contains(searchLower)) ||
                (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchLower))
            );
        }

        // TODO: Implementar filtro soloActivos cuando el campo Activo exista en la entidad
        // if (soloActivos.HasValue)
        // {
        //     query = query.Where(e => e.Activo == soloActivos.Value);
        // }

        // TODO: Implementar filtro sector cuando se agregue relación con Perfil/Sector
        // if (!string.IsNullOrWhiteSpace(sector))
        // {
        //     query = query.Where(e => e.Sector == sector);
        // }

        // TODO: Implementar filtro provincia cuando se agregue relación con Perfil/Provincia
        // if (!string.IsNullOrWhiteSpace(provincia))
        // {
        //     query = query.Where(e => e.Provincia == provincia);
        // }

        // Contar total
        var totalCount = await query.CountAsync(ct);

        // Paginación, ordenamiento y proyección
        var items = await query
            .OrderByDescending(e => e.FechaPublicacion ?? e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
