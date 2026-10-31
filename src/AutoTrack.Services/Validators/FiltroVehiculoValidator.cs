using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class FiltroVehiculoValidator : AbstractValidator<FiltroVehiculoDto>
{
    public FiltroVehiculoValidator()
    {
        RuleFor(x => x.Marca).MaximumLength(100);
        RuleFor(x => x.Anio).InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1))
            .When(x => x.Anio.HasValue);
        RuleFor(x => x.KilometrajeMinimo).GreaterThanOrEqualTo(0)
            .When(x => x.KilometrajeMinimo.HasValue);
        RuleFor(x => x.KilometrajeMaximo).GreaterThanOrEqualTo(0)
            .When(x => x.KilometrajeMaximo.HasValue);
        RuleFor(x => x.KilometrajeMaximo)
            .Must((filtro, maximo) => !maximo.HasValue || !filtro.KilometrajeMinimo.HasValue ||
                maximo.Value >= filtro.KilometrajeMinimo.Value)
            .WithMessage("El kilometraje máximo no puede ser menor al mínimo");
        RuleFor(x => x.TipoCombustible).MaximumLength(50);
        RuleFor(x => x.UsuarioId).GreaterThan(0).When(x => x.UsuarioId.HasValue);
    }
}
