using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;

public class CreateEmpleadoTemporalCommandValidator : AbstractValidator<CreateEmpleadoTemporalCommand>
{
    public CreateEmpleadoTemporalCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es requerido");

        RuleFor(x => x)
            .Custom((command, context) =>
            {
                var tipo = command.Tipo ?? 1;
                var esJuridica = tipo == 2;

                if (esJuridica)
                {
                    if (string.IsNullOrWhiteSpace(command.NombreComercial))
                    {
                        context.AddFailure(nameof(command.NombreComercial), "El nombre comercial es requerido para contratista jurídico");
                    }
                    else if (command.NombreComercial.Length > 100)
                    {
                        context.AddFailure(nameof(command.NombreComercial), "El nombre comercial no puede exceder 100 caracteres");
                    }

                    if (string.IsNullOrWhiteSpace(command.Rnc))
                    {
                        context.AddFailure(nameof(command.Rnc), "El RNC es requerido para contratista jurídico");
                    }
                    else if (command.Rnc.Length > 20)
                    {
                        context.AddFailure(nameof(command.Rnc), "El RNC no puede exceder 20 caracteres");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(command.Nombre))
                    {
                        context.AddFailure(nameof(command.Nombre), "El nombre es requerido");
                    }
                    else if (command.Nombre.Length > 100)
                    {
                        context.AddFailure(nameof(command.Nombre), "El nombre no puede exceder 100 caracteres");
                    }

                    if (string.IsNullOrWhiteSpace(command.Apellido))
                    {
                        context.AddFailure(nameof(command.Apellido), "El apellido es requerido");
                    }
                    else if (command.Apellido.Length > 100)
                    {
                        context.AddFailure(nameof(command.Apellido), "El apellido no puede exceder 100 caracteres");
                    }

                    if (string.IsNullOrWhiteSpace(command.Identificacion))
                    {
                        context.AddFailure(nameof(command.Identificacion), "La identificación es requerida");
                    }
                    else if (command.Identificacion.Length > 20)
                    {
                        context.AddFailure(nameof(command.Identificacion), "La identificación no puede exceder 20 caracteres");
                    }
                }
            });

        RuleFor(x => x.Servicio)
            .NotEmpty()
            .WithMessage("El servicio es requerido");

        RuleFor(x => x.FechaInicio)
            .NotNull()
            .WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFinal)
            .NotNull()
            .WithMessage("La fecha final es requerida")
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha final debe ser posterior a la fecha de inicio");

        RuleFor(x => x.Pago)
            .GreaterThan(0)
            .When(x => x.Pago.HasValue)
            .WithMessage("El pago debe ser mayor que 0");
    }
}
