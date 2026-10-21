using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class ActualizarVehiculoValidator : AbstractValidator<ActualizarVehiculoDto>
{
    public ActualizarVehiculoValidator()
    {
        RuleFor(x => x.ModeloId).GreaterThan(0).When(x => x.ModeloId.HasValue);
        RuleFor(x => x.Placa).MaximumLength(15);
        RuleFor(x => x.Anio).InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1))
            .When(x => x.Anio.HasValue);
        RuleFor(x => x.KilometrajeActual).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TipoCombustible).MaximumLength(50);
    }
}
