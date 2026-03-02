using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Contratistas;
using MiGenteEnLinea.Domain.Entities.Pagos;
using MiGenteEnLinea.Domain.Entities.Suscripciones;

namespace MiGenteEnLinea.Application.Features.Suscripciones.Commands.ProcesarVentaSimple;

/// <summary>
/// Procesa venta/suscripción en modo simple sin pasarela de pago.
/// </summary>
public class ProcesarVentaSimpleCommandHandler : IRequestHandler<ProcesarVentaSimpleCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ProcesarVentaSimpleCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public ProcesarVentaSimpleCommandHandler(
        IApplicationDbContext context,
        ILogger<ProcesarVentaSimpleCommandHandler> logger,
        IIdentityService identityService)
    {
        _context = context;
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<int> Handle(ProcesarVentaSimpleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "payment.simple.requested UserId={UserId}, PlanId={PlanId}, Motivo={Motivo}",
            request.UserId,
            request.PlanId,
            request.Motivo ?? "N/A");

        var planEmpleador = await _context.PlanesEmpleadores
            .Where(p => p.PlanId == request.PlanId && p.Activo)
            .FirstOrDefaultAsync(cancellationToken);

        var planContratista = planEmpleador == null
            ? await _context.PlanesContratistas
                .Where(p => p.PlanId == request.PlanId && p.Activo)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        if (planEmpleador == null && planContratista == null)
        {
            throw new NotFoundException($"Plan con ID {request.PlanId} no encontrado o inactivo");
        }

        var credencial = await _context.Credenciales
            .Where(c => c.UserId == request.UserId)
            .Select(c => new { c.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (credencial == null)
        {
            throw new NotFoundException($"No existe credencial para el usuario {request.UserId}");
        }

        var precio = planEmpleador?.Precio ?? planContratista!.Precio;
        const int duracionMeses = 1;

        var venta = Venta.Create(
            userId: request.UserId,
            planId: request.PlanId,
            precio: precio,
            metodoPago: 4,
            idempotencyKey: $"SIMPLE-{Guid.NewGuid():N}",
            direccionIp: null);

        venta.Aprobar(
            idTransaccion: $"SIMPLE-{DateTime.UtcNow:yyyyMMddHHmmss}",
            ultimosDigitosTarjeta: null,
            comentario: request.Motivo ?? "Checkout simple/fake");

        _context.Ventas.Add(venta);

        var suscripcionExistente = await _context.Suscripciones
            .Where(s => s.UserId == request.UserId && !s.Cancelada)
            .FirstOrDefaultAsync(cancellationToken);

        DateTime fechaVencimiento;
        if (suscripcionExistente != null)
        {
            suscripcionExistente.Renovar(duracionMeses);
            fechaVencimiento = suscripcionExistente.Vencimiento.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            var nuevaSuscripcion = Suscripcion.Create(
                userId: request.UserId,
                planId: request.PlanId,
                duracionMeses: duracionMeses);

            _context.Suscripciones.Add(nuevaSuscripcion);
            fechaVencimiento = nuevaSuscripcion.Vencimiento.ToDateTime(TimeOnly.MinValue);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var planUpdated = await _identityService.UpdateUserPlanAsync(
            request.UserId,
            request.PlanId,
            fechaVencimiento);

        if (planContratista != null)
        {
            await EnsureContratistaProfileExistsAsync(request.UserId, cancellationToken);
        }

        if (!planUpdated)
        {
            _logger.LogWarning(
                "payment.simple.processed.without_identity_plan_update UserId={UserId}, PlanId={PlanId}, VentaId={VentaId}",
                request.UserId,
                request.PlanId,
                venta.VentaId);
        }
        else
        {
            _logger.LogInformation(
                "payment.simple.processed UserId={UserId}, PlanId={PlanId}, VentaId={VentaId}",
                request.UserId,
                request.PlanId,
                venta.VentaId);
        }

        return venta.VentaId;
    }

    private async Task EnsureContratistaProfileExistsAsync(string userId, CancellationToken cancellationToken)
    {
        var exists = await _context.Contratistas
            .AnyAsync(c => c.UserId == userId, cancellationToken);

        if (exists)
        {
            return;
        }

        var perfil = await _context.Perfiles
            .Where(p => p.UserId == userId)
            .Select(p => new { p.Nombre, p.Apellido, p.Email, p.Telefono1 })
            .FirstOrDefaultAsync(cancellationToken);

        var nombre = string.IsNullOrWhiteSpace(perfil?.Nombre) ? "Usuario" : perfil.Nombre.Trim();
        var apellido = string.IsNullOrWhiteSpace(perfil?.Apellido) ? "Contratista" : perfil.Apellido.Trim();

        var bootstrap = Contratista.Create(
            userId: userId,
            nombre: nombre,
            apellido: apellido,
            tipo: 1,
            titulo: "Perfil en inicialización",
            presentacion: "Perfil creado automáticamente tras compra de plan",
            telefono1: perfil?.Telefono1,
            provincia: null,
            nivelNacional: false);

        if (!string.IsNullOrWhiteSpace(perfil?.Email))
        {
            bootstrap.ActualizarContacto(email: MiGenteEnLinea.Domain.ValueObjects.Email.Create(perfil.Email));
        }

        bootstrap.Desactivar();
        _context.Contratistas.Add(bootstrap);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "AutoBootstrap de perfil contratista completado. UserId={UserId}, ContratistaId={ContratistaId}",
            userId,
            bootstrap.Id);
    }
}
