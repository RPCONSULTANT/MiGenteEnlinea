using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;
using MiGenteEnLinea.Domain.Entities.Empleados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Servicio REAL para cálculos de nómina.
/// Implementa la lógica de armarNovedad() del Legacy (fichaEmpleado.aspx.cs).
/// </summary>
public class NominaCalculatorService : INominaCalculatorService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<NominaCalculatorService> _logger;

    // Constantes TSS (Ley 87-01 de la Seguridad Social Dominicana)
    private const decimal TOPE_AFP_MENSUAL = 250000m; // Tope para cálculo AFP
    private const decimal TASA_AFP = 0.0287m; // 2.87% (empleado)
    private const decimal TASA_SFS = 0.0304m; // 3.04% (empleado)
    private const decimal TASA_SRL = 0.0100m; // 1.00% (empleado - Ley 87-01)

    public NominaCalculatorService(
        IApplicationDbContext context,
        ILogger<NominaCalculatorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NominaCalculoResult> CalcularNominaAsync(
        int empleadoId,
        DateTime fechaPago,
        string tipoConcepto,
        bool esFraccion,
        bool aplicarTss,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calculando nómina real para EmpleadoId={EmpleadoId}, Concepto={Concepto}, EsFraccion={EsFraccion}, AplicarTss={AplicarTss}",
            empleadoId, tipoConcepto, esFraccion, aplicarTss);

        // PASO 1: Obtener datos del empleado (salario, periodo de pago)
        var empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmpleadoId == empleadoId, cancellationToken)
            ?? throw new NotFoundException("Empleado", empleadoId);

        // PASO 2: Obtener remuneraciones extras del empleado
        var remuneraciones = await _context.Set<Remuneracion>()
            .AsNoTracking()
            .Where(r => r.EmpleadoId == empleadoId && r.Monto > 0)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Empleado: {Nombre}, Salario={Salario}, Periodo={Periodo}, Remuneraciones={CountRem}",
            $"{empleado.Nombre} {empleado.Apellido}",
            empleado.Salario,
            empleado.PeriodoPago,
            remuneraciones.Count);

        var result = new NominaCalculoResult();

        // PASO 3: CALCULAR PERCEPCIONES (salario + remuneraciones extras)
        
        // 3.1 Salario base (ajustado por periodo)
        decimal salarioBase = CalcularSalarioPorPeriodo(empleado.Salario, empleado.PeriodoPago, esFraccion);
        
        result.Percepciones.Add(new ConceptoNomina
        {
            EmpleadoId = empleadoId,
            Descripcion = esFraccion ? $"{tipoConcepto} (Fracción)" : tipoConcepto,
            Monto = salarioBase
        });

        // 3.2 Remuneraciones extras (bonos, incentivos, etc.)
        foreach (var remuneracion in remuneraciones)
        {
            result.Percepciones.Add(new ConceptoNomina
            {
                EmpleadoId = empleadoId,
                Descripcion = remuneracion.Descripcion ?? "Remuneración Adicional",
                Monto = remuneracion.Monto
            });
        }

        _logger.LogInformation(
            "Percepciones calculadas: {Count} conceptos, Total={Total:C}",
            result.Percepciones.Count,
            result.TotalPercepciones);

        // PASO 4: CALCULAR DEDUCCIONES TSS (si aplica)
        if (aplicarTss && tipoConcepto == "Salario") // TSS solo aplica a salarios, no a regalías
        {
            var deducciones = CalcularDeduccionesTss(salarioBase, empleado.PeriodoPago);
            result.Deducciones.AddRange(deducciones.Select(d => new ConceptoNomina
            {
                EmpleadoId = empleadoId,
                Descripcion = d.Descripcion,
                Monto = -Math.Abs(d.Monto) // Deducciones siempre negativas
            }));

            _logger.LogInformation(
                "Deducciones TSS calculadas: AFP={AFP:C}, SFS={SFS:C}, SRL={SRL:C}, Total={Total:C}",
                deducciones.FirstOrDefault(d => d.Descripcion.Contains("AFP"))?.Monto ?? 0,
                deducciones.FirstOrDefault(d => d.Descripcion.Contains("SFS"))?.Monto ?? 0,
                deducciones.FirstOrDefault(d => d.Descripcion.Contains("SRL"))?.Monto ?? 0,
                result.TotalDeducciones);
        }

        _logger.LogInformation(
            "Nómina calculada: Percepciones={Percepciones:C}, Deducciones={Deducciones:C}, Neto={Neto:C}",
            result.TotalPercepciones,
            result.TotalDeducciones,
            result.NetoPagar);

        return result;
    }

    /// <summary>
    /// Ajusta el salario según el período de pago y si es fracción.
    /// Legacy: armarNovedad() líneas 186-197
    /// </summary>
    private decimal CalcularSalarioPorPeriodo(decimal salario, int periodoPago, bool esFraccion)
    {
        if (esFraccion)
        {
            // Fracción: retornar el salario tal como está (el usuario ya lo ajustó)
            return salario;
        }

        // Período completo: no se ajusta, se paga el salario tal cual está registrado
        // Nota: El salario ya está guardado en el período correcto (semanal/quincenal/mensual)
        return salario;
    }

    /// <summary>
    /// Calcula las deducciones TSS (AFP, SFS, SRL) según la Ley 87-01.
    /// Legacy: armarNovedad() líneas 199-236
    /// </summary>
    private List<ConceptoNomina> CalcularDeduccionesTss(decimal salarioBase, int periodoPago)
    {
        var deducciones = new List<ConceptoNomina>();

        // PASO 1: Normalizar salario a mensual para aplicar topes TSS
        decimal salarioMensual = NormalizarSalarioAMensual(salarioBase, periodoPago);

        // PASO 2: Aplicar tope AFP (máximo RD$ 250,000/mes)
        decimal salarioParaAfp = Math.Min(salarioMensual, TOPE_AFP_MENSUAL);

        // PASO 3: Calcular deducciones mensuales
        decimal afpMensual = Math.Round(salarioParaAfp * TASA_AFP, 2);
        decimal sfsMensual = Math.Round(salarioMensual * TASA_SFS, 2);
        decimal srlMensual = Math.Round(salarioMensual * TASA_SRL, 2);

        // PASO 4: Proporcionar deducciones al período de pago
        decimal factorPeriodo = ObtenerFactorPeriodo(periodoPago);
        
        deducciones.Add(new ConceptoNomina
        {
            Descripcion = "AFP (Fondo de Pensiones)",
            Monto = Math.Round(afpMensual / factorPeriodo, 2)
        });

        deducciones.Add(new ConceptoNomina
        {
            Descripcion = "SFS (Seguro Familiar de Salud)",
            Monto = Math.Round(sfsMensual / factorPeriodo, 2)
        });

        deducciones.Add(new ConceptoNomina
        {
            Descripcion = "SRL (Seguro de Riesgos Laborales)",
            Monto = Math.Round(srlMensual / factorPeriodo, 2)
        });

        return deducciones;
    }

    /// <summary>
    /// Convierte cualquier salario a su equivalente mensual.
    /// </summary>
    private decimal NormalizarSalarioAMensual(decimal salario, int periodoPago)
    {
        return periodoPago switch
        {
            1 => salario * 4,    // Semanal → Mensual (x4)
            2 => salario * 2,    // Quincenal → Mensual (x2)
            3 => salario,        // Mensual → Mensual (x1)
            _ => salario
        };
    }

    /// <summary>
    /// Obtiene el factor de conversión mensual → período.
    /// </summary>
    private decimal ObtenerFactorPeriodo(int periodoPago)
    {
        return periodoPago switch
        {
            1 => 4,    // Mensual → Semanal (/4)
            2 => 2,    // Mensual → Quincenal (/2)
            3 => 1,    // Mensual → Mensual (/1)
            _ => 1
        };
    }
}
