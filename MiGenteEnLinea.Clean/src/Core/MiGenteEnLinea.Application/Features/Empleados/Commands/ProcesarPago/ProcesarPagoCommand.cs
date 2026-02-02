using System;
using MediatR;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.ProcesarPago;

/// <summary>
/// Command para procesar un pago de nómina de un empleado.
/// Genera un recibo con percepciones (salario, extras) y deducciones (TSS).
/// 
/// Mapea: EmpleadosService.procesarPago() + fichaEmpleado.aspx.cs armarNovedad()
/// Legacy: 2 operaciones separadas (header primero, luego detalles)
/// </summary>
public record ProcesarPagoCommand : IRequest<int>
{
    /// <summary>
    /// GUID del empleador que procesa el pago (inyectado por el controlador desde JWT)
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// ID del empleado que recibe el pago
    /// </summary>
    public int EmpleadoId { get; init; }

    /// <summary>
    /// Fecha del pago (si no se especifica, usa fecha actual)
    /// </summary>
    public DateTime FechaPago { get; init; } = DateTime.Now;

    /// <summary>
    /// Tipo de concepto: "Salario" o "Regalia"
    /// </summary>
    public string TipoConcepto { get; init; } = "Salario";

    /// <summary>
    /// True si es fracción de período (días trabajados), False si es período completo
    /// </summary>
    public bool EsFraccion { get; init; }

    /// <summary>
    /// True si se deben aplicar deducciones TSS
    /// </summary>
    public bool AplicarTss { get; init; } = true;

    /// <summary>
    /// Comentarios opcionales del pago
    /// </summary>
    public string? Comentarios { get; init; }
}
