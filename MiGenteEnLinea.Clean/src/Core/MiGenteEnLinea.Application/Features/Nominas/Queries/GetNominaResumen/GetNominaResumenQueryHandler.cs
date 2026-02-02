using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Nominas.DTOs;
using MiGenteEnLinea.Domain.Interfaces.Repositories;

namespace MiGenteEnLinea.Application.Features.Nominas.Queries.GetNominaResumen;

/// <summary>
/// Handler para obtener resumen de nómina por período.
/// 
/// LÓGICA DE NEGOCIO:
/// 1. Obtiene todos los recibos del empleador en el período
/// 2. Agrupa por empleado y tipo de concepto
/// 3. Calcula totales, promedios y estadísticas
/// 4. Retorna resumen consolidado con desglose opcional
/// </summary>
public class GetNominaResumenQueryHandler : IRequestHandler<GetNominaResumenQuery, NominaResumenDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetNominaResumenQueryHandler> _logger;

    public GetNominaResumenQueryHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ILogger<GetNominaResumenQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<NominaResumenDto> Handle(
        GetNominaResumenQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo resumen de nómina - Empleador: {EmpleadorId}, Período: {Periodo}",
            request.EmpleadorId,
            request.Periodo);

        // Validar empleador
        var empleador = await _unitOfWork.Empleadores.GetByIdAsync(request.EmpleadorId);
        if (empleador == null)
        {
            throw new KeyNotFoundException($"Empleador {request.EmpleadorId} no encontrado");
        }

        // Determinar rango de fechas
        DateOnly? fechaInicio = request.FechaInicio.HasValue 
            ? DateOnly.FromDateTime(request.FechaInicio.Value) 
            : null;
        DateOnly? fechaFin = request.FechaFin.HasValue 
            ? DateOnly.FromDateTime(request.FechaFin.Value) 
            : null;

        // Obtener recibos del período
        var recibosQuery = _context.RecibosHeader
            .Where(r => r.UserId == empleador.UserId);

        if (fechaInicio.HasValue && fechaFin.HasValue)
        {
            recibosQuery = recibosQuery.Where(r => 
                r.PeriodoInicio >= fechaInicio && 
                r.PeriodoFin <= fechaFin);
        }

        var recibos = await recibosQuery
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Contar empleados ACTIVOS del empleador (no recibos generados)
        var totalEmpleados = await _context.Empleados
            .Where(e => e.UserId == empleador.UserId && e.Activo)
            .CountAsync(cancellationToken);
        
        // Calcular totales (sin Include porque Detalles está ignorado en EF config)
        var totalSalarioBruto = recibos.Sum(r => r.TotalIngresos);
        var totalDeducciones = recibos.Sum(r => r.TotalDeducciones);
        var totalSalarioNeto = recibos.Sum(r => r.NetoPagar);

        // Obtener pagoIds para cargar detalles por separado
        var pagoIds = recibos.Select(r => r.PagoId).ToList();
        
        // Cargar detalles de deducciones directamente
        var deduccionesPorTipo = pagoIds.Any()
            ? await _context.RecibosDetalle
                .Where(d => pagoIds.Contains(d.PagoId) && d.Monto < 0) // Deducciones tienen monto negativo
                .GroupBy(d => d.Concepto)
                .Select(g => new { Concepto = g.Key, Total = g.Sum(d => Math.Abs(d.Monto)) })
                .ToDictionaryAsync(x => x.Concepto ?? "Otros", x => x.Total, cancellationToken)
            : new Dictionary<string, decimal>();

        // Calcular estadísticas
        var recibosGenerados = recibos.Count(r => r.EstaPagado() || r.EstaPendiente());
        var recibosAnulados = recibos.Count(r => r.EstaAnulado());
        var promedioSalarioBruto = totalEmpleados > 0 ? totalSalarioBruto / totalEmpleados : 0;
        var promedioSalarioNeto = totalEmpleados > 0 ? totalSalarioNeto / totalEmpleados : 0;

        // Crear DTO de respuesta
        var resumen = new NominaResumenDto
        {
            EmpleadorId = request.EmpleadorId,
            Periodo = request.Periodo,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            TotalEmpleados = totalEmpleados,
            TotalSalarioBruto = totalSalarioBruto,
            TotalDeducciones = totalDeducciones,
            TotalSalarioNeto = totalSalarioNeto,
            DeduccionesPorTipo = deduccionesPorTipo,
            RecibosGenerados = recibosGenerados,
            RecibosAnulados = recibosAnulados,
            PromedioSalarioBruto = promedioSalarioBruto,
            PromedioSalarioNeto = promedioSalarioNeto
        };

        // Agregar detalle por empleado si se solicita
        if (request.IncluirDetalleEmpleados)
        {
            resumen.DetalleEmpleados = recibos
                .GroupBy(r => r.EmpleadoId)
                .Select(g => new NominaEmpleadoDto
                {
                    EmpleadoId = g.Key,
                    NombreEmpleado = string.Empty, // TODO: Cargar desde Empleado
                    TotalRecibos = g.Count(),
                    TotalSalarioBruto = g.Sum(r => r.TotalIngresos),
                    TotalDeducciones = g.Sum(r => r.TotalDeducciones),
                    TotalSalarioNeto = g.Sum(r => r.NetoPagar),
                    PromedioSalarioBruto = g.Average(r => r.TotalIngresos),
                    PromedioSalarioNeto = g.Average(r => r.NetoPagar)
                })
                .ToList();
        }

        _logger.LogInformation(
            "Resumen generado - Empleados: {Empleados}, Recibos: {Recibos}, Total: {Total}",
            totalEmpleados,
            recibosGenerados,
            totalSalarioNeto);

        return resumen;
    }
}
