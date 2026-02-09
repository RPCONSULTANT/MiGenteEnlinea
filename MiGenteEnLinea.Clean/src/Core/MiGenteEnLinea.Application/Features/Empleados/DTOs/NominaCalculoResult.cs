using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Empleados.DTOs;

/// <summary>
/// Resultado del cálculo de nómina con percepciones y deducciones.
/// Generado por INominaCalculatorService.
/// </summary>
public class NominaCalculoResult
{
    [JsonPropertyName("percepciones")]
    public List<ConceptoNomina> Percepciones { get; set; } = new();
    [JsonPropertyName("deducciones")]
    public List<ConceptoNomina> Deducciones { get; set; } = new();

    /// <summary>
    /// Total de percepciones (salario + extras). Valores positivos.
    /// </summary>
    [JsonPropertyName("totalPercepciones")]
    public decimal TotalPercepciones => Percepciones.Sum(x => x.Monto);

    /// <summary>
    /// Total de deducciones (TSS). Valores absolutos (sin signo negativo).
    /// </summary>
    [JsonPropertyName("totalDeducciones")]
    public decimal TotalDeducciones => Deducciones.Sum(x => Math.Abs(x.Monto));

    /// <summary>
    /// Neto a pagar = Percepciones - Deducciones
    /// </summary>
    [JsonPropertyName("netoPagar")]
    public decimal NetoPagar => TotalPercepciones - TotalDeducciones;
}

/// <summary>
/// Representa un concepto de nómina (percepción o deducción).
/// </summary>
public class ConceptoNomina
{
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = null!;
    
    /// <summary>
    /// Monto del concepto.
    /// POSITIVO para percepciones (salario, bonos).
    /// NEGATIVO para deducciones (TSS).
    /// </summary>
    [JsonPropertyName("monto")]
    public decimal Monto { get; set; }

    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
}
