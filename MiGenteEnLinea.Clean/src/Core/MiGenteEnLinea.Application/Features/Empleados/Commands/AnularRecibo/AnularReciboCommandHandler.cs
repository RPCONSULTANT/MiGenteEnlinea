using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Nominas;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.AnularRecibo;

/// <summary>
/// Handler para anular un recibo de pago (soft delete).
/// ⚠️ MEJORA: Legacy usa hard delete, Clean usa soft delete con Estado=3 (Anulado).
/// </summary>
public class AnularReciboCommandHandler : IRequestHandler<AnularReciboCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AnularReciboCommandHandler> _logger;

    public AnularReciboCommandHandler(
        IApplicationDbContext context,
        ILogger<AnularReciboCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(AnularReciboCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Anulando recibo: PagoId={PagoId}, UserId={UserId}",
            request.PagoId, request.UserId);

        // PASO 1: Buscar recibo y validar propiedad
        var recibo = await _context.RecibosHeader
            .FirstOrDefaultAsync(r => r.PagoId == request.PagoId && 
                                     r.UserId == request.UserId, 
                                     cancellationToken)
            ?? throw new NotFoundException(nameof(ReciboHeader), request.PagoId);

        // PASO 2: Validar que el recibo no esté ya anulado
        if (recibo.Estado == 3) // 3 = Anulado
        {
            throw new ValidationException(
                $"El recibo ya está anulado (PagoId={request.PagoId})");
        }

        // PASO 3: Anular usando método de dominio (soft delete)
        // ⚠️ MEJORA vs Legacy:
        // Legacy: db.Empleador_Recibos_Header.Remove(header) - hard delete
        // Clean: recibo.Anular() - soft delete con Estado=3
        var motivo = string.IsNullOrWhiteSpace(request.MotivoAnulacion)
            ? "Anulado por el sistema"
            : request.MotivoAnulacion;
        recibo.Anular(motivo);

        // PASO 4: Guardar cambios
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Recibo anulado exitosamente: PagoId={PagoId}, Motivo={Motivo}",
            request.PagoId, request.MotivoAnulacion ?? "No especificado");

        return Unit.Value;
    }
}
