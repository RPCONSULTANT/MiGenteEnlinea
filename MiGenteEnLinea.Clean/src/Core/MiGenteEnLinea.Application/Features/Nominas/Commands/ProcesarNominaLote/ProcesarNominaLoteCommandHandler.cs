using MediatR;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Empleados;
using MiGenteEnLinea.Domain.Entities.Nominas;
using MiGenteEnLinea.Domain.Interfaces.Repositories;

namespace MiGenteEnLinea.Application.Features.Nominas.Commands.ProcesarNominaLote;

/// <summary>
/// Handler para procesar nómina en lote (batch processing).
/// 
/// LÓGICA DE NEGOCIO:
/// 1. Valida que todos los empleados existan y pertenezcan al empleador
/// 2. Crea ReciboHeader + ReciboDetalle para cada empleado
/// 3. Calcula totales y deducciones automáticamente
/// 4. Opcionalmente genera PDFs y envía emails
/// 5. Registra errores individuales sin detener el proceso completo
/// </summary>
public class ProcesarNominaLoteCommandHandler : IRequestHandler<ProcesarNominaLoteCommand, ProcesarNominaLoteResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly INominaCalculatorService _nominaCalculatorService;
    private readonly ILogger<ProcesarNominaLoteCommandHandler> _logger;

    public ProcesarNominaLoteCommandHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        INominaCalculatorService nominaCalculatorService,
        ILogger<ProcesarNominaLoteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _nominaCalculatorService = nominaCalculatorService;
        _logger = logger;
    }

    public async Task<ProcesarNominaLoteResult> Handle(
        ProcesarNominaLoteCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Procesando nómina en lote - Empleador: {EmpleadorId}, Período: {Periodo}, Empleados: {Count}",
            request.EmpleadorId,
            request.Periodo,
            request.Empleados.Count > 0 ? request.Empleados.Count : request.EmpleadoIds.Count);

        var result = new ProcesarNominaLoteResult
        {
            ReciboIds = new List<int>(),
            Errores = new List<string>()
        };

        int recibosCreados = 0;
        int empleadosProcesados = 0;
        decimal totalPagado = 0;
        decimal totalDeducciones = 0;
        var reciboIds = new List<int>();
        var errores = new List<string>();

        var command = request;
        if (command.Empleados.Count == 0 && command.EmpleadoIds.Count > 0)
        {
            var userId = await ResolveUserIdAsync(command.EmpleadorId, cancellationToken);
            var empleados = new List<EmpleadoNominaItem>();
            foreach (var empleadoId in command.EmpleadoIds.Distinct())
            {
                var empleado = await _unitOfWork.Empleados.GetByIdAsync(empleadoId);
                if (empleado == null || !empleado.Activo || empleado.UserId != userId)
                {
                    errores.Add($"Empleado {empleadoId} no encontrado o no pertenece al empleador");
                    continue;
                }

                empleados.Add(new EmpleadoNominaItem
                {
                    EmpleadoId = empleadoId,
                    Salario = empleado.Salario,
                    AplicarTss = command.AplicarTss && empleado.InscritoTss
                });
            }

            command = command with { Empleados = empleados };
        }

        // Validar que empleador existe
        var empleador = await _unitOfWork.Empleadores.GetByIdAsync(command.EmpleadorId);
        if (empleador == null)
        {
            errores.Add($"Empleador {command.EmpleadorId} no encontrado");
            return new ProcesarNominaLoteResult
            {
                RecibosCreados = 0,
                EmpleadosProcesados = 0,
                TotalPagado = 0,
                TotalDeducciones = 0,
                ReciboIds = new List<int>(),
                Errores = errores
            };
        }

        // Procesar cada empleado individualmente
        foreach (var empleadoItem in command.Empleados)
        {
            try
            {
                // Validar que empleado existe y pertenece al empleador
                var empleado = await _unitOfWork.Empleados.GetByIdAsync(empleadoItem.EmpleadoId);
                if (empleado == null)
                {
                    errores.Add($"Empleado {empleadoItem.EmpleadoId} no encontrado");
                    continue;
                }

                if (empleado.UserId != empleador.UserId)
                {
                    errores.Add($"Empleado {empleadoItem.EmpleadoId} no pertenece al empleador");
                    continue;
                }

                var calculoNomina = await _nominaCalculatorService.CalcularNominaAsync(
                    empleadoItem.EmpleadoId,
                    command.FechaPago,
                    command.TipoConcepto,
                    command.EsFraccion,
                    empleadoItem.AplicarTss,
                    cancellationToken);

                // Crear ReciboHeader usando factory method con firmas correctas
                var reciboHeader = ReciboHeader.Create(
                    userId: empleador.UserId,
                    empleadoId: empleadoItem.EmpleadoId,
                    conceptoPago: string.IsNullOrWhiteSpace(command.Periodo)
                        ? $"Nómina {command.TipoConcepto}"
                        : $"Nómina {command.Periodo}",
                    tipo: 1, // Tipo 1 = Nómina Regular
                    periodoInicio: DateOnly.FromDateTime(command.FechaPago.AddDays(-14)),
                    periodoFin: DateOnly.FromDateTime(command.FechaPago)
                );

                foreach (var percepcion in calculoNomina.Percepciones)
                {
                    reciboHeader.AgregarIngreso(percepcion.Descripcion, percepcion.Monto);
                }

                foreach (var concepto in empleadoItem.Conceptos)
                {
                    if (concepto.EsDeduccion)
                    {
                        reciboHeader.AgregarDeduccion(concepto.Concepto, concepto.Monto);
                    }
                    else
                    {
                        reciboHeader.AgregarIngreso(concepto.Concepto, concepto.Monto);
                    }
                }

                foreach (var deduccion in calculoNomina.Deducciones)
                {
                    reciboHeader.AgregarDeduccion(deduccion.Descripcion, Math.Abs(deduccion.Monto));
                }

                // Guardar en base de datos
                await _unitOfWork.RecibosHeader.AddAsync(reciboHeader);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Actualizar contadores
                recibosCreados++;
                empleadosProcesados++;
                totalPagado += reciboHeader.NetoPagar;
                totalDeducciones += reciboHeader.TotalDeducciones;
                reciboIds.Add(reciboHeader.PagoId);

                _logger.LogInformation(
                    "Recibo creado - ID: {PagoId}, Empleado: {EmpleadoId}, Neto: {Monto}",
                    reciboHeader.PagoId,
                    empleadoItem.EmpleadoId,
                    reciboHeader.NetoPagar);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error procesando empleado {EmpleadoId}",
                    empleadoItem.EmpleadoId);

                errores.Add($"Error procesando empleado {empleadoItem.EmpleadoId}: {ex.Message}");
            }
        }

        _logger.LogInformation(
            "Nómina lote procesada - Recibos: {Recibos}, Empleados: {Empleados}, Total: {Total}",
            recibosCreados,
            empleadosProcesados,
            totalPagado);

        return new ProcesarNominaLoteResult
        {
            RecibosCreados = recibosCreados,
            EmpleadosProcesados = empleadosProcesados,
            TotalPagado = totalPagado,
            TotalDeducciones = totalDeducciones,
            ReciboIds = reciboIds,
            Errores = errores
        };
    }

    private async Task<string> ResolveUserIdAsync(int empleadorId, CancellationToken cancellationToken)
    {
        var empleador = await _unitOfWork.Empleadores.GetByIdAsync(empleadorId);
        if (empleador != null)
        {
            return empleador.UserId;
        }

        return empleadorId.ToString();
    }
}
