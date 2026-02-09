using MiGenteEnLinea.Domain.Entities.Empleadores;
using System.Linq.Expressions;

namespace MiGenteEnLinea.Domain.Interfaces.Repositories.Empleadores;

/// <summary>
/// Repositorio específico para la entidad Empleador
/// </summary>
/// <remarks>
/// Implementa el Repository Pattern para operaciones CRUD y queries específicas de negocio.
/// Hereda de IRepository&lt;Empleador&gt; para operaciones genéricas.
/// </remarks>
public interface IEmpleadorRepository : IRepository<Empleador>
{
    // ============================================
    // BÚSQUEDAS BÁSICAS
    // ============================================

    /// <summary>
    /// Obtiene un Empleador por su UserId (relación 1:1 con Credenciales)
    /// </summary>
    /// <param name="userId">ID del usuario en tabla Credenciales</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Empleador o null si no existe</returns>
    /// <remarks>
    /// Caso de uso: Obtener perfil de empleador autenticado
    /// </remarks>
    Task<Empleador?> GetByUserIdAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// Verifica si existe un Empleador para el UserId especificado
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>True si existe, False si no</returns>
    /// <remarks>
    /// Caso de uso: Validar que un usuario no sea empleador Y contratista simultáneamente
    /// </remarks>
    Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// Obtiene proyección DTO de un Empleador por UserId (sin tracking)
    /// </summary>
    /// <typeparam name="TResult">Tipo del DTO de resultado</typeparam>
    /// <param name="userId">ID del usuario</param>
    /// <param name="selector">Proyección LINQ (Select)</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>DTO o null si no existe</returns>
    /// <remarks>
    /// Caso de uso: Obtener perfil de empleador autenticado con proyección DTO
    /// </remarks>
    Task<TResult?> GetByUserIdProjectedAsync<TResult>(
        string userId,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class;

    // ============================================
    // BÚSQUEDAS PAGINADAS CON FILTROS
    // ============================================

    /// <summary>
    /// Busca empleadores con filtros y paginación
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda (opcional) - busca en Habilidades, Experiencia, Descripcion</param>
    /// <param name="pageNumber">Número de página (1-based)</param>
    /// <param name="pageSize">Cantidad de registros por página</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Tupla con lista de empleadores y total de registros</returns>
    /// <remarks>
    /// Caso de uso: Listado público de empleadores con búsqueda y paginación
    /// Ordenamiento: Por FechaPublicacion descendente, luego CreatedAt
    /// </remarks>
    Task<(IEnumerable<Empleador> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    // ============================================
    // PROYECCIONES DTO (OPTIMIZACIÓN)
    // ============================================

    /// <summary>
    /// Obtiene proyección DTO de un Empleador por ID (sin tracking)
    /// </summary>
    /// <typeparam name="TResult">Tipo del DTO de resultado</typeparam>
    /// <param name="id">ID del empleador</param>
    /// <param name="selector">Proyección LINQ (Select)</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>DTO o null si no existe</returns>
    /// <remarks>
    /// Caso de uso: Obtener solo propiedades necesarias para UI (optimización de consultas)
    /// Ejemplo: Select(e => new EmpleadorDto { EmpleadorId = e.Id, ... })
    /// </remarks>
    Task<TResult?> GetByIdProjectedAsync<TResult>(
        int id,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class;

    /// <summary>
    /// Busca empleadores con proyección DTO (sin tracking)
    /// </summary>
    /// <typeparam name="TResult">Tipo del DTO de resultado</typeparam>
    /// <param name="searchTerm">Término de búsqueda (opcional)</param>
    /// <param name="soloActivos">Filtrar solo empleadores activos (opcional)</param>
    /// <param name="sector">Filtrar por sector (opcional)</param>
    /// <param name="provincia">Filtrar por provincia (opcional)</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Cantidad de registros por página</param>
    /// <param name="selector">Proyección LINQ</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Tupla con lista de DTOs y total de registros</returns>
    Task<(IEnumerable<TResult> Items, int TotalCount)> SearchProjectedAsync<TResult>(
        string? searchTerm,
        bool? soloActivos,
        string? sector,
        string? provincia,
        int pageNumber,
        int pageSize,
        Expression<Func<Empleador, TResult>> selector,
        CancellationToken ct = default) where TResult : class;
}
