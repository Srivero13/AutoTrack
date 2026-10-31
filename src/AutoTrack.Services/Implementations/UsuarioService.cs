using AutoMapper;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Entities;
using AutoTrack.Core.Exceptions;
using AutoTrack.Core.Interfaces;
using AutoTrack.Services.Interfaces;
using FluentValidation;

namespace AutoTrack.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CrearUsuarioDto> _crearValidator;
    private readonly IValidator<ActualizarUsuarioDto> _actualizarValidator;

    public UsuarioService(
        IUsuarioRepository repository,
        IMapper mapper,
        IValidator<CrearUsuarioDto> crearValidator,
        IValidator<ActualizarUsuarioDto> actualizarValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
    }

    public async Task<List<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await _repository.ObtenerTodosAsync();
        return _mapper.Map<List<UsuarioDto>>(usuarios);
    }

    public async Task<UsuarioDto> ObtenerPorIdAsync(int id)
    {
        var usuario = await _repository.ObtenerPorIdAsync(id);

        if (usuario == null)
            throw new RecursoNoEncontradoException("Usuario no encontrado");

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto> CrearAsync(CrearUsuarioDto datos)
    {
        datos.Nombre = datos.Nombre?.Trim() ?? string.Empty;
        datos.Apellido = datos.Apellido?.Trim() ?? string.Empty;
        datos.Correo = datos.Correo?.Trim().ToLowerInvariant() ?? string.Empty;
        datos.Telefono = NormalizarTelefono(datos.Telefono);
        await _crearValidator.ValidateAndThrowAsync(datos);

        if (await _repository.ExisteCorreoAsync(datos.Correo))
            throw new ReglaNegocioException("Ya existe un usuario con ese correo");

        var usuario = _mapper.Map<Usuario>(datos);
        usuario.Activo = true;
        usuario.FechaRegistro = DateTime.UtcNow;

        await _repository.AgregarAsync(usuario);
        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto> ActualizarAsync(int id, ActualizarUsuarioDto datos)
    {
        datos.Nombre = datos.Nombre?.Trim();
        datos.Apellido = datos.Apellido?.Trim();
        datos.Correo = datos.Correo?.Trim().ToLowerInvariant();
        datos.Telefono = NormalizarTelefono(datos.Telefono);
        await _actualizarValidator.ValidateAndThrowAsync(datos);

        var usuario = await _repository.ObtenerPorIdAsync(id);

        if (usuario == null)
            throw new RecursoNoEncontradoException("Usuario no encontrado");

        if (datos.Correo != null && await _repository.ExisteCorreoAsync(datos.Correo, id))
            throw new ReglaNegocioException("Ya existe un usuario con ese correo");

        datos.Nombre ??= usuario.Nombre;
        datos.Apellido ??= usuario.Apellido;
        datos.Correo ??= usuario.Correo;
        datos.Telefono ??= usuario.Telefono;

        _mapper.Map(datos, usuario);
        await _repository.GuardarAsync();
        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task EliminarAsync(int id)
    {
        var usuario = await _repository.ObtenerPorIdAsync(id);

        if (usuario == null)
            throw new RecursoNoEncontradoException("Usuario no encontrado");

        usuario.Activo = false;
        await _repository.GuardarAsync();
    }

    private static string? NormalizarTelefono(string? telefono)
    {
        return string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
    }
}
