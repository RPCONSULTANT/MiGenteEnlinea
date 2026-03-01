using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Validador para RegisterCommand
/// </summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido")
            .EmailAddress().WithMessage("El correo electrónico no es válido")
            .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres");

        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$")
            .WithMessage("La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&#)")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El tipo de usuario es requerido")
            .Must(x => x == 1 || x == 2)
            .WithMessage("El tipo de usuario debe ser 1 (Empleador) o 2 (Contratista)");

        RuleFor(x => x.Telefono1)
            .MaximumLength(20).WithMessage("El teléfono 1 no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono1));

        RuleFor(x => x.Telefono2)
            .MaximumLength(20).WithMessage("El teléfono 2 no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono2));

        RuleFor(x => x.Host)
            .NotEmpty().WithMessage("El host es requerido para enviar el correo de activación")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("El host debe ser una URL válida");
    }
}
