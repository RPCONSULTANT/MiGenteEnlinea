using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Suscripciones.Commands.ProcesarVentaSimple;

public class ProcesarVentaSimpleCommandValidator : AbstractValidator<ProcesarVentaSimpleCommand>
{
    public ProcesarVentaSimpleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId es requerido");

        RuleFor(x => x.PlanId)
            .GreaterThan(0).WithMessage("PlanId debe ser mayor a 0");

        RuleFor(x => x.Motivo)
            .MaximumLength(250).WithMessage("Motivo no puede exceder 250 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Motivo));
    }
}
