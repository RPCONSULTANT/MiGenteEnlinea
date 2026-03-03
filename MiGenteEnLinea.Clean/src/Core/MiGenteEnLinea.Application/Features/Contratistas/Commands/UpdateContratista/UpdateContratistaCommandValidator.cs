using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratista;

/// <summary>
/// Validator: Validaciones de input para UpdateContratistaCommand
/// </summary>
public class UpdateContratistaCommandValidator : AbstractValidator<UpdateContratistaCommand>
{
    public UpdateContratistaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId es requerido")
            .Must(BeValidGuid).WithMessage("UserId debe ser un GUID válido");

        RuleFor(x => x.Titulo)
            .MaximumLength(70).WithMessage("Titulo no puede exceder 70 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Titulo));

        RuleFor(x => x.Tipo)
            .InclusiveBetween(1, 2).WithMessage("Tipo debe ser 1 (Persona Física) o 2 (Empresa)")
            .When(x => x.Tipo.HasValue);

        RuleFor(x => x.Identificacion)
            .MaximumLength(20).WithMessage("Identificacion no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Identificacion));

        RuleFor(x => x.Nombre)
            .MaximumLength(20).WithMessage("Nombre no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Nombre));

        RuleFor(x => x.Apellido)
            .MaximumLength(50).WithMessage("Apellido no puede exceder 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Apellido));

        RuleFor(x => x.NombreComercial)
            .MaximumLength(20).WithMessage("NombreComercial no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.NombreComercial));

        RuleFor(x => x.Sector)
            .MaximumLength(40).WithMessage("Sector no puede exceder 40 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Sector));

        RuleFor(x => x.Experiencia)
            .GreaterThanOrEqualTo(0).WithMessage("Experiencia no puede ser negativa")
            .When(x => x.Experiencia.HasValue);

        RuleFor(x => x.Presentacion)
            .MaximumLength(250).WithMessage("Presentacion no puede exceder 250 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Presentacion));

        RuleFor(x => x.Provincia)
            .MaximumLength(50).WithMessage("Provincia no puede exceder 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Provincia));

        RuleFor(x => x.Telefono1)
            .MaximumLength(16).WithMessage("Telefono1 no puede exceder 16 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono1));

        RuleFor(x => x.Telefono2)
            .MaximumLength(20).WithMessage("Telefono2 no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono2));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email debe ser una dirección válida")
            .MaximumLength(50).WithMessage("Email no puede exceder 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        // Al menos un campo debe ser proporcionado
        RuleFor(x => x)
            .Must(command => 
                command.Titulo != null ||
                command.Tipo.HasValue ||
                command.Identificacion != null ||
                command.Nombre != null ||
                command.Apellido != null ||
                command.NombreComercial != null ||
                command.Sector != null ||
                command.Experiencia.HasValue ||
                command.Presentacion != null ||
                command.Provincia != null ||
                command.NivelNacional.HasValue ||
                command.Telefono1 != null ||
                command.Whatsapp1.HasValue ||
                command.Telefono2 != null ||
                command.Whatsapp2.HasValue ||
                command.Email != null)
            .WithMessage("Debe proporcionar al menos un campo para actualizar");
    }

    private bool BeValidGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }
}
