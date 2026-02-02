using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Empleados.Commands.ProcesarPago;

public class ProcesarPagoCommandValidator : AbstractValidator<ProcesarPagoCommand>
{
    public ProcesarPagoCommandValidator()
    {
        // UserId es inyectado por el controlador desde JWT, no validar aquí
        // Solo validar que no exceda el límite si está presente
        RuleFor(x => x.UserId)
            .MaximumLength(450).WithMessage("El ID del usuario no puede exceder 450 caracteres")
            .When(x => !string.IsNullOrEmpty(x.UserId));

        RuleFor(x => x.EmpleadoId)
            .GreaterThan(0).WithMessage("El ID del empleado debe ser mayor a 0");

        RuleFor(x => x.FechaPago)
            .LessThanOrEqualTo(DateTime.Now.AddDays(7))
            .WithMessage("La fecha de pago no puede ser mayor a 7 días en el futuro")
            .When(x => x.FechaPago != default);

        RuleFor(x => x.TipoConcepto)
            .NotEmpty().WithMessage("El tipo de concepto es requerido")
            .Must(x => x == "Salario" || x == "Regalia")
            .WithMessage("El tipo de concepto debe ser 'Salario' o 'Regalia'");

        RuleFor(x => x.Comentarios)
            .MaximumLength(500).WithMessage("Los comentarios no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Comentarios));
    }
}
