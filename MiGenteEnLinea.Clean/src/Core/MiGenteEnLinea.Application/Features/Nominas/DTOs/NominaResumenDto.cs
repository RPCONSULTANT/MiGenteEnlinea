using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Nominas.DTOs;

/// <summary>
/// DTO con resumen completo de nómina por período
/// </summary>
public class NominaResumenDto
{
    [JsonPropertyName("empleadorId")]
    public int EmpleadorId { get; set; }
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;
    [JsonPropertyName("fechaInicio")]
    public DateTime? FechaInicio { get; set; }
    [JsonPropertyName("fechaFin")]
    public DateTime? FechaFin { get; set; }
    
    // Totales generales
    [JsonPropertyName("totalEmpleados")]
    public int TotalEmpleados { get; set; }
    [JsonPropertyName("totalSalarioBruto")]
    public decimal TotalSalarioBruto { get; set; }
    [JsonPropertyName("totalDeducciones")]
    public decimal TotalDeducciones { get; set; }
    [JsonPropertyName("totalSalarioNeto")]
    public decimal TotalSalarioNeto { get; set; }
    
    // Desglose de deducciones por tipo (ej: AFP, SFS, ISR, etc.)
    [JsonPropertyName("deduccionesPorTipo")]
    public Dictionary<string, decimal> DeduccionesPorTipo { get; set; } = new();
    
    // Estadísticas
    [JsonPropertyName("recibosGenerados")]
    public int RecibosGenerados { get; set; }
    [JsonPropertyName("recibosAnulados")]
    public int RecibosAnulados { get; set; }
    [JsonPropertyName("promedioSalarioBruto")]
    public decimal PromedioSalarioBruto { get; set; }
    [JsonPropertyName("promedioSalarioNeto")]
    public decimal PromedioSalarioNeto { get; set; }
    
    // Detalle por empleado (opcional)
    [JsonPropertyName("detalleEmpleados")]
    public List<NominaEmpleadoDto> DetalleEmpleados { get; set; } = new();
}

/// <summary>
/// DTO con resumen de nómina para un empleado específico
/// </summary>
public class NominaEmpleadoDto
{
    [JsonPropertyName("empleadoId")]
    public int EmpleadoId { get; set; }
    [JsonPropertyName("nombreEmpleado")]
    public string NombreEmpleado { get; set; } = string.Empty;
    
    [JsonPropertyName("totalRecibos")]
    public int TotalRecibos { get; set; }
    [JsonPropertyName("totalSalarioBruto")]
    public decimal TotalSalarioBruto { get; set; }
    [JsonPropertyName("totalDeducciones")]
    public decimal TotalDeducciones { get; set; }
    [JsonPropertyName("totalSalarioNeto")]
    public decimal TotalSalarioNeto { get; set; }
    
    [JsonPropertyName("promedioSalarioBruto")]
    public decimal PromedioSalarioBruto { get; set; }
    [JsonPropertyName("promedioSalarioNeto")]
    public decimal PromedioSalarioNeto { get; set; }
}

/// <summary>
/// DTO para estadísticas avanzadas de nómina
/// </summary>
public class EstadisticasNominaDto
{
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;
    
    // Métricas generales
    [JsonPropertyName("totalEmpleadosActivos")]
    public int TotalEmpleadosActivos { get; set; }
    [JsonPropertyName("totalEmpleadosInactivos")]
    public int TotalEmpleadosInactivos { get; set; }
    [JsonPropertyName("masaSalarial")]
    public decimal MasaSalarial { get; set; }
    [JsonPropertyName("costoTotalEmpresa")]
    public decimal CostoTotalEmpresa { get; set; } // Incluye contribuciones patronales
    
    // Distribución salarial
    [JsonPropertyName("salarioMinimo")]
    public decimal SalarioMinimo { get; set; }
    [JsonPropertyName("salarioMaximo")]
    public decimal SalarioMaximo { get; set; }
    [JsonPropertyName("salarioPromedio")]
    public decimal SalarioPromedio { get; set; }
    [JsonPropertyName("salarioMediano")]
    public decimal SalarioMediano { get; set; }
    
    // Deducciones
    [JsonPropertyName("totalDeduccionesLegales")]
    public decimal TotalDeduccionesLegales { get; set; }
    [JsonPropertyName("totalDeduccionesVoluntarias")]
    public decimal TotalDeduccionesVoluntarias { get; set; }
    
    // Tendencias (comparación con período anterior)
    [JsonPropertyName("variacionPorcentualNomina")]
    public decimal VariacionPorcentualNomina { get; set; }
    [JsonPropertyName("variacionCantidadEmpleados")]
    public int VariacionCantidadEmpleados { get; set; }
}
