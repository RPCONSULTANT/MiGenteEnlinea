using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace MiGenteEnLinea.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de envío de emails usando MailKit (SMTP)
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _settings.Validate(); // Validar config al inicializar
        _logger = logger;
    }

    /// <summary>
    /// Enviar email de activación de cuenta
    /// </summary>
    public async Task SendActivationEmailAsync(string toEmail, string toName, string activationUrl)
    {
        _logger.LogInformation(
            "Enviando email de activación a: {Email}",
            toEmail);

        var subject = "¡Activa tu cuenta de MiGente En Línea!";
        var htmlBody = GetActivationEmailTemplate(toName, activationUrl);
        var plainText = $"Hola {toName},\n\nGracias por registrarte. Activa tu cuenta visitando: {activationUrl}\n\nSaludos,\nMiGente En Línea";

        await SendEmailAsync(toEmail, toName, subject, htmlBody, plainText);
    }

    /// <summary>
    /// Enviar email de bienvenida después de activar cuenta
    /// </summary>
    public async Task SendWelcomeEmailAsync(string toEmail, string toName, string userType)
    {
        _logger.LogInformation(
            "Enviando email de bienvenida a: {Email}, Tipo: {UserType}",
            toEmail,
            userType);

        var subject = $"¡Bienvenido a MiGente En Línea!";
        var htmlBody = GetWelcomeEmailTemplate(toName, userType);
        var plainText = $"Hola {toName},\n\n¡Tu cuenta ha sido activada exitosamente!\n\nSaludos,\nMiGente En Línea";

        await SendEmailAsync(toEmail, toName, subject, htmlBody, plainText);
    }

    /// <summary>
    /// Enviar email de recuperación de contraseña
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetUrl)
    {
        _logger.LogInformation(
            "Enviando email de recuperación de contraseña a: {Email}",
            toEmail);

        var subject = "Recuperación de Contraseña - MiGente En Línea";
        var htmlBody = GetPasswordResetEmailTemplate(toName, resetUrl);
        var plainText = $"Hola {toName},\n\nRecibimos una solicitud para restablecer tu contraseña.\n\nVisita: {resetUrl}\n\nSi no solicitaste esto, ignora este email.\n\nSaludos,\nMiGente En Línea";

        await SendEmailAsync(toEmail, toName, subject, htmlBody, plainText);
    }

    /// <summary>
    /// Enviar confirmación de pago de suscripción
    /// </summary>
    public async Task SendPaymentConfirmationEmailAsync(
        string toEmail,
        string toName,
        string planName,
        decimal amount,
        string transactionId)
    {
        _logger.LogInformation(
            "Enviando confirmación de pago a: {Email}, Plan: {Plan}, Monto: {Amount}",
            toEmail,
            planName,
            amount);

        var subject = "Confirmación de Pago - MiGente En Línea";
        var htmlBody = GetPaymentConfirmationEmailTemplate(toName, planName, amount, transactionId);
        var plainText = $"Hola {toName},\n\nTu pago ha sido procesado exitosamente.\n\nPlan: {planName}\nMonto: RD${amount:N2}\nTransacción: {transactionId}\n\nSaludos,\nMiGente En Línea";

        await SendEmailAsync(toEmail, toName, subject, htmlBody, plainText);
    }

    /// <summary>
    /// Enviar notificación de contratación (nueva, aceptada, rechazada, etc.)
    /// </summary>
    public async Task SendContractNotificationEmailAsync(
        string toEmail,
        string toName,
        string contractTitle,
        string status,
        string message)
    {
        _logger.LogInformation(
            "Enviando notificación de contratación a: {Email}, Estado: {Status}",
            toEmail,
            status);

        var subject = $"Actualización de Contratación: {status}";
        var htmlBody = GetContractNotificationEmailTemplate(toName, contractTitle, status, message);
        var plainText = $"Hola {toName},\n\nActualización de tu contratación '{contractTitle}'.\nEstado: {status}\n\n{message}\n\nSaludos,\nMiGente En Línea";

        await SendEmailAsync(toEmail, toName, subject, htmlBody, plainText);
    }

    /// <summary>
    /// Enviar email genérico
    /// </summary>
    public async Task SendEmailAsync(
        string toEmail,
        string? toName,
        string subject,
        string htmlBody,
        string? plainTextBody = null)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Email destinatario requerido", nameof(toEmail));

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Asunto requerido", nameof(subject));

        if (string.IsNullOrWhiteSpace(htmlBody))
            throw new ArgumentException("Cuerpo del email requerido", nameof(htmlBody));

        await SendWithRetryAsync(toEmail, toName, subject, htmlBody, plainTextBody);
    }

    /// <summary>
    /// Enviar email con retry policy (exponential backoff)
    /// </summary>
    private async Task SendWithRetryAsync(
        string toEmail,
        string? toName,
        string subject,
        string htmlBody,
        string? plainTextBody)
    {
        int attempt = 0;
        Exception? lastException = null;

        while (attempt < _settings.MaxRetryAttempts)
        {
            attempt++;

            try
            {
                _logger.LogInformation(
                    "Intento {Attempt} de {MaxAttempts} para enviar email a: {Email}",
                    attempt,
                    _settings.MaxRetryAttempts,
                    toEmail);

                // CONSTRUIR MENSAJE
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.ReplyTo.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(new MailboxAddress(toName ?? toEmail, toEmail));
                message.Subject = subject;
                message.MessageId = MimeKit.Utils.MimeUtils.GenerateMessageId("migenteenlinea.com");

                // CUERPO: HTML + Plain Text (fallback)
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = plainTextBody ?? StripHtml(htmlBody)
                };

                message.Body = bodyBuilder.ToMessageBody();

                // ENVIAR VÍA SMTP
                using var smtpClient = new SmtpClient();

                // Configurar timeout más largo para conexiones lentas
                smtpClient.Timeout = 120000; // 2 minutos

                // Mantener validación SSL por defecto. Solo relajarla si la config lo exige explícitamente.
                smtpClient.CheckCertificateRevocation = true;

                // Usar Auto para que MailKit detecte el tipo correcto de SSL
                _logger.LogDebug(
                    "Conectando a SMTP: {Server}:{Port}",
                    _settings.SmtpServer,
                    _settings.SmtpPort);
                
                await smtpClient.ConnectAsync(
                    _settings.SmtpServer,
                    _settings.SmtpPort,
                    SecureSocketOptions.Auto);

                _logger.LogDebug("Autenticando con usuario: {Username}", _settings.Username);

                // Autenticar
                await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);

                _logger.LogDebug("Enviando mensaje...");

                // Enviar
                await smtpClient.SendAsync(message);

                // Desconectar
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email enviado exitosamente a: {Email} en intento {Attempt}. Subject={Subject}, MessageId={MessageId}",
                    toEmail,
                    attempt,
                    subject,
                    message.MessageId);

                return; // ÉXITO
            }
            catch (Exception ex)
            {
                lastException = ex;

                _logger.LogWarning(
                    ex,
                    "Fallo al enviar email en intento {Attempt}/{MaxAttempts} a: {Email}",
                    attempt,
                    _settings.MaxRetryAttempts,
                    toEmail);

                // Si no es el último intento, esperar antes de reintentar (exponential backoff)
                if (attempt < _settings.MaxRetryAttempts)
                {
                    var delay = _settings.RetryDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                    _logger.LogInformation("Esperando {Delay}ms antes de reintentar...", delay);
                    await Task.Delay(delay);
                }
            }
        }

        // Si llegamos aquí, todos los intentos fallaron
        _logger.LogError(
            lastException,
            "Fallo al enviar email después de {MaxAttempts} intentos a: {Email}",
            _settings.MaxRetryAttempts,
            toEmail);

        throw new InvalidOperationException(
            $"No se pudo enviar el email a {toEmail} después de {_settings.MaxRetryAttempts} intentos",
            lastException);
    }

    /// <summary>
    /// Remover tags HTML para plain text fallback
    /// </summary>
    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Reemplazar <br> y </p> con saltos de línea
        html = html.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("</p>", "\n\n");

        // Remover todos los tags HTML
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", string.Empty);

        // Decodificar entidades HTML
        text = System.Net.WebUtility.HtmlDecode(text);

        return text.Trim();
    }

    #region HTML Email Templates

    /// <summary>
    /// Template: Email de activación de cuenta
    /// </summary>
    private static string GetActivationEmailTemplate(string toName, string activationUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Activa tu cuenta</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 28px;"">MiGente En Línea</h1>
        <p style=""color: #f0f0f0; margin: 10px 0 0 0;"">Plataforma de Gestión de Empleados</p>
    </div>
    
    <div style=""background: white; padding: 40px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: #667eea; margin-top: 0;"">¡Hola {toName}!</h2>
        
        <p>Gracias por registrarte en <strong>MiGente En Línea</strong>. Estás a un paso de comenzar a gestionar tu equipo de forma profesional.</p>
        
        <p>Para activar tu cuenta, haz clic en el botón de abajo:</p>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{activationUrl}"" style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                Activar mi Cuenta
            </a>
        </div>
        
        <p style=""color: #666; font-size: 14px;"">Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
        <p style=""color: #667eea; word-break: break-all; font-size: 12px;"">{activationUrl}</p>
        
        <hr style=""border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;"">
        
        <p style=""color: #999; font-size: 12px; text-align: center;"">
            Este enlace expirará en 24 horas por seguridad.<br>
            Si no solicitaste esta cuenta, ignora este email.
        </p>
    </div>
    
    <div style=""text-align: center; padding: 20px; color: #999; font-size: 12px;"">
        <p>&copy; 2025 MiGente En Línea. Todos los derechos reservados.</p>
        <p>República Dominicana</p>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Template: Email de bienvenida
    /// </summary>
    private static string GetWelcomeEmailTemplate(string toName, string userType)
    {
        var userTypeText = userType == "Empleador" ? "empleadores" : "contratistas";
        var dashboardUrl = "https://migenteenlinea.com/dashboard"; // TODO: URL real

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>¡Bienvenido!</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 28px;"">¡Bienvenido a MiGente En Línea!</h1>
    </div>
    
    <div style=""background: white; padding: 40px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: #667eea;"">¡Hola {toName}!</h2>
        
        <p>Tu cuenta ha sido <strong>activada exitosamente</strong>. Ya puedes acceder a todas las funcionalidades de nuestra plataforma para {userTypeText}.</p>
        
        <h3 style=""color: #764ba2; margin-top: 30px;"">¿Qué puedes hacer ahora?</h3>
        
        <ul style=""line-height: 2;"">
            <li>✅ Gestionar tu información de perfil</li>
            <li>✅ Explorar nuestros planes de suscripción</li>
            <li>✅ Comenzar a usar nuestras herramientas</li>
            <li>✅ Acceder a soporte técnico 24/7</li>
        </ul>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{dashboardUrl}"" style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                Ir a mi Dashboard
            </a>
        </div>
        
        <p style=""color: #666; margin-top: 30px;"">Si tienes alguna pregunta, no dudes en contactarnos. Nuestro equipo está aquí para ayudarte.</p>
    </div>
    
    <div style=""text-align: center; padding: 20px; color: #999; font-size: 12px;"">
        <p>&copy; 2025 MiGente En Línea. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Template: Email de recuperación de contraseña
    /// </summary>
    private static string GetPasswordResetEmailTemplate(string toName, string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Recuperación de Contraseña</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 24px;"">Recuperación de Contraseña</h1>
    </div>
    
    <div style=""background: white; padding: 40px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: #f5576c;"">Hola {toName},</h2>
        
        <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta en <strong>MiGente En Línea</strong>.</p>
        
        <p>Si fuiste tú, haz clic en el botón de abajo para crear una nueva contraseña:</p>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{resetUrl}"" style=""background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                Restablecer Contraseña
            </a>
        </div>
        
        <p style=""color: #666; font-size: 14px;"">Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
        <p style=""color: #f5576c; word-break: break-all; font-size: 12px;"">{resetUrl}</p>
        
        <div style=""background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 30px 0;"">
            <p style=""margin: 0; color: #856404; font-weight: bold;"">⚠️ Seguridad Importante</p>
            <p style=""margin: 10px 0 0 0; color: #856404; font-size: 14px;"">
                Si <strong>NO solicitaste</strong> restablecer tu contraseña, ignora este email. 
                Tu contraseña actual seguirá siendo válida.
            </p>
        </div>
        
        <p style=""color: #999; font-size: 12px; text-align: center; margin-top: 30px;"">
            Este enlace expirará en 1 hora por seguridad.
        </p>
    </div>
    
    <div style=""text-align: center; padding: 20px; color: #999; font-size: 12px;"">
        <p>&copy; 2025 MiGente En Línea. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Template: Email de confirmación de pago
    /// </summary>
    private static string GetPaymentConfirmationEmailTemplate(
        string toName,
        string planName,
        decimal amount,
        string transactionId)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirmación de Pago</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 28px;"">✓ Pago Confirmado</h1>
    </div>
    
    <div style=""background: white; padding: 40px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: #11998e;"">¡Gracias {toName}!</h2>
        
        <p>Tu pago ha sido procesado <strong>exitosamente</strong>. Tu suscripción está activa.</p>
        
        <div style=""background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 5px; padding: 20px; margin: 30px 0;"">
            <h3 style=""color: #11998e; margin-top: 0;"">Detalles del Pago</h3>
            
            <table style=""width: 100%; border-collapse: collapse;"">
                <tr style=""border-bottom: 1px solid #dee2e6;"">
                    <td style=""padding: 10px 0; font-weight: bold;"">Plan:</td>
                    <td style=""padding: 10px 0; text-align: right;"">{planName}</td>
                </tr>
                <tr style=""border-bottom: 1px solid #dee2e6;"">
                    <td style=""padding: 10px 0; font-weight: bold;"">Monto:</td>
                    <td style=""padding: 10px 0; text-align: right; color: #11998e; font-size: 18px; font-weight: bold;"">RD${amount:N2}</td>
                </tr>
                <tr style=""border-bottom: 1px solid #dee2e6;"">
                    <td style=""padding: 10px 0; font-weight: bold;"">Transacción:</td>
                    <td style=""padding: 10px 0; text-align: right; font-family: monospace; font-size: 12px;"">{transactionId}</td>
                </tr>
                <tr>
                    <td style=""padding: 10px 0; font-weight: bold;"">Fecha:</td>
                    <td style=""padding: 10px 0; text-align: right;"">{DateTime.Now:dd/MM/yyyy HH:mm}</td>
                </tr>
            </table>
        </div>
        
        <p>Puedes descargar tu factura desde tu dashboard en cualquier momento.</p>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""https://migenteenlinea.com/mis-facturas"" style=""background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                Ver mis Facturas
            </a>
        </div>
        
        <p style=""color: #666; font-size: 14px; margin-top: 30px;"">
            Recibirás un recordatorio antes de que tu suscripción expire.
        </p>
    </div>
    
    <div style=""text-align: center; padding: 20px; color: #999; font-size: 12px;"">
        <p>&copy; 2025 MiGente En Línea. Todos los derechos reservados.</p>
        <p>¿Tienes preguntas? <a href=""mailto:soporte@migenteenlinea.com"" style=""color: #11998e;"">Contáctanos</a></p>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Template: Email de notificación de contratación
    /// </summary>
    private static string GetContractNotificationEmailTemplate(
        string toName,
        string contractTitle,
        string status,
        string message)
    {
        // Determinar color según estado
        var (color, emoji) = status.ToLower() switch
        {
            "pendiente" => ("#ffc107", "⏳"),
            "aceptada" => ("#17a2b8", "✓"),
            "en progreso" => ("#ff9800", "🚀"),
            "completada" => ("#28a745", "✓"),
            "rechazada" => ("#dc3545", "✗"),
            "cancelada" => ("#6c757d", "✗"),
            _ => ("#007bff", "ℹ️")
        };

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Notificación de Contratación</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: {color}; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 28px;"">{emoji} Actualización de Contratación</h1>
    </div>
    
    <div style=""background: white; padding: 40px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: {color};"">Hola {toName},</h2>
        
        <p>Hay una actualización en tu contratación:</p>
        
        <div style=""background: #f8f9fa; border-left: 4px solid {color}; padding: 20px; margin: 20px 0;"">
            <h3 style=""margin-top: 0; color: #333;"">{contractTitle}</h3>
            <p style=""margin: 10px 0;"">
                <strong style=""color: {color};"">Estado: {status}</strong>
            </p>
            <p style=""margin: 10px 0; color: #666;"">{message}</p>
        </div>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""https://migenteenlinea.com/contrataciones"" style=""background: {color}; color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                Ver Detalles
            </a>
        </div>
        
        <p style=""color: #666; font-size: 14px;"">Puedes revisar todos los detalles ingresando a tu cuenta.</p>
    </div>
    
    <div style=""text-align: center; padding: 20px; color: #999; font-size: 12px;"">
        <p>&copy; 2025 MiGente En Línea. Todos los derechos reservados.</p>
    </div>
</body>
</html>";
    }

    #endregion
}
