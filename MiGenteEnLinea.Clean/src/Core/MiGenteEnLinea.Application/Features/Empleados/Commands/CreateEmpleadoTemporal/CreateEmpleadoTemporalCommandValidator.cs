using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateEmpleadoTemporal;

public class CreateEmpleadoTemporalCommandValidator : AbstractValidator<CreateEmpleadoTemporalCommand>
{
    public CreateEmpleadoTemporalCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(100);

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("El apellido es requerido")
            .MaximumLength(100);

        RuleFor(x => x.Identificacion)
            .NotEmpty()
            .WithMessage("La identificación es requerida")
            .MaximumLength(20);

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
