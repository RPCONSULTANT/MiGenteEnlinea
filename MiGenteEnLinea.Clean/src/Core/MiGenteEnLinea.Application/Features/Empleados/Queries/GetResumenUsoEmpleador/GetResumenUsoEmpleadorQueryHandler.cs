using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Interfaces.Repositories;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.GetResumenUsoEmpleador;

/// <summary>
/// Handler para obtener resumen de uso actual del empleador.
/// 
/// LÓGICA DE NEGOCIO:
/// 1. Obtiene el empleador asociado al usuario
/// 2. Obtiene su plan actual y límites
/// 3. Cuenta empleados activos registrados
/// 4. Cuenta contratistas consultados en últimos 30 días (auditoría)
/// 5. Cuenta nóminas procesadas en el mes actual
/// 6. Calcula porcentajes y alertas
/// </summary>
public class GetResumenUsoEmpleadorQueryHandler : IRequestHandler<GetResumenUsoEmpleadorQuery, ResumenUsoEmpleadorDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetResumenUsoEmpleadorQueryHandler> _logger;

    // Límites de planes (definidos en configuración)
    private static readonly Dictionary<int, (int empleados, int contratistas, bool nomina)> PlanLimites = 
        new()
        {
            { 1, (empleados: 1, contratistas: 0, nomina: false) },
            { 2, (empleados: 5, contratistas: 1, nomina: false) },
            { 3, (empleados: 15, contratistas: 2, nomina: true) }
        };

    public GetResumenUsoEmpleadorQueryHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ILogger<GetResumenUsoEmpleadorQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task<ResumenUsoEmpleadorDto> Handle(
        GetResumenUsoEmpleadorQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo resumen de uso - UserId: {UserId}",
            request.UserId);

        try
        {
            // 1. Obtener el empleador
            var empleador = await _unitOfWork.Empleadores.GetByUserIdAsync(request.UserId);
            if (empleador == null)
            {
                _logger.LogWarning("Empleador no encontrado para UserId: {UserId}", request.UserId);
                throw new KeyNotFoundException($"No se encontró el empleador para el usuario {request.UserId}");
            }

            // 2. Obtener suscripción activa para obtener el plan
            var fechaActual = DateOnly.FromDateTime(DateTime.UtcNow);
            var suscripcion = await _context.Suscripciones
                .Where(s => s.UserId == request.UserId && !s.Cancelada && s.Vencimiento >= fechaActual)
                .FirstOrDefaultAsync(cancellationToken);

            int planId = suscripcion?.PlanId ?? 1; // Default a plan 1
            var (limitesEmpleados, limitesContratistas, inclujeNomina) = 
                PlanLimites.ContainsKey(planId) 
                    ? PlanLimites[planId] 
                    : PlanLimites[1];

            // 3. Contar empleados activos registrados por el empleador
            var empleadosRegistrados = await _context.Empleados
                .Where(e => e.UserId == empleador.UserId && e.Activo)
                .CountAsync(cancellationToken);

            // 4. Contar contratistas consultados en últimos 30 días
            // NOTA: Esto requiere una tabla de auditoría ContratistaConsultas
            int contratistasConsultados = 0;
            // Contratistas consultados no disponible sin tabla de auditoria.

            // 5. Contar nóminas procesadas en el mes actual
            var inicioDeMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var finDeMes = inicioDeMes.AddMonths(1).AddTicks(-1);

            int nominasProcesadas = 0;
            try
            {
                // Nota: Esto requiere acceso a EmpleadorRecibosHeader
                nominasProcesadas = await _context.Set<dynamic>()
                    .FromSqlInterpolated($@"
                        SELECT COUNT(DISTINCT id) 
                        FROM [EmpleadorRecibosHeader] 
                        WHERE UserId = {request.UserId}
                        AND FechaProcesamiento >= {inicioDeMes}
                        AND FechaProcesamiento <= {finDeMes}
                    ")
                    .CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al contar nóminas procesadas para UserId: {UserId}", request.UserId);
                nominasProcesadas = 0;
            }

            // 6. Construir y retornar el DTO
            var resultado = new ResumenUsoEmpleadorDto
            {
                EmpleadosRegistrados = empleadosRegistrados,
                LimiteEmpleados = limitesEmpleados,
                ContratistasConsultados = contratistasConsultados,
                LimiteContratistas = limitesContratistas,
                NominasProcesadasMes = nominasProcesadas,
                PlanInclujeNomina = inclujeNomina
            };

            _logger.LogInformation(
                "Resumen de uso obtenido - Empleados: {Empleados}/{Limite}, " +
                "Contratistas: {Contratistas}/{LimiteContratistas}, Nóminas: {Nominas}",
                resultado.EmpleadosRegistrados,
                resultado.LimiteEmpleados,
                resultado.ContratistasConsultados,
                resultado.LimiteContratistas,
                resultado.NominasProcesadasMes);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de uso para UserId: {UserId}", request.UserId);
            throw new InvalidOperationException(
                $"Error al obtener resumen de uso para el usuario {request.UserId}",
                ex);
        }
    }
}
