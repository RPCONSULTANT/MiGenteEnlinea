using Microsoft.Extensions.Options;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Infrastructure.Options;

namespace MiGenteEnLinea.Infrastructure.Services;

public sealed class AuthLinksProvider : IAuthLinksProvider
{
    private readonly AuthLinksOptions _options;

    public AuthLinksProvider(IOptions<AuthLinksOptions> options)
    {
        _options = options.Value;
    }

    public string BuildActivationUrl(string userId, string email)
    {
        var baseUrl = NormalizeBaseUrl(_options.PublicWebBaseUrl);
        return $"{baseUrl}/Auth/Activar?userId={Uri.EscapeDataString(userId)}&email={Uri.EscapeDataString(email)}";
    }

    public string BuildResetPasswordUrl(string email, string token)
    {
        var baseUrl = NormalizeBaseUrl(_options.PublicWebBaseUrl);
        return $"{baseUrl}/Auth/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }

    private static string NormalizeBaseUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "https://plattaformv2.migenteenlinea.do";
        }

        return value.TrimEnd('/');
    }
}
