using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioDto>
{
    public CrearUsuarioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Clave).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.Telefono).MaximumLength(20);
    }
}
