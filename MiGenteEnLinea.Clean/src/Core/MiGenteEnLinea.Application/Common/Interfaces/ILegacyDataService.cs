using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CalificarContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.ModificarCalificacion;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;

namespace MiGenteEnLinea.Application.Common.Interfaces;

/// <summary>
/// Servicio para acceder a tablas Legacy que no están migradas a DDD
/// Evita dependencia circular entre Application e Infrastructure
/// </summary>
public interface ILegacyDataService
{
    /// <summary>
    /// Obtiene un recibo de contratación con su detalle y empleado temporal
    /// Migrado de: EmpleadosService.GetContratacion_ReciboByPagoID(int pagoID)
    /// </summary>
    Task<ReciboContratacionDto?> GetReciboContratacionAsync(int pagoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un empleado temporal con todos sus recibos asociados (cascade delete)
    /// Migrado de: EmpleadosService.eliminarEmpleadoTemporal(int contratacionID)
    /// Orden de eliminación:
    /// 1. Empleador_Recibos_Detalle_Contrataciones (para cada recibo)
    /// 2. Empleador_Recibos_Header_Contrataciones (para cada recibo)
    /// 3. EmpleadosTemporales (el empleado temporal)
    /// </summary>
    Task<bool> EliminarEmpleadoTemporalAsync(int contratacionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene pagos de contrataciones desde la vista VPagosContrataciones
    /// Migrado de: EmpleadosService.GetEmpleador_RecibosContratacionesByID(int contratacionID, int detalleID)
    /// </summary>
    Task<List<PagoContratacionDto>> GetPagosContratacionesAsync(int contratacionId, int detalleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo EmpleadoTemporal y su DetalleContrataciones asociado
    /// Migrado de: EmpleadosService.nuevoTemporal(EmpleadosTemporales temp, DetalleContrataciones det)
    /// Usa dos transacciones como en Legacy (2 DbContexts)
    /// </summary>
    Task<int> CreateEmpleadoTemporalAsync(
        CreateEmpleadoTemporalCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo DetalleContrataciones
    /// Migrado de: EmpleadosService.nuevaContratacionTemporal(DetalleContrataciones det)
    /// </summary>
    Task<int> CreateDetalleContratacionAsync(
        CreateDetalleContratacionCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un DetalleContrataciones existente
    /// Migrado de: EmpleadosService.actualizarContratacion(DetalleContrataciones det)
    /// </summary>
    Task<bool> UpdateDetalleContratacionAsync(
        UpdateDetalleContratacionCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca una contratación como calificada (calificado=true, asigna calificacionID)
    /// Migrado de: EmpleadosService.calificarContratacion(int contratacionID, int calificacionID)
    /// </summary>
    Task<bool> CalificarContratacionAsync(
        int contratacionId,
        int calificacionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifica una calificación existente (updates 9 fields in Calificaciones table)
    /// Migrado de: EmpleadosService.modificarCalificacionDeContratacion(Calificaciones cal)
    /// </summary>
    Task<bool> ModificarCalificacionAsync(
        ModificarCalificacionCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene EmpleadosTemporales con DetalleContrataciones incluido
    /// Migrado de: EmpleadosService.obtenerFichaTemporales(int contratacionID, string userID)
    /// </summary>
    Task<EmpleadoTemporalDto?> GetFichaTemporalesAsync(
        int contratacionId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los EmpleadosTemporales de un usuario con transformación de nombres
    /// Migrado de: EmpleadosService.obtenerTodosLosTemporales(string userID) - line 526
    /// Business Logic:
    ///   - tipo == 1 (Individual): Nombre = Nombre + Apellido
    ///   - tipo == 2 (Business): Nombre = NombreComercial, Identificacion = Rnc
    /// </summary>
    Task<List<EmpleadoTemporalDto>> GetTodosLosTemporalesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene VistaContratacionTemporal por contratacionID y userID
    /// Migrado de: EmpleadosService.obtenerVistaTemporal(int contratacionID, string userID) - line 554
    /// </summary>
    Task<VistaContratacionTemporalDto?> GetVistaContratacionTemporalAsync(
        int contratacionId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene Recibo Header completo con Detalle y Empleado por PagoID
    /// Migrado de: EmpleadosService.GetEmpleador_ReciboByPagoID(int pagoID) - line 212
    /// </summary>
    Task<ReciboHeaderCompletoDto?> GetReciboHeaderByPagoIdAsync(
        int pagoId,
        CancellationToken cancellationToken = default);
}
