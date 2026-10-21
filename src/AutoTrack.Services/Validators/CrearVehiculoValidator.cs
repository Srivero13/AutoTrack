using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class CrearVehiculoValidator : AbstractValidator<CrearVehiculoDto>
{
    public CrearVehiculoValidator()
    {
        RuleFor(x => x.ModeloId).GreaterThan(0);
        RuleFor(x => x.Placa).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Anio).InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1));
        RuleFor(x => x.KilometrajeActual).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TipoCombustible).NotEmpty().MaximumLength(30);
    }
}
