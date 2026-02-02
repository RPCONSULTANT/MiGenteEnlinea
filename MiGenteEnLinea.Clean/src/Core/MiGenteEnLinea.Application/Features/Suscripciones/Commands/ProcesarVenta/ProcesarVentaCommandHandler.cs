using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Exceptions;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Pagos;
using MiGenteEnLinea.Domain.Entities.Suscripciones;

namespace MiGenteEnLinea.Application.Features.Suscripciones.Commands.ProcesarVenta;

/// <summary>
/// Handler para ProcesarVentaCommand - Procesa pago con Cardnet y crea suscripción.
/// </summary>
/// <remarks>
/// LÓGICA LEGACY REPLICADA:
/// 1. PaymentService.consultarIdempotency() → genera idempotency key
/// 2. PaymentService.Payment() → llama a Cardnet API sales endpoint
/// 3. Si ResponseCode == "00": Crear Venta + Suscripción + Actualizar AspNetUsers.PlanID/VencimientoPlan
/// 4. Si ResponseCode != "00": Crear Venta (rechazada) + throw exception
/// 
/// CARDNET API:
/// - Test URL: https://ecommerce.cardnet.com.do/api/payment/transactions/sales
/// - Response codes: "00" = Aprobado, otros = Rechazado
/// - Requiere idempotency key para evitar doble cobro
/// </remarks>
public class ProcesarVentaCommandHandler : IRequestHandler<ProcesarVentaCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ProcesarVentaCommandHandler> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IIdentityService _identityService;

    public ProcesarVentaCommandHandler(
        IApplicationDbContext context,
        ILogger<ProcesarVentaCommandHandler> logger,
        IPaymentService paymentService,
        IIdentityService identityService)
    {
        _context = context;
        _logger = logger;
        _paymentService = paymentService;
        _identityService = identityService;
    }

    public async Task<int> Handle(ProcesarVentaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Procesando venta con pago para usuario {UserId}, plan {PlanId}",
            request.UserId,
            request.PlanId);

        // PASO 1: Validar plan existe y obtener precio
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

        var precio = planEmpleador?.Precio ?? planContratista!.Precio;
        var duracionMeses = 1; // Hardcoded a 1 mes (como en Legacy)

        _logger.LogInformation(
            "Plan encontrado: {PlanId}, Precio: {Precio:C}",
            request.PlanId,
            precio);

        // PASO 2: Generar idempotency key
        var idempotencyKey = await _paymentService.GenerateIdempotencyKeyAsync(cancellationToken);

        _logger.LogInformation(
            "Idempotency key generada: {IdempotencyKey}",
            idempotencyKey);

        // PASO 3: Procesar pago con Cardnet
        var paymentRequest = new PaymentRequest
        {
            Amount = precio,
            CardNumber = request.CardNumber,
            CVV = request.Cvv,
            ExpirationDate = request.ExpirationDate,
            ClientIP = request.ClientIp ?? "0.0.0.0",
            ReferenceNumber = request.ReferenceNumber ?? Guid.NewGuid().ToString(),
            InvoiceNumber = request.InvoiceNumber ?? $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };

        _logger.LogInformation(
            "Enviando solicitud de pago a Cardnet. Monto: {Monto:C}, Referencia: {Referencia}",
            precio,
            paymentRequest.ReferenceNumber);

        PaymentResult paymentResponse;
        try
        {
            paymentResponse = await _paymentService.ProcessPaymentAsync(paymentRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error al procesar pago con Cardnet para usuario {UserId}",
                request.UserId);

            // Crear registro de venta rechazada (estado = 3: Error)
            var ventaError = Venta.Create(
                userId: request.UserId,
                planId: request.PlanId,
                precio: precio,
                metodoPago: 1, // Tarjeta de crédito
                idempotencyKey: idempotencyKey,
                direccionIp: request.ClientIp);

            ventaError.Rechazar($"Error de comunicación con Cardnet: {ex.Message}");
            _context.Ventas.Add(ventaError);
            await _context.SaveChangesAsync(cancellationToken);

            throw new PaymentException("Error al procesar el pago. Intente nuevamente.", ex);
        }

        // PASO 4: Validar respuesta de Cardnet
        var esAprobado = paymentResponse.ResponseCode == "00";

        _logger.LogInformation(
            "Respuesta Cardnet - Código: {ResponseCode}, Descripción: {ResponseDesc}, Aprobado: {Aprobado}",
            paymentResponse.ResponseCode,
            paymentResponse.ResponseDescription,
            esAprobado);

        // PASO 5: Crear registro de venta
        var venta = Venta.Create(
            userId: request.UserId,
            planId: request.PlanId,
            precio: precio,
            metodoPago: 1, // 1 = Tarjeta de crédito
            idempotencyKey: idempotencyKey,
            direccionIp: request.ClientIp);

        if (esAprobado)
        {
            // Pago aprobado
            var ultimosDigitos = request.CardNumber.Length >= 4
                ? request.CardNumber.Substring(request.CardNumber.Length - 4)
                : request.CardNumber;

            venta.Aprobar(
                idTransaccion: paymentResponse.TransactionReference ?? paymentResponse.ApprovalCode ?? "N/A",
                ultimosDigitosTarjeta: ultimosDigitos,
                comentario: $"Aprobado - {paymentResponse.ResponseDescription}");

            _logger.LogInformation(
                "Pago aprobado. ID Transacción: {IdTransaccion}",
                paymentResponse.TransactionReference);
        }
        else
        {
            // Pago rechazado
            venta.Rechazar($"{paymentResponse.ResponseDescription} (Código: {paymentResponse.ResponseCode})");

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(
                "Pago rechazado para usuario {UserId}. Código: {ResponseCode}, Razón: {ResponseDesc}",
                request.UserId,
                paymentResponse.ResponseCode,
                paymentResponse.ResponseDescription);

            throw new PaymentRejectedException(
                $"Pago rechazado: {paymentResponse.ResponseDescription}",
                paymentResponse.ResponseCode);
        }

        _context.Ventas.Add(venta);

        // PASO 6: Crear o renovar suscripción (solo si pago aprobado)
        var suscripcionExistente = await _context.Suscripciones
            .Where(s => s.UserId == request.UserId && !s.Cancelada)
            .FirstOrDefaultAsync(cancellationToken);

        DateTime fechaVencimiento;

        if (suscripcionExistente != null)
        {
            // Renovar existente
            _logger.LogInformation(
                "Renovando suscripción existente {SuscripcionId} por {Meses} meses",
                suscripcionExistente.Id,
                duracionMeses);

            suscripcionExistente.Renovar(duracionMeses);
            fechaVencimiento = suscripcionExistente.Vencimiento.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            // Crear nueva
            _logger.LogInformation(
                "Creando nueva suscripción para usuario {UserId}, plan {PlanId}",
                request.UserId,
                request.PlanId);

            var nuevaSuscripcion = Suscripcion.Create(
                userId: request.UserId,
                planId: request.PlanId,
                duracionMeses: duracionMeses);

            _context.Suscripciones.Add(nuevaSuscripcion);
            fechaVencimiento = nuevaSuscripcion.Vencimiento.ToDateTime(TimeOnly.MinValue);
        }

        // PASO 7: Guardar cambios en base de datos
        await _context.SaveChangesAsync(cancellationToken);

        // PASO 8: Actualizar AspNetUsers.PlanID y VencimientoPlan
        // Esto asegura que el próximo login devuelva el plan correcto
        var planUpdated = await _identityService.UpdateUserPlanAsync(
            request.UserId,
            request.PlanId,
            fechaVencimiento);

        if (!planUpdated)
        {
            _logger.LogWarning(
                "No se pudo actualizar PlanID/VencimientoPlan en AspNetUsers para usuario {UserId}. " +
                "El usuario deberá re-autenticar para ver el plan actualizado.",
                request.UserId);
        }

        _logger.LogInformation(
            "Venta procesada exitosamente. VentaId: {VentaId}, Usuario: {UserId}, Monto: {Monto:C}",
            venta.VentaId,
            request.UserId,
            precio);

        return venta.VentaId;
    }
}

/// <summary>
/// Excepción lanzada cuando un pago es rechazado por Cardnet.
/// </summary>
public class PaymentRejectedException : Exception
{
    public string ResponseCode { get; }

    public PaymentRejectedException(string message, string responseCode)
        : base(message)
    {
        ResponseCode = responseCode;
    }
}

/// <summary>
/// Excepción genérica para errores de pago.
/// </summary>
public class PaymentException : Exception
{
    public PaymentException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
