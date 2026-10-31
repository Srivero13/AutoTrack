using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class IniciarSesionValidator : AbstractValidator<IniciarSesionDto>
{
    public IniciarSesionValidator()
    {
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Clave).NotEmpty();
    }
}
