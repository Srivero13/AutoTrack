using AutoTrack.Core.DTOs;
using FluentValidation;

namespace AutoTrack.Services.Validators;

public class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioDto>
{
    public ActualizarUsuarioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(80).When(x => x.Nombre != null);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(80).When(x => x.Apellido != null);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(150).When(x => x.Correo != null);
        RuleFor(x => x.Telefono).MaximumLength(20);
    }
}
