using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class CrearCargaCombustibleValidator : AbstractValidator<CrearCargaCombustibleDto>
{
    public CrearCargaCombustibleValidator()
    {
        RuleFor(x => x.Fecha).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow);
        RuleFor(x => x.Kilometraje).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Litros).GreaterThan(0).LessThanOrEqualTo(99999999.99m);
        RuleFor(x => x.PrecioPorLitro).GreaterThan(0).LessThanOrEqualTo(99999999.99m);
    }
}
