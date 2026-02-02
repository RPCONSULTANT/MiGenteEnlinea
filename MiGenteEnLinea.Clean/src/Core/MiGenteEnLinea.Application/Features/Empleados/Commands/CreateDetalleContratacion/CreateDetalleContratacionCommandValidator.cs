using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.CreateDetalleContratacion;

public class CreateDetalleContratacionCommandValidator : AbstractValidator<CreateDetalleContratacionCommand>
{
    public CreateDetalleContratacionCommandValidator()
    {
        RuleFor(x => x.ContratacionId)
            .GreaterThan(0)
            .WithMessage("El ID de contratación debe ser mayor que 0");

        RuleFor(x => x.DescripcionCorta)
            .NotEmpty()
            .WithMessage("La descripción corta es requerida")
            .MaximumLength(60);

        RuleFor(x => x.FechaInicio)
            .NotNull()
            .WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFinal)
            .NotNull()
            .WithMessage("La fecha final es requerida")
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha final debe ser posterior a la fecha de inicio");

        RuleFor(x => x.MontoAcordado)
            .GreaterThan(0)
            .When(x => x.MontoAcordado.HasValue)
            .WithMessage("El monto acordado debe ser mayor que 0");
    }
}
