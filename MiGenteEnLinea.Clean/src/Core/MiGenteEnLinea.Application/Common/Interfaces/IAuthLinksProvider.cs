namespace MiGenteEnLinea.Application.Common.Interfaces;

public interface IAuthLinksProvider
{
    string BuildActivationUrl(string userId, string email);
    string BuildResetPasswordUrl(string email, string token);
}
