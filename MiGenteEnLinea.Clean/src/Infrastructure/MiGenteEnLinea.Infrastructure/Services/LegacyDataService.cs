using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.UpdateDetalleContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.CalificarContratacion;
using MiGenteEnLinea.Application.Features.Empleados.Commands.ModificarCalificacion;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using MiGenteEnLinea.Infrastructure.Persistence.Entities.Generated;
using System;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Implementación de ILegacyDataService usando raw SQL
/// Accede a tablas Legacy sin necesidad de entidades DDD completas
/// </summary>
public class LegacyDataService : ILegacyDataService
{
    private readonly MiGenteDbContext _context;

    public LegacyDataService(MiGenteDbContext context)
    {
        _context = context;
    }

    public async Task<ReciboContratacionDto?> GetReciboContratacionAsync(
        int pagoId,
        CancellationToken cancellationToken = default)
    {
        // Query identical to Legacy:
        // db.Empleador_Recibos_Header_Contrataciones.Where(x => x.pagoID == pagoID)
        //   .Include(h => h.Empleador_Recibos_Detalle_Contrataciones)
        //   .Include(f => f.EmpleadosTemporales).FirstOrDefault();

        var headerEntity = await _context
            .Set<EmpleadorRecibosHeaderContratacione>()
            .Where(x => x.PagoId == pagoId)
            .Include(h => h.EmpleadorRecibosDetalleContrataciones)
            .Include(f => f.Contratacion) // EmpleadoTemporal
            .FirstOrDefaultAsync(cancellationToken);

        if (headerEntity == null)
        {
            return null;
        }

        // Map to DTO
        var dto = new ReciboContratacionDto
        {
            PagoId = headerEntity.PagoId,
            UserId = headerEntity.UserId,
            ContratacionId = headerEntity.ContratacionId,
            FechaRegistro = headerEntity.FechaRegistro,
            FechaPago = headerEntity.FechaPago,
            ConceptoPago = headerEntity.ConceptoPago,
            Tipo = headerEntity.Tipo,
            Detalles = headerEntity.EmpleadorRecibosDetalleContrataciones
                .Select(d => new ReciboContratacionDetalleDto
                {
                    DetalleId = d.DetalleId,
                    PagoId = d.PagoId,
                    Concepto = d.Concepto,
                    Monto = d.Monto
                })
                .ToList()
        };

        // Map EmpleadoTemporal if exists
        if (headerEntity.Contratacion != null)
        {
            var emp = headerEntity.Contratacion;
            dto.EmpleadoTemporal = new EmpleadoTemporalSimpleDto
            {
                ContratacionId = emp.ContratacionId,
                Nombre = emp.Nombre,
                Apellido = emp.Apellido,
                Cedula = emp.Identificacion // In Legacy, "identificacion" is the cedula field
            };
        }

        return dto;
    }

    public async Task<List<PagoContratacionDto>> GetPagosContratacionesAsync(
        int contratacionId,
        int detalleId,
        CancellationToken cancellationToken = default)
    {
        // Legacy: SELECT from VPagosContrataciones view with filters
        var result = await _context
            .Set<VpagosContratacione>()
            .Where(x => x.ContratacionId == contratacionId && x.DetalleId == detalleId)
            .Select(x => new PagoContratacionDto
            {
                PagoId = x.PagoId,
                UserId = x.UserId,
                FechaRegistro = x.FechaRegistro,
                FechaPago = x.FechaPago,
                Expr1 = x.Expr1,
                Monto = x.Monto,
                ContratacionId = x.ContratacionId,
                DetalleId = x.DetalleId
            })
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<bool> CalificarContratacionAsync(
        int contratacionId,
        int calificacionId,
        CancellationToken cancellationToken = default)
    {
        // Legacy: Find DetalleContrataciones by contratacionID and set calificado=true + calificacionID
        var detalle = await _context
            .Set<DetalleContratacione>()
            .Where(x => x.ContratacionId == contratacionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (detalle == null)
            return false;

        // Set calificado flag and assign calificacionID
        detalle.Calificado = true;
        detalle.CalificacionId = calificacionId;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ModificarCalificacionAsync(
        ModificarCalificacionCommand command,
        CancellationToken cancellationToken = default)
    {
        // Legacy: Find Calificaciones by calificacionID and update all 9 fields
        var calificacion = await _context
            .Set<Calificacione>()
            .Where(x => x.CalificacionId == command.CalificacionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calificacion == null)
            return false;

        // Update all 9 fields from command
        calificacion.Identificacion = command.Identificacion ?? calificacion.Identificacion;
        calificacion.Conocimientos = command.Conocimientos ?? calificacion.Conocimientos;
        calificacion.Cumplimiento = command.Cumplimiento ?? calificacion.Cumplimiento;
        calificacion.Fecha = command.Fecha ?? calificacion.Fecha;
        calificacion.Nombre = command.Nombre ?? calificacion.Nombre;
        calificacion.Puntualidad = command.Puntualidad ?? calificacion.Puntualidad;
        calificacion.Recomendacion = command.Recomendacion ?? calificacion.Recomendacion;
        calificacion.Tipo = command.Tipo ?? calificacion.Tipo;
        calificacion.UserId = command.UserId ?? calificacion.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<EmpleadoTemporalDto?> GetFichaTemporalesAsync(
        int contratacionId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Legacy: Get EmpleadosTemporales with DetalleContrataciones included
        var empleadoTemporal = await _context
            .Set<Domain.Entities.Empleados.EmpleadoTemporal>()
            .Where(x => x.UserId == userId && x.ContratacionId == contratacionId)
            .Select(e => new EmpleadoTemporalDto
            {
                ContratacionId = e.ContratacionId,
                UserId = e.UserId,
                FechaRegistro = e.FechaRegistro,
                Tipo = e.Tipo,
                NombreComercial = e.NombreComercial,
                Rnc = e.Rnc,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Identificacion = e.Identificacion,
                Telefono1 = e.Telefono1,
                Direccion = e.Direccion,
                // Include DetalleContrataciones
                Detalle = _context.Set<Domain.Entities.Contrataciones.DetalleContratacion>()
                    .Where(d => d.ContratacionId == e.ContratacionId)
                    .Select(d => new DetalleContratacionDto
                    {
                        DetalleId = d.DetalleId,
                        ContratacionId = d.ContratacionId,
                        DescripcionCorta = d.DescripcionCorta,
                        DescripcionAmpliada = d.DescripcionAmpliada,
                        FechaInicio = d.FechaInicio,
                        FechaFinal = d.FechaFinal,
                        MontoAcordado = d.MontoAcordado,
                        EsquemaPagos = d.EsquemaPagos,
                        Estatus = d.Estatus,
                        Calificado = d.Calificado,
                        CalificacionId = d.CalificacionId
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return empleadoTemporal;
    }

    /// <summary>
    /// Obtiene todos los EmpleadosTemporales de un usuario con transformación de nombres
    /// Migrado de: EmpleadosService.obtenerTodosLosTemporales(string userID) - line 526
    /// 
    /// BUSINESS LOGIC (copied from Legacy):
    ///   - tipo == 1 (Individual): Nombre = Nombre + Apellido
    ///   - tipo == 2 (Business): Nombre = NombreComercial, Identificacion = Rnc
    /// </summary>
    public async Task<List<EmpleadoTemporalDto>> GetTodosLosTemporalesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Legacy: Query EmpleadosTemporales by userID with Include
        var empleadosTemporales = await _context
            .Set<Domain.Entities.Empleados.EmpleadoTemporal>()
            .Where(x => x.UserId == userId)
            .Select(e => new EmpleadoTemporalDto
            {
                ContratacionId = e.ContratacionId,
                UserId = e.UserId,
                FechaRegistro = e.FechaRegistro,
                Tipo = e.Tipo,
                NombreComercial = e.NombreComercial,
                Rnc = e.Rnc,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Identificacion = e.Identificacion,
                Telefono1 = e.Telefono1,
                Direccion = e.Direccion,
                // Include DetalleContrataciones
                Detalle = _context.Set<Domain.Entities.Contrataciones.DetalleContratacion>()
                    .Where(d => d.ContratacionId == e.ContratacionId)
                    .Select(d => new DetalleContratacionDto
                    {
                        DetalleId = d.DetalleId,
                        ContratacionId = d.ContratacionId,
                        DescripcionCorta = d.DescripcionCorta,
                        DescripcionAmpliada = d.DescripcionAmpliada,
                        FechaInicio = d.FechaInicio,
                        FechaFinal = d.FechaFinal,
                        MontoAcordado = d.MontoAcordado,
                        EsquemaPagos = d.EsquemaPagos,
                        Estatus = d.Estatus,
                        Calificado = d.Calificado,
                        CalificacionId = d.CalificacionId
                    })
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        // Legacy post-processing: Transform names based on tipo
        foreach (var empleado in empleadosTemporales)
        {
            if (empleado.Tipo == 1) // Individual
            {
                // Concatenate nombre + apellido
                empleado.Nombre = empleado.Nombre + " " + empleado.Apellido;
            }
            else if (empleado.Tipo == 2) // Business
            {
                // Use nombreComercial as nombre
                empleado.Nombre = empleado.NombreComercial;
                // Use rnc as identificacion
                empleado.Identificacion = empleado.Rnc;
            }
        }

        return empleadosTemporales;
    }

    /// <summary>
    /// Obtiene VistaContratacionTemporal por contratacionID y userID
    /// Migrado de: EmpleadosService.obtenerVistaTemporal(int contratacionID, string userID) - line 554
    /// </summary>
    public async Task<VistaContratacionTemporalDto?> GetVistaContratacionTemporalAsync(
        int contratacionId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Legacy: Query VistaContratacionTemporal view
        var vista = await _context.VistasContratacionTemporal
            .Where(x => x.UserId == userId && x.ContratacionId == contratacionId)
            .Select(v => new VistaContratacionTemporalDto
            {
                ContratacionId = v.ContratacionId,
                UserId = v.UserId,
                FechaRegistro = v.FechaRegistro,
                Tipo = v.Tipo,
                NombreComercial = v.NombreComercial,
                Rnc = v.Rnc,
                Identificacion = v.Identificacion,
                Nombre = v.Nombre,
                Apellido = v.Apellido,
                Alias = v.Alias,
                Direccion = v.Direccion,
                Provincia = v.Provincia,
                Municipio = v.Municipio,
                Telefono1 = v.Telefono1,
                Telefono2 = v.Telefono2,
                DetalleId = v.DetalleId,
                Expr1 = v.Expr1,
                DescripcionCorta = v.DescripcionCorta,
                DescripcionAmpliada = v.DescripcionAmpliada,
                FechaInicio = v.FechaInicio,
                FechaFinal = v.FechaFinal,
                MontoAcordado = v.MontoAcordado,
                EsquemaPagos = v.EsquemaPagos,
                Estatus = v.Estatus,
                ComposicionNombre = v.ComposicionNombre,
                ComposicionId = v.ComposicionId,
                Conocimientos = v.Conocimientos,
                Puntualidad = v.Puntualidad,
                Recomendacion = v.Recomendacion,
                Cumplimiento = v.Cumplimiento
            })
            .FirstOrDefaultAsync(cancellationToken);

        return vista;
    }

    /// <summary>
    /// Method #21: Obtiene Empleador_Recibos_Header completo con Detalle y Empleado
    /// Migrado de: EmpleadosService.GetEmpleador_ReciboByPagoID(int pagoID) - line 212
    /// Legacy: db.Empleador_Recibos_Header.Where(x => x.pagoID == pagoID)
    ///         .Include(h => h.Empleador_Recibos_Detalle)
    ///         .Include(f => f.Empleados).FirstOrDefault()
    /// </summary>
    public async Task<ReciboHeaderCompletoDto?> GetReciboHeaderByPagoIdAsync(
        int pagoId,
        CancellationToken cancellationToken = default)
    {
        var recibo = await _context
            .Set<EmpleadorRecibosHeader>()
            .Where(x => x.PagoId == pagoId)
            .Select(h => new ReciboHeaderCompletoDto
            {
                // Map header fields
                PagoId = h.PagoId,
                UserId = h.UserId,
                EmpleadoId = h.EmpleadoId,
                FechaRegistro = h.FechaRegistro,
                FechaPago = h.FechaPago,
                ConceptoPago = h.ConceptoPago,
                Tipo = h.Tipo,
                
                // Nested Select for Detalles (1:N)
                Detalles = _context
                    .Set<EmpleadorRecibosDetalle>()
                    .Where(d => d.PagoId == h.PagoId)
                    .Select(d => new EmpleadorReciboDetalleDto
                    {
                        DetalleId = d.DetalleId,
                        PagoId = d.PagoId,
                        Concepto = d.Concepto,
                        Monto = d.Monto
                    })
                    .ToList(),
                
                // Nested Select for Empleado (1:1)
                Empleado = h.EmpleadoId.HasValue
                    ? _context
                        .Set<Empleado>()
                        .Where(e => e.EmpleadoId == h.EmpleadoId.Value)
                        .Select(e => new EmpleadoBasicoDto
                        {
                            EmpleadoId = e.EmpleadoId,
                            Nombre = e.Nombre,
                            Apellido = e.Apellido,
                            Identificacion = e.Identificacion
                        })
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return recibo;
    }
}

