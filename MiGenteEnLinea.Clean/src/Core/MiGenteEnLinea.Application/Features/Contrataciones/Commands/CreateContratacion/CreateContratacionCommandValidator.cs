using FluentValidation;

namespace MiGenteEnLinea.Application.Features.Contrataciones.Commands.CreateContratacion;

/// <summary>
/// Validador para CreateContratacionCommand.
/// 
/// REGLAS DE VALIDACIÓN:
/// 1. DescripcionCorta: Requerida, máx 60 caracteres
/// 2. DescripcionAmpliada: Opcional, máx 250 caracteres si se proporciona
/// 3. FechaInicio: Requerida
/// 4. FechaFinal: Requerida, debe ser >= FechaInicio
/// 5. MontoAcordado: Requerido, debe ser > 0 y <= 1,000,000 (límite razonable)
/// 6. EsquemaPagos: Opcional, máx 50 caracteres si se proporciona
/// 7. Notas: Opcional, máx 500 caracteres si se proporcionan
/// </summary>
public class CreateContratacionCommandValidator : AbstractValidator<CreateContratacionCommand>
{
    public CreateContratacionCommandValidator()
    {
        RuleFor(x => x.EmpleadorUserId)
            .NotEmpty()
            .WithMessage("No se pudo identificar el usuario autenticado");

        RuleFor(x => x.ContratacionId)
            .NotNull()
            .WithMessage("Debe seleccionar un contratista válido antes de crear la contratación");

        RuleFor(x => x.ContratistaId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un contratista");

        RuleFor(x => x.ServicioId)
            .GreaterThan(0)
            .When(x => x.ServicioId.HasValue)
            .WithMessage("El servicio seleccionado no es válido");

        RuleFor(x => x.DescripcionCorta)
            .NotEmpty()
            .WithMessage("La descripción corta es requerida")
            .MaximumLength(60)
            .WithMessage("La descripción corta no puede exceder 60 caracteres");

        RuleFor(x => x.DescripcionAmpliada)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.DescripcionAmpliada))
            .WithMessage("La descripción ampliada no puede exceder 250 caracteres");

        RuleFor(x => x.FechaInicio)
            .NotEmpty()
            .WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFinal)
            .NotEmpty()
            .WithMessage("La fecha final es requerida")
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha final debe ser igual o posterior a la fecha de inicio");

        RuleFor(x => x.MontoAcordado)
            .GreaterThan(0)
            .WithMessage("El monto acordado debe ser mayor a 0")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("El monto acordado no puede exceder RD$ 1,000,000");

        RuleFor(x => x.EsquemaPagos)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.EsquemaPagos))
            .WithMessage("El esquema de pagos no puede exceder 50 caracteres");

        RuleFor(x => x.Notas)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notas))
            .WithMessage("Las notas no pueden exceder 500 caracteres");

        // Validación de lógica de negocio: duración razonable
        RuleFor(x => x)
            .Must(HavingReasonableDuration)
            .WithMessage("La duración de la contratación debe estar entre 1 día y 2 años");
    }

    private bool HavingReasonableDuration(CreateContratacionCommand command)
    {
        var duracionDias = command.FechaFinal.DayNumber - command.FechaInicio.DayNumber;
        return duracionDias >= 0 && duracionDias <= 730; // Máximo 2 años
    }
}
